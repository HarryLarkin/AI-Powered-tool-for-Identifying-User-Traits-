README
This project is released for academic purposes only.


This repository contains the Unity and Python code developed for the dissertation project “AI-Powered Tool for Identifying User Traits in VR.”

The aim is to demonstrate an end-to-end pipeline for data capture → feature extraction → inference → feedback, using Unity for VR integration and Python for model training/export.

The unity demo does as follows:

Captures head movement from the Main Camera (via FlyCamera).

Computes simple velocity-based features (SimpleFeatureAggregator).

Classifies activity either by:

Direct-threshold mode (default) – if movement is above a threshold → green, else red.

DummyInferenceClient mode – adaptive rule using velocity median/std.

Sends the result to a UI indicator (red/green box in the top-left corner of the Game tab).

Scene setup

Main Camera → add FlyCamera.

InferenceSystem (Empty GO) → add SimpleFeatureAggregator + DummyInferenceClient.

Set Head = Main Camera.

Set Ui Indicator = Raw Image (with PredictionIndicator).

Canvas → Raw Image (PredictionIndicator) → attach PredictionIndicator (auto-wires to its own Graphic).

Controls

WASD + mouse → move camera.

Game tab (top-left) → red box when stationary, green when moving.

Press G/R (if IndicatorKeyTest is attached) → manually force green/red (for demo verification).

In regards to the python:
Python Pipeline
Files

feature_extraction.py → Extracts features (velocities, gaze, physiology placeholders).

training_pipeline.py → Trains ML models on extracted features.

onnx_export.py → Exports trained model to ONNX for Unity.

Requirements

Install with:

pip install -r requirements.txt

Note

Since no user testing was conducted, no trained ONNX model is included.
The pipeline is implemented and ready to accept real data in the future.

Final Notes:
By default, SimpleFeatureAggregator.useDummy = false. This ensures the red/green indicator visibly flips when moving vs. stationary — ideal for demonstration.

The DummyInferenceClient is also included and can be enabled (useDummy = true) to illustrate adaptive behaviour.

ONNX-based inference (InferenceClient.cs) is not included in the default build, but the Unity system is architected to integrate ONNX models exported from the Python pipeline.