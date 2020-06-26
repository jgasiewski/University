import numpy as np
from numpy.linalg import solve
import pandas as pd
from scipy.sparse import csr_matrix
import numba

class ContentALS():
    def __init__(self, users_num, items_num,
                 user_profiles, item_profiles,
                 base_rank=40, user_reg_loss=0.1, item_reg_loss=0.1, damping_factor=25):
        """Explicit Alternating Least Squares with content embeding.
        
        Arguments:
            users_num {int} -- Total number of users in system
            items_num {int} -- Total number of items in system
            user_profiles {numpy.ndarray(shape=(users_num, _), dtype=float)} -- Description of each user characteristics 
            item_profiles {numpy.ndarray(shape=(items_num, _), dtype=float)} -- Description of each item characteristics
        
        Keyword Arguments:
            base_rank {int} -- Number of latent factors (default: {40})
            user_reg_loss {float} -- Regularization loss of user factors matrix (default: {0.1})
            item_reg_loss {float} -- Regularization loss of item factors matrix (default: {0.1})
            damping_factor {int} -- Weight of global mean for bias calculation (default: {25})
        """
        self._init_status = 'partial'

        self._users_num = users_num
        self._items_num = items_num
        self._base_rank = base_rank
        self._full_rank = base_rank

        if user_profiles is not None:
            self._user_profile_rank = user_profiles.shape[1]
            self._full_rank += self._user_profile_rank
        else:
            self._user_profile_rank = 0
        if item_profiles is not None:
            self._item_profile_rank = item_profiles.shape[1]
            self._full_rank += self._item_profile_rank
        else:
            self._item_profile_rank = 0
        self._item_rank_offset = self._user_profile_rank + self._base_rank

        self._user_reg_loss = user_reg_loss
        self._item_reg_loss = item_reg_loss
        self._damping_factor = damping_factor
        
        self._user_factors = np.zeros((self._users_num, self._full_rank))
        self._item_factors = np.zeros((self._items_num, self._full_rank))

        if user_profiles is not None:
            self._user_factors[:, :self._user_profile_rank] = user_profiles
        if item_profiles is not None:
            self._item_factors[:, self._item_rank_offset:] = item_profiles

        self._ratings_ui = None
        self._ratings_iu = None
        self._biases = None


    def init_ratings(self, ratings):
        """Load trainset into model.
        
        Arguments:
            ratings {pandas.DataFrame(columns=['uid', 'iid', 'rating'])} -- Trainset ratings. uid and iid must be numbers
            inbetween 0 and users_num/items_num respectively.
        """
        self._init_status = 'ratings'
        self._biases = self._calculate_biases(ratings, self._damping_factor)
        for gr_col, bias_name in zip(['uid', 'iid'], ['user_bias', 'item_bias']):
            ratings = ratings.merge(self._biases[bias_name], on=gr_col, how='left')
            ratings['rating'] = ratings['rating'] - ratings[bias_name]
            ratings = ratings.drop(bias_name, axis=1)
        ratings['rating'] = ratings['rating'] - self._biases['mean']

        self._ratings_ui = csr_matrix((ratings['rating'], (ratings['uid'], ratings['iid'])),
                                      shape=(self._users_num, self._items_num))
        self._ratings_iu = csr_matrix((ratings['rating'], (ratings['iid'], ratings['uid'])),
                                      shape=(self._items_num, self._users_num))


    def _calculate_biases(self, ratings, damping_factor):
        """Calculate global mean and biases for each user and item.
        Biases are calculated as difference between global mean and weighted average of global mean with weight
        equal to dumping factor and all user/item rating with weight 1 for each.
        
        Arguments:
            ratings {pandas.DataFrame(columns=['uid', 'iid', 'rating'])} -- Ratings dataframe 
            for which biases shoul be calculated.
            damping_factor {int} -- Minimal number of ratings to use as trust factor.
        
        Returns:
            dict('mean': float,
                 'user_bias': pandasDataFrame(columns=['uid', 'user_bias']),
                 'item_bias': pandasDataFrame(columns=['iid', 'item_bias'])) -- Mapping of biases
        """
        mean = ratings['rating'].mean()
        result = {'mean': mean}

        item_biases = ratings.groupby('iid').agg({'rating': ['count', 'sum']})
        item_biases['item_bias'] = mean * damping_factor + item_biases['rating']['sum']
        item_biases['item_bias'] /= (item_biases['rating']['count'] + damping_factor)
        item_biases['item_bias'] -= mean
        item_biases = item_biases.reset_index()[['iid', 'item_bias']]
        item_biases.columns = item_biases.columns.droplevel(1)
        item_biases = pd.DataFrame(np.arange(self._items_num), columns=['iid']) \
                        .merge(item_biases, on='iid', how='left').fillna(0)
        result['item_bias'] = item_biases

        user_biases = ratings
        user_biases = user_biases.merge(item_biases, on='iid')
        user_biases['rating_biased'] = user_biases['rating'] - user_biases['item_bias']
        user_biases = user_biases.groupby('uid').agg({'rating_biased': ['count', 'sum']})
        user_biases['user_bias'] = mean * damping_factor + user_biases['rating_biased']['sum']
        user_biases['user_bias'] /= (user_biases['rating_biased']['count'] + damping_factor)
        user_biases['user_bias'] -= mean
        user_biases = user_biases.reset_index()[['uid', 'user_bias']]
        user_biases.columns = user_biases.columns.droplevel(1)
        user_biases = pd.DataFrame(np.arange(self._users_num), columns=['uid']) \
                        .merge(user_biases, on='uid', how='left').fillna(0)
        result['user_bias'] = user_biases

        return result


    def train(self, iterations=20):
        """Train model on preloaded ratings set. It may be called iteratively for feature improvement. 
        
        Keyword Arguments:
            iterations {int} -- Number of steps for all factors train phase (default: {20})
        """

        if self._init_status != 'full':
            self._init_status = 'full'
            self._user_factors[:, self._user_profile_rank:self._item_rank_offset] = \
                np.random.random(size=(self._users_num, self._base_rank))
            self._item_factors[:, self._user_profile_rank:self._item_rank_offset] = \
                np.random.random(size=(self._items_num, self._base_rank))

        for _ in range(iterations):
            self._update_step('user')
            self._update_step('item')


    def _update_step(self, mode):
        """Half of training step.
        
        Arguments:
            mode {str} -- 'user' or 'item', which matrix will be modified
        """
        if mode == 'user':
            reg = self._user_reg_loss
            ratings = self._ratings_ui
            update_rank = self._item_profile_rank + self._base_rank
            updated = self._user_factors[:, self._user_profile_rank:]
            updated_c = self._user_factors[:, :self._user_profile_rank]
            constant = self._item_factors[:, self._user_profile_rank:]
            constant_c = self._item_factors[:, :self._user_profile_rank]
        else:
            reg = self._item_reg_loss
            ratings = self._ratings_iu
            update_rank = self._user_profile_rank + self._base_rank
            updated = self._item_factors[:, :self._item_rank_offset]
            updated_c = self._item_factors[:, self._item_rank_offset:]
            constant = self._user_factors[:, :self._item_rank_offset]
            constant_c = self._user_factors[:, self._item_rank_offset:]
        
        ContentALS._calculation_step(ratings.data, ratings.indices, ratings.indptr,
                                     update_rank, reg,
                                     constant, updated,
                                     constant_c, updated_c)


    @staticmethod
    @numba.jit(numba.void(numba.float64[::1], numba.int32[::1], numba.int32[::1],
                          numba.int64, numba.float64,
                          numba.float64[:,:], numba.float64[:,:],
                          numba.float64[:,:], numba.float64[:,:]),
               nopython=True)
    def _calculation_step(ratings, indices, indptr,
                          update_rank, reg_loss,
                          fixed_factors, updated_factors,
                          side_fixed_factors, side_updated_factors):
        """Calculate and update part of factors based on ALS algorithm.
        
        Arguments:
            ratings {np.ndarray(shape=(_,), dtype=float)} -- Values of ratings
            indices {np.ndarray(shape=(_,), dtype=int)} -- Second coordinates in csr matrix, same length as ratings
            indptr {np.ndarray(shape=(fixed_factors.shape[0],), dtype=int)} -- Ends of slices linked to values
            of indices for each of first coordiante
            update_rank {int} -- Number of updated factors
            reg_loss {float} -- Regularization loss
            fixed_factors {np.ndarray(shape=(_, update_rank), dtype=float)} -- Factors strictly coresponding to 
            updated factors
            updated_factors {np.ndarray(shape=(_, update_rank), dtype=float)} -- Factors changed in current step
            side_fixed_factors {np.ndarray(shape=(_, _), dtype=float)} -- Fixed factors used for partial rating calculation
            side_updated_factors {np.ndarray(shape=(_, _), dtype=float)} -- Part of updated matrix, not changed 
            in this step
        """
        lambda_eye = np.eye(update_rank) * reg_loss
        rows_num = indptr.shape[0] - 1
        for idx in numba.prange(rows_num): # pylint: disable=not-an-iterable
            row_indices = indices[indptr[idx]:indptr[idx+1]]
            indices_num = len(row_indices)
            if indices_num == 0:
                continue
            MTM = fixed_factors[row_indices].T.dot(fixed_factors[row_indices])
            conv_ratings = ratings[indptr[idx]:indptr[idx+1]] \
                           - side_updated_factors[idx, :].dot(side_fixed_factors[row_indices, :].T)
            updated_factors[idx, :] = solve(MTM + lambda_eye * indices_num,
                                            conv_ratings.dot(fixed_factors[row_indices]))


    def predict(self, df):
        """Generate predicted ratings for given pairs of uid and iid.
        
        Arguments:
            df {pandas.DataFrame(columns=['uid', 'iid'])}
        
        Returns:
            df {pandas.DataFrame(columns=['uid', 'iid', 'prediction'])}
        """
        dot_prod = (lambda x: self._user_factors[x[0]].dot(self._item_factors[x[1]].T))
        df['prediction'] = df[['uid', 'iid']].apply(dot_prod, axis=1)

        for gr_col, bias_name in zip(['uid', 'iid'], ['user_bias', 'item_bias']):
            df = df.merge(self._biases[bias_name], on=gr_col, how='left')
            df['prediction'] += df[bias_name]
            df = df.drop(bias_name, axis=1)
        df['prediction'] += self._biases['mean']
        return df

      
    def predict_for_user(self, user):
        """Generate all predictions for selected user
        
        Arguments:
            user {int} -- User id
        
        Returns:
            numpy.ndarray(shape=(users_num, _), dtype=float) -- Predicted ratings
        """
        result = self._user_factors[user].dot(self._item_factors.T)
        result += self._biases['item_bias']['item_bias'].values
        result += (self._biases['user_bias']['user_bias'][user] + self._biases['mean'])
        return result
