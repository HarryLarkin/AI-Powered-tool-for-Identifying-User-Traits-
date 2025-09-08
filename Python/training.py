"""
Trains a simple RandomForest model on features.csv and exports to model.onnx.

Expected input CSV (from feature_extraction):
  columns: t_start, vel_mean, vel_var[, label]

If 'label' column is missing, we create a synthetic label for demo purposes.

Usage:
  python training_pipeline.py --features features.csv --model model.onnx
"""
import argparse
import json
import pandas as pd
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType

def parse_args():
    ap = argparse.ArgumentParser()
    ap.add_argument("--features", required=True, help="Path to features CSV")
    ap.add_argument("--model", default="model.onnx", help="Output ONNX path")
    ap.add_argument("--config", default="feature_config.json", help="Feature config (from extraction)")
    ap.add_argument("--n_estimators", type=int, default=100)
    return ap.parse_args()

def main():
    args = parse_args()
    df = pd.read_csv(args.features)
    with open(args.config, "r") as f:
        meta = json.load(f)
    feature_cols = meta["features"]

    X = df[feature_cols].values.astype("float32")

    if "label" in df.columns:
        y = df["label"].values.astype(int)
    else:
        # For demo: classify based on whether vel_mean is above median
        y = (df["vel_mean"] > df["vel_mean"].median()).astype(int).values

    Xtr, Xte, ytr, yte = train_test_split(X, y, test_size=0.25, random_state=42, stratify=y)
    clf = RandomForestClassifier(n_estimators=args.n_estimators, random_state=42)
    clf.fit(Xtr, ytr)
    ypred = clf.predict(Xte)
    print(classification_report(yte, ypred, digits=3))

    initial_type = [("input", FloatTensorType([None, X.shape[1]]))]
    onnx_model = convert_sklearn(clf, initial_types=initial_type)
    with open(args.model, "wb") as f:
        f.write(onnx_model.SerializeToString())
    print(f"Saved ONNX model -> {args.model}")

if __name__ == "__main__":
    main()
