using UnityEngine;
using UnityEngine.UI;

public class PredictionIndicator : MonoBehaviour
{
    // Works with RawImage OR Image (both inherit Graphic)
    public Graphic indicator;
    public Color class0 = Color.red;
    public Color class1 = Color.green;

    int _currentClass = -1;

    void Reset()
    {
        if (indicator == null) indicator = GetComponent<Graphic>();
    }

    public void UpdateIndicator(float[] outputs)
    {
        if (outputs == null || outputs.Length == 0) return;
        SetClass(outputs[0] >= 0.5f ? 1 : 0);
    }

    public void SetClass(int cls)
    {
        if (indicator == null) indicator = GetComponent<Graphic>();
        if (indicator == null) return;

        if (cls == _currentClass) return;
        _currentClass = cls;

        indicator.color = (cls == 1) ? class1 : class0;
        Debug.Log($"[Indicator] SetClass({cls}) -> color={(cls==1? "green" : "red")}");
    }
}


