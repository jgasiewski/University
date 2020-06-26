import numpy as np
import pandas as pd
import pytest

from ContentALS import Metrics


def test_mae():
    input_df = pd.DataFrame({"rating": [1, 4, 5],
                             "prediction": [2, 2, 5]})
    assert Metrics.mae(input_df) == pytest.approx(1, 1e-6)


def test_rmse():
    input_df = pd.DataFrame({"rating": [1, 4, 5],
                             "prediction": [2, 2, 5]})
    assert Metrics.rmse(input_df) == pytest.approx(1.29099445, 1e-6)


def test_single_user_ndcg_perfect_order():
    input_df = pd.DataFrame({"uid": [1, 1, 1],
                             "iid": [1, 2, 3],
                             "rating": [6, 2, 3],
                             "prediction": [8, 1, 4]})
    assert Metrics.ndcg(input_df, 3) == pytest.approx(1, 1e-6)
    assert Metrics.ndcg(input_df, 2) == pytest.approx(1, 1e-6)


def test_single_user_ndcg_bad_order():
    input_df = pd.DataFrame({"uid": [1, 1, 1, 1, 1, 1],
                             "iid": [1, 2, 3, 4, 5, 6],
                             "rating": [6, 2, 3, 0, 0, 0],
                             "prediction": [8, 1, 4, 10, 10, 10]})
    assert Metrics.ndcg(input_df, 3) == pytest.approx(0, 1e-6)
    assert Metrics.ndcg(input_df, 2) == pytest.approx(0, 1e-6)


def test_single_user_ndcg():
    input_df = pd.DataFrame({"uid": [1, 1, 1],
                             "iid": [1, 2, 3],
                             "rating": [3, 2, 1],
                             "prediction": [3, 1, 2]})
    assert Metrics.ndcg(input_df, 3) == pytest.approx(0.972504, 1e-6)
    assert Metrics.ndcg(input_df, 2) == pytest.approx(0.851959, 1e-6)


def test_multiple_user_ndcg():
    input_df = pd.DataFrame({"uid": [1, 1, 1, 2, 2, 2],
                             "iid": [1, 2, 3, 4, 5, 6],
                             "rating": [3, 2, 1, 5, 5, 5],
                             "prediction": [3, 1, 2, 6, 6, 6]})
    assert Metrics.ndcg(input_df, 3) == pytest.approx(0.986252, 1e-6)
    assert Metrics.ndcg(input_df, 2) == pytest.approx(0.925980, 1e-6)


class NDCGAlgo:
    def __init__(self):
        result_base = [[6, 5, 4, 3, 2, 1],
                       [0, 0, 0, 1, 1, 1],
                       [2, 4, 5, 1, 6, 3]]
        self.result_base = np.array(result_base, dtype=np.float32)

    def predict_for_user(self, uids):
        return self.result_base[uids, :]


@pytest.fixture
def ndcg_algo():
    return NDCGAlgo()


def test_single_user_full_ndcg_perfect_order(ndcg_algo):
    input_df = pd.DataFrame({"uid": [0, 0, 0, 0, 0, 0],
                             "iid": [0, 1, 2, 3, 4, 5],
                             "rating": [6, 5, 4, 3, 2, 1]})
    assert Metrics.full_ndcg(ndcg_algo, input_df, 6) == pytest.approx(1, 1e-6)
    assert Metrics.full_ndcg(ndcg_algo, input_df, 5) == pytest.approx(1, 1e-6)


def test_single_user_full_ndcg_bad_order(ndcg_algo):
    input_df = pd.DataFrame({"uid": [1, 1, 1],
                             "iid": [0, 1, 2],
                             "rating": [6, 2, 3]})
    assert Metrics.full_ndcg(ndcg_algo, input_df, 3) == pytest.approx(0, 1e-6)
    assert Metrics.full_ndcg(ndcg_algo, input_df, 2) == pytest.approx(0, 1e-6)


def test_single_user_full_ndcg(ndcg_algo):
    input_df = pd.DataFrame({"uid": [0, 0, 0],
                             "iid": [2, 3, 1],
                             "rating": [3, 2, 1]})
    assert Metrics.full_ndcg(ndcg_algo, input_df, 3) == pytest.approx(0.447500, 1e-5)
    assert Metrics.full_ndcg(ndcg_algo, input_df, 2) == pytest.approx(0.148041, 1e-5)


def test_multiple_user_full_ndcg(ndcg_algo):
    input_df = pd.DataFrame({"uid": [0, 0, 0, 1, 1, 1, 2, 2],
                             "iid": [0, 1, 2, 0, 1, 4, 1, 2],
                             "rating": [1, 2, 3, 4, 5, 6, 7, 8]})
    assert Metrics.full_ndcg(ndcg_algo, input_df, 3) == pytest.approx(0.605921, 1e-5)
    assert Metrics.full_ndcg(ndcg_algo, input_df, 2) == pytest.approx(0.450249, 1e-5)
