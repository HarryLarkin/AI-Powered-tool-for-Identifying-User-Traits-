using System.Collections.Generic;
using UnityEngine;

public class SimpleFeatureAggregator : MonoBehaviour
{
    [Header("Inputs")]
    public Transform head;                     // drag Main Camera here
    public int windowFrames = 120;             // ~1.3s @ 90fps
    public float inferEverySeconds = 0.5f;     // inference cadence (sec)

    [Header("Backend")]
    public bool useDummy = false;              // set FALSE first to verify UI flips
    public DummyInferenceClient dummy;         // used only when useDummy = true

    [Header("UI")]
    public PredictionIndicator uiIndicator;    // drag your Raw Image (with PredictionIndicator)
    public bool logToConsole = true;

    [Header("Direct classifier (when useDummy = false)")]
    [Tooltip("If vel_mean (m/s) is above this, class=1 (green); else class=0 (red).")]
    public float movementThreshold = 0.05f;

    private struct Sample { public Vector3 p; public float t; }
    private readonly Queue<Sample> _samples = new Queue<Sample>();
    private float _timer;

    void Start()
    {
        if (head == null && Camera.main != null) head = Camera.main.transform;

        if (useDummy)
        {
            if (dummy == null) dummy = GetComponent<DummyInferenceClient>();
            if (dummy == null) dummy = gameObject.AddComponent<DummyInferenceClient>();
        }

        if (uiIndicator == null) uiIndicator = FindObjectOfType<PredictionIndicator>();
        Debug.Log("[Aggregator] Start() running. useDummy=" + useDummy);
    }

    void Update()
    {
        if (head == null) return;

        // accumulate samples
        _samples.Enqueue(new Sample { p = head.position, t = Time.time });
        while (_samples.Count > windowFrames) _samples.Dequeue();

        _timer += Time.deltaTime;
        if (_timer < inferEverySeconds || _samples.Count < 3) return;
        _timer = 0f;

        // features: [vel_mean, vel_std, ang(placeholder)]
        var feats = ComputeFeatures();
        float clsFloat;

        if (useDummy && dummy != null)
        {
            var pred = dummy.Predict(feats);              // [0] or [1]
            clsFloat = (pred != null && pred.Length > 0) ? pred[0] : 0f;

            if (logToConsole)
            {
                int clsInt = (clsFloat >= 0.5f) ? 1 : 0;
                Debug.Log($"[Dummy] vel_mean={feats[0]:F3} vel_std={feats[1]:F3} ang={feats[2]:F3} -> pred={clsInt}");
            }
        }
        else
        {
            // Direct, transparent rule: move vs. still
            clsFloat = (feats[0] > movementThreshold) ? 1f : 0f;

            if (logToConsole)
            {
                int clsInt = (clsFloat > 0.5f) ? 1 : 0;
                Debug.Log($"[Direct] vel_mean={feats[0]:F3} (thr={movementThreshold:F3}) vel_std={feats[1]:F3} -> cls={clsInt}");
            }
        }

        // drive UI
        if (uiIndicator == null) uiIndicator = FindObjectOfType<PredictionIndicator>();
        if (uiIndicator != null)
        {
            int cls = (clsFloat >= 0.5f) ? 1 : 0;
            uiIndicator.SetClass(cls);
        }
        else
        {
            Debug.LogWarning("[Aggregator] uiIndicator is null; cannot update colour.");
        }
    }

    private float[] ComputeFeatures()
    {
        var arr = _samples.ToArray();
        int n = arr.Length;
        if (n < 3) return new float[] { 0f, 0f, 0f };

        var vels = new List<float>(n - 1);
        for (int i = 1; i < n; i++)
        {
            float dt = Mathf.Max(arr[i].t - arr[i - 1].t, 1e-3f);
            float v  = (arr[i].p - arr[i - 1].p).magnitude / dt; // m/s
            vels.Add(v);
        }

        // mean & std
        float mean = 0f; for (int i = 0; i < vels.Count; i++) mean += vels[i];
        mean /= Mathf.Max(vels.Count, 1);
        float var = 0f; for (int i = 0; i < vels.Count; i++) { float d = vels[i] - mean; var += d * d; }
        var /= Mathf.Max(vels.Count - 1, 1);
        float std = Mathf.Sqrt(var);

        float ang = 0f; // placeholder
        return new float[] { mean, std, ang };
    }
}

