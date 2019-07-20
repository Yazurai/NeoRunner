using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextController : MonoBehaviour {
    public Text ValueText;
    public Slider ValueSlider;
    public int multiplier;

    public void SetText() {
        ValueText.text = (multiplier * ValueSlider.value).ToString();
    }
}
