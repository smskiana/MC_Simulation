using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextSetter : MonoBehaviour
{
    public TMP_Text text;
    public Slider slider;
    public void OnValueChange()
    {
        this.text.SetText($"{slider.value:F0}");
    }
}
