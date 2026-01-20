
using _3C.Actors.player;
using Effects;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public Player player;
    public Slider slider;
    public Slider progressBar;
    public GameObject panel;
    public TextMeshProUGUI text;
    public float speedMin = 0.2f;          // 随机速度最小值
    public float speedMax = 1.0f;          // 随机速度最大值
    [HideInInspector]
    public bool isDataLoaded = false;      // 外部数据加载完成后由外部设置
    private void Start()
    {
        StartCoroutine(ProgressRoutine());
    }
    private IEnumerator ProgressRoutine()
    {
        float displayProgress = 0;
        progressBar.value = 0;

        // ----------- 前 90% 的随机伪进度 ----------- 
        while (!isDataLoaded)
        {
            if (displayProgress < 0.9f)
            {
                float distanceTo90 = 0.9f - displayProgress;
                float randomSpeed = Random.Range(speedMin, speedMax);

                // 越靠近 90%，速度越慢
                displayProgress += randomSpeed * distanceTo90 * Time.deltaTime;
            }

            progressBar.value = displayProgress;
            yield return null;
        }

        // ----------- 数据加载完成后补到 100% ----------- 
        while (displayProgress < 0.999f)
        {
            displayProgress = Mathf.Lerp(displayProgress, 1f, Time.deltaTime * 2f);
            progressBar.value = displayProgress;
            yield return null;
        }

        progressBar.value = 1f;
        panel.SetActive(false);
        WorldManager.Instance.WorldCreateOver = true;
    }
    public void OnHpChange(float value)
    {
        slider.maxValue = value;
        text.text = $"({slider.value}/{value}";
    }
    public void OnCurHpChange(float value)
    {
        slider.value = value;
        text.text = $"({value}/{slider.maxValue}";
    }
 
    public void OnDownButtonDown()
    {
       
    }
    public void Regist(Player player)
    {    
    
        text.text = $"({slider.value}/{slider.maxValue})";
     
    }

}
