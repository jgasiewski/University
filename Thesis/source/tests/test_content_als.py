import numpy as np
import pandas as pd
from scipy.sparse import csr_matrix
import pytest

from ContentALS import ContentALS
from ContentALS import Metrics


CALS = ContentALS.ContentALS


def test_default_init():
    algo = CALS(users_num=10, items_num=20,
                user_profiles=None, item_profiles=None)
    assert algo._users_num == 10
    assert algo._items_num == 20
    assert algo._base_rank == 40
    assert algo._user_profile_rank == 0
    assert algo._item_profile_rank == 0
    assert algo._full_rank == 40
    assert algo._user_reg_loss == 0.1
    assert algo._item_reg_loss == 0.1
    assert algo._damping_factor == 25
    assert algo._user_factors.shape == (algo._users_num, algo._full_rank)
    assert algo._item_factors.shape == (algo._items_num, algo._full_rank)


def test_full_init():
    algo = CALS(users_num=10, items_num=20,
                user_profiles=2*np.ones(shape=(10, 4), dtype=np.float),
                item_profiles=np.ones(shape=(20, 3), dtype=np.float),
                base_rank=10, user_reg_loss=0.2, item_reg_loss=0.5, damping_factor=5)
    assert algo._base_rank == 10
    assert algo._user_profile_rank == 4
    assert algo._item_profile_rank == 3
    assert algo._full_rank == 17
    assert algo._user_reg_loss == 0.2
    assert algo._item_reg_loss == 0.5
    assert algo._damping_factor == 5
    assert algo._user_factors.shape == (algo._users_num, algo._full_rank)
    assert algo._item_factors.shape == (algo._items_num, algo._full_rank)
    assert np.all(algo._user_factors[:, :4] == 2)
    assert np.all(algo._user_factors[:, 4:] == 0)
    assert np.all(algo._item_factors[:, :14] == 0)
    assert np.all(algo._item_factors[:, 14:] == 1)


def test_bias_calculation():
    algo = CALS(users_num=5, items_num=6,
                user_profiles=None, item_profiles=None,
                damping_factor=1)

    ratings = pd.DataFrame({'uid': [0, 0, 0, 0, 1, 1, 1, 1],
                            'iid': [0, 1, 2, 3, 1, 2, 3, 4],
                            'rating': [1, 5, 2, 1, 5, 3, 2, 1]})
    
    biases = algo._calculate_biases(ratings, 1)
    assert biases['mean'] == 2.5
    assert np.allclose(biases['item_bias']['item_bias'], [-0.75, 5/3, 0, -2/3, -0.75, 0])
    assert np.allclose(biases['user_bias']['user_bias'], [-0.25, 0.15, 0, 0, 0])


def test_ratings_init():
    algo = CALS(users_num=5, items_num=6,
                user_profiles=None, item_profiles=None,
                damping_factor=1)

    ratings = pd.DataFrame({'uid': [0, 0, 0, 0, 1, 1, 1, 1],
                            'iid': [0, 1, 2, 3, 1, 2, 3, 4],
                            'rating': [1, 5, 2, 1, 5, 3, 2, 1]})

    algo.init_ratings(ratings)
    assert algo._biases['mean'] == 2.5
    assert np.allclose(algo._biases['item_bias']['item_bias'], [-0.75, 5/3, 0, -2/3, -0.75, 0])
    assert np.allclose(algo._biases['user_bias']['user_bias'], [-0.25, 0.15, 0, 0, 0])
    assert np.all(algo._ratings_ui.indices == [0, 1, 2, 3, 1, 2, 3, 4])
    assert np.all(algo._ratings_ui.indptr == [0, 4, 8, 8, 8, 8])
    assert np.allclose(algo._ratings_ui.data, [-0.5, 13/12, -0.25, -7/12, 0.1+7/12, 0.35, 1/60, -0.9])
    assert np.all(algo._ratings_iu.indices == [0, 0, 1, 0, 1, 0, 1, 1])
    assert np.all(algo._ratings_iu.indptr == [0, 1, 3, 5, 7, 8, 8])
    assert np.allclose(algo._ratings_iu.data, [-0.5, 13/12, 0.1+7/12, -0.25, 0.35, -7/12, 1/60, -0.9])


def test_calculation_step():
    np.random.seed = 0

    user_factors = np.random.random(size=(10, 6))
    item_factors = 2*np.random.random(size=(20, 6))

    user_factors_partial = np.copy(user_factors)
    user_factors_partial[:, :3] = 0
    item_factors_full = np.copy(item_factors)

    ratings = csr_matrix(user_factors.dot(item_factors.T))

    CALS._calculation_step(ratings.data, ratings.indices, ratings.indptr,
                           update_rank=3, reg_loss=1e-12,
                           fixed_factors=item_factors_full[:, :3],
                           updated_factors=user_factors_partial[:, :3],
                           side_fixed_factors=item_factors_full[:, 3:],
                           side_updated_factors=user_factors_partial[:, 3:])

    assert np.allclose(user_factors_partial[:, :3], user_factors[:, :3])
    assert np.all(user_factors_partial[:, 3:] == user_factors[:, 3:])
    assert np.all(item_factors_full == item_factors)


