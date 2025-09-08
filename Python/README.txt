Python Extraction and Training

This folder contains the Python scripts for training the AI model used in Unity.

Setup
```bash
cd Python
pip install -r requirements.txt

To run:
Extracts features from unity session log
python feature_extraction.py --log ../path/to/session_log.csv --out features.csv --window 5 --hop 2.5
Train and export the ONNX model
python training_pipeline.py --features features.csv --model model.onnx
Then copy that model into:
Unity/Assets/StreamingAssets/model.onnx


