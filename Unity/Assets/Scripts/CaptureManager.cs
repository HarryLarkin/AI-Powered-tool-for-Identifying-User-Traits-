using System;
using System.IO;
using System.Text;
using UnityEngine;

// Logs pose and simple events. CSV columns:
// time,source,posX,posY,posZ,rotX,rotY,rotZ,rotW
public class CaptureManager : MonoBehaviour
{
    [Header("Transforms")]
    public Transform leftController;
    public Transform rightController;

    [Header("Logging")]
    public string logFileName = "session_log.csv";
    public bool append = false;
    public int flushEveryNFrames = 60;

    private StreamWriter _writer;
    private int _frameCounter;

    void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, logFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        _writer = new StreamWriter(path, append, Encoding.UTF8);
        if (!append) _writer.WriteLine("time,source,posX,posY,posZ,rotX,rotY,rotZ,rotW");
        Debug.Log($"[CaptureManager] Logging to: {path}");
    }

    void Update()
    {
        float t = Time.time;

        if (Camera.main != null) LogPose(t, "HMD", Camera.main.transform);
        if (leftController  != null) LogPose(t, "LHand", leftController);
        if (rightController != null) LogPose(t, "RHand", rightController);

        // Example input event (left mouse as a stand-in for trigger)
        if (Input.GetMouseButtonDown(0))
            _writer.WriteLine($"{t},Trigger,1,0,0,0,0,0,0");

        if (++_frameCounter % flushEveryNFrames == 0) _writer.Flush();
    }

    private void LogPose(float t, string source, Transform tr)
    {
        Vector3 p = tr.position; Quaternion r = tr.rotation;
        _writer.WriteLine($"{t},{source},{p.x:F6},{p.y:F6},{p.z:F6},{r.x:F6},{r.y:F6},{r.z:F6},{r.w:F6}");
    }

    void OnApplicationQuit()
    {
        try { _writer?.Flush(); _writer?.Close(); } catch {}
    }
}