def test_update_user_step():
    np.random.seed = 0

    user_factors = np.random.random(size=(10, 6))
    item_factors = 2*np.random.random(size=(20, 6))
    ratings = csr_matrix(user_factors.dot(item_factors.T))

    algo = CALS(users_num=10, items_num=20,
                user_profiles=user_factors[:, :3],
                item_profiles=item_factors[:, 5:],
                base_rank=2, user_reg_loss=1e-12, item_reg_loss=1e-12,
                damping_factor=0)
    algo._item_factors[:, :] = item_factors[:, :]
    algo._ratings_ui = ratings
    
    algo._update_step('user')

    assert np.allclose(algo._user_factors[:, 3:], user_factors[:, 3:])
    assert np.all(algo._user_factors[:, :3] == user_factors[:, :3])
    assert np.all(algo._item_factors == item_factors)


def test_update_item_step():
    np.random.seed = 0

    user_factors = np.random.random(size=(10, 6))
    item_factors = 2*np.random.random(size=(20, 6))
    ratings = csr_matrix(user_factors.dot(item_factors.T).T)

    algo = CALS(users_num=10, items_num=20,
                user_profiles=user_factors[:, :1],
                item_profiles=item_factors[:, 3:],
                base_rank=2, user_reg_loss=1e-12, item_reg_loss=1e-12,
                damping_factor=0)
    algo._user_factors[:, :] = user_factors[:, :]
    algo._ratings_iu = ratings
    
    algo._update_step('item')

    assert np.allclose(algo._item_factors[:, :3], item_factors[:, :3])
    assert np.all(algo._item_factors[:, 3:] == item_factors[:, 3:])
    assert np.all(algo._user_factors == user_factors)


def test_full_training():
    np.random.seed = 0
    
    user_factors = np.random.random(size=(10, 6))
    item_factors = 2*np.random.random(size=(20, 6))
    ratings = user_factors.dot(item_factors.T)

    algo = CALS(users_num=10, items_num=20,
                user_profiles=user_factors[:, :2],
                item_profiles=item_factors[:, 4:],
                base_rank=2, user_reg_loss=1e-8, item_reg_loss=1e-8,
                damping_factor=0)
    algo._ratings_ui = csr_matrix(ratings)
    algo._ratings_iu = csr_matrix(ratings.T)

    algo.train(100)
    result_diff = algo._user_factors.dot(algo._item_factors.T) - user_factors.dot(item_factors.T)
    assert np.max(np.abs(result_diff)) < 1e-5


def test_predict():
    algo = CALS(users_num=5, items_num=3, base_rank=5,
                user_profiles=None, item_profiles=None)
    algo._biases = {
            'mean': 3,
            'item_bias': pd.DataFrame(
                {
                    'iid': [0, 1, 2],
                    'item_bias': [0.3, -1.0, 0],
                }),
            'user_bias': pd.DataFrame(
                {
                    'uid': [0, 1, 2, 3, 4],
                    'user_bias': [0.1, -0.1, 0, 0, 0],
                })
        }
    algo._user_factors[:, :] = [[1, 2, 3, 4, 5],
                                [2, 3, 1, 4, 5],
                                [1, 1, 2, 1, 1],
                                [1, 2, 3, 1, 1],
                                [1, 4, 2, 3, 2]]
    algo._item_factors[:, :] = [[0.1, 0.1, 0.1, 0.1, 0.1],
                                [0.1, 0.2, 0.1, 0.3, 0.1],
                                [0.5, 0.1, 0.1, 0.1, 0.3]]

    test_df = pd.DataFrame({
        'uid': [1, 0, 2, 2, 3],
        'iid': [1, 2, 1, 2, 0],
        'rating': [4.5, 6.0, 2.9, 4.2, 4.1]
    })

    result = algo.predict(test_df)
    assert 'rating' in result.columns
    assert 'prediction' in result.columns
    assert np.allclose(result['rating'], result['prediction'])


def test_predict_for_user():
    algo = CALS(users_num=5, items_num=3, base_rank=5,
                user_profiles=None, item_profiles=None)
    algo._biases = {
            'mean': 3,
            'item_bias': pd.DataFrame(
                {
                    'iid': [0, 1, 2],
                    'item_bias': [0.3, -1.0, 0],
                }),
            'user_bias': pd.DataFrame(
                {
                    'uid': [0, 1, 2, 3, 4],
                    'user_bias': [0.1, -0.1, 0, 0, 0],
                })
        }
    algo._user_factors[0, :] = [1, 2, 3, 4, 5]
    algo._item_factors[:, :] = [[0.1, 0.1, 0.1, 0.1, 0.1],
                                [0.1, 0.2, 0.1, 0.3, 0.1],
                                [0.5, 0.1, 0.1, 0.1, 0.3]]

    assert np.allclose(algo.predict_for_user(0), [4.9, 4.6, 6])
