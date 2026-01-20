

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bags.UI
{
    public class SplitUI :MonoBehaviour
    {
        public Slider slider;
        public TextMeshProUGUI textMesh;
        public BagSystemUI UI;
        public int Value => (int)slider.value;
        private void Awake()
        {
            slider.onValueChanged.AddListener(OnsliderValueChanged);
            gameObject.SetActive(false);
        }
        public void SetSliderValue(float value,float maxValue)
        {
            slider.value = value;
            slider.maxValue = maxValue;
        }
        public void OnsliderValueChanged(float value)
        {
            int val = (int) value;
            textMesh.text = val.ToString();
            slider.value = val;
        }
        public void Comfirm()
        {
            UI.OnSplitDown();
            gameObject.SetActive(false);
        }
        public void Cancel() => gameObject.SetActive(false);
        public void Open(float value, float maxValue)
        {
            SetSliderValue(value,maxValue);
            gameObject.SetActive(true);
        }
    
    }
}
