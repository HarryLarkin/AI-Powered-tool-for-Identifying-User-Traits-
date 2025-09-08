using System.Collections.Generic;
using UnityEngine;

/// Dummy classifier that adapts to your recent movement so class toggles 0/1.
public class DummyInferenceClient : MonoBehaviour
{
    [Header("Adaptive thresholding")]
    [Tooltip("How many recent vel_mean samples to keep (smaller = more responsive).")]
    public int historySize = 20;

    [Tooltip("Threshold = median(vel_mean_history) + kStd * vel_std.")]
    public float kStd = 0.25f;

    private readonly Queue<float> _hist = new Queue<float>();

    /// features = [vel_mean, vel_std, ang_speed]
    public float[] Predict(float[] features)
    {
        float velMean = (features != null && features.Length > 0) ? features[0] : 0f;
        float velStd  = (features != null && features.Length > 1) ? features[1] : 0f;

        _hist.Enqueue(velMean);
        while (_hist.Count > historySize) _hist.Dequeue();

        float med = Median(_hist);
        float thr = med + kStd * Mathf.Max(velStd, 1e-4f);

        int cls = velMean > thr ? 1 : 0;
        return new float[] { cls };
    }

    private float Median(IEnumerable<float> vals)
    {
        var list = new List<float>(vals);
        if (list.Count == 0) return 0f;
        list.Sort();
        int m = list.Count / 2;
        return (list.Count % 2 == 1) ? list[m] : 0.5f * (list[m - 1] + list[m]);
    }
}
