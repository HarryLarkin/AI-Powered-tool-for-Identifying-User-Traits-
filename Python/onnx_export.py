"""
Convert a pre-trained scikit-learn model (.pkl) into ONNX format.

Usage:
  python onnx_export.py --pickle model.pkl --n_features 2 --out model.onnx
"""
import argparse
import joblib
from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType

def parse_args():
    ap = argparse.ArgumentParser()
    ap.add_argument("--pickle", required=True, help="Path to saved sklearn model (.pkl)")
    ap.add_argument("--n_features", type=int, required=True, help="Number of input features")
    ap.add_argument("--out", default="model.onnx", help="Output ONNX file")
    return ap.parse_args()

def main():
    args = parse_args()
    model = joblib.load(args.pickle)
    initial_type = [("input", FloatTensorType([None, args.n_features]))]
    onnx_model = convert_sklearn(model, initial_types=initial_type)
    with open(args.out, "wb") as f:
        f.write(onnx_model.SerializeToString())
    print(f"Saved ONNX model -> {args.out}")

if __name__ == "__main__":
    main()
