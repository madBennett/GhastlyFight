using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void setValue(float val)
    {
        //
        slider.value = val;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void setMaxValue(float val)
    {
        //
        slider.maxValue = val;
        slider.value = val;

        fill.color = gradient.Evaluate(1f);
    }
}
