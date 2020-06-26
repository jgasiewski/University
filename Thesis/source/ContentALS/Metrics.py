import math

import numpy as np
import pandas as pd

def mae(df):
    return np.mean(np.abs(df['rating'] - df['prediction']))


def rmse(df):
    return np.sqrt(np.mean(np.power(df['rating'] - df['prediction'], 2)))


def ndcg(df, k):
    df_g = df.groupby('uid')
    df['rank_pred'] = df_g['prediction'].rank(ascending=False, method='first')
    df['rank_ideal'] = df_g['rating'].rank(ascending=False, method='first')
    df['dcg'] = df['rating'] / np.log2(1 + df['rank_pred'])
    df['idcg'] = df['rating'] / np.log2(1 + df['rank_ideal'])
    dcg = df[df['rank_pred'] <= k][['uid', 'dcg']]
    idcg = df[df['rank_ideal'] <= k][['uid', 'idcg']]
    dcg = dcg.groupby('uid').sum()
    idcg = idcg.groupby('uid').sum()
    ndcg = dcg.merge(idcg, left_index=True, right_index=True)
    ndcg = np.mean(ndcg['dcg'] / ndcg['idcg'])
    return ndcg


def full_ndcg(algo, df, k):
    def calculate_pred_rank(algo, df):
        predictions = algo.predict_for_user(df.uid.values[0])
        indices = (-predictions).argsort()
        return pd.DataFrame({'uid': df.uid,
                             'iid': df.iid,
                             'rank_pred': [1 + np.nonzero(indices == iid)[0][0] for iid in df.iid.values]})

    df_g = df.groupby('uid')
    df = df.merge(df_g['uid', 'iid'].apply(lambda df: calculate_pred_rank(algo, df)),
                  on=['uid', 'iid'], how='left')
    df['rank_ideal'] = df_g['rating'].rank(ascending=False, method='first')
    df['dcg'] = df['rating'] / np.log2(1 + df['rank_pred'])
    df['idcg'] = df['rating'] / np.log2(1 + df['rank_ideal'])
    dcg = df[df['rank_pred'] <= k][['uid', 'dcg']]
    idcg = df[df['rank_ideal'] <= k][['uid', 'idcg']]
    dcg = dcg.groupby('uid').sum()
    idcg = idcg.groupby('uid').sum()
    ndcg = dcg.merge(idcg, left_index=True, right_index=True)
    ndcg.fillna(value=0.0, inplace=True)
    ndcg = np.mean(ndcg['dcg'] / ndcg['idcg'])
    ndcg = 0 if math.isnan(ndcg) else ndcg
    return ndcg
