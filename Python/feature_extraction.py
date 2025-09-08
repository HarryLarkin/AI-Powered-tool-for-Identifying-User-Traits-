"""
Reads VR session CSV logs and produces a features CSV suitable for training.

Input CSV columns (from CaptureManager):
time,source,posX,posY,posZ,rotX,rotY,rotZ,rotW

Usage:
  python feature_extraction.py --log session_log.csv --out features.csv --window 5 --hop 2.5
"""
import argparse
import json
import numpy as np
import pandas as pd

def parse_args():
    ap = argparse.ArgumentParser()
    ap.add_argument("--log", required=True, help="Path to session_log.csv")
    ap.add_argument("--out", required=True, help="Path to output features CSV")
    ap.add_argument("--window", type=float, default=5.0, help="Window length (s)")
    ap.add_argument("--hop", type=float, default=2.5, help="Hop length (s)")
    ap.add_argument("--meta", default="feature_config.json", help="Where to save feature config")
    return ap.parse_args()

def compute_velocity(positions, times):
    vels = []
    for i in range(1, len(positions)):
        dt = times[i] - times[i-1]
        if dt <= 0:
            continue
        vel = np.linalg.norm(positions[i] - positions[i-1]) / dt
        vels.append(vel)
    if len(vels) == 0:
        return np.array([0.0]), 0.0, 0.0
    v = np.asarray(vels)
    return v, float(v.mean()), float(v.var())

def window_features(df, window, hop):
    tmin, tmax = df['time'].min(), df['time'].max()
    starts = np.arange(tmin, tmax - window + 1e-6, hop)
    rows = []
    for s in starts:
        e = s + window
        seg = df[(df['time'] >= s) & (df['time'] < e)]
        if seg.empty:
            continue
        hmd = seg[seg['source'] == 'HMD'][['time','posX','posY','posZ']].values
        if len(hmd) < 2:
            continue
        times = hmd[:,0]
        positions = hmd[:,1:]
        _, vel_mean, vel_var = compute_velocity(positions, times)
        rows.append({
            "t_start": s,
            "vel_mean": vel_mean,
            "vel_var": vel_var
        })
    return pd.DataFrame(rows)

def main():
    args = parse_args()
    df = pd.read_csv(args.log)
    feats = window_features(df, args.window, args.hop)
    if feats.empty:
        print("No features extracted; check your log or parameters.")
        return
    feats.to_csv(args.out, index=False)
    meta = {
        "features": ["vel_mean","vel_var"],
        "window_seconds": args.window,
        "hop_seconds": args.hop
    }
    with open(args.meta, "w") as f:
        json.dump(meta, f, indent=2)
    print(f"Saved features -> {args.out}")
    print(f"Saved config   -> {args.meta}")

if __name__ == "__main__":
    main()
