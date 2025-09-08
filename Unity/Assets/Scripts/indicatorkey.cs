using UnityEngine;

public class IndicatorKeyTest : MonoBehaviour
{
    public PredictionIndicator indicator;

    void Reset()
    {
        if (indicator == null) indicator = GetComponent<PredictionIndicator>();
        if (indicator == null) indicator = FindObjectOfType<PredictionIndicator>();
    }

    void Update()
    {
        if (indicator == null) return;

        if (Input.GetKeyDown(KeyCode.G)) indicator.SetClass(1); // green
        if (Input.GetKeyDown(KeyCode.R)) indicator.SetClass(0); // red
    }
}
