using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Begin : MonoBehaviour
{   
    public void LoadScene()
    {
        StartCoroutine(Load());
    }
    float time = 0; 
    public IEnumerator Load()
    {
        Debug.Log("开始加载");
        AsyncOperation asload = SceneManager.LoadSceneAsync(1);
        asload.allowSceneActivation = false;
        while (!asload.isDone)
        {
            float progress = asload.progress / 0.9f;
            Debug.Log("进度："+progress);
            time += Time.deltaTime;          
            if (asload.progress >= .9f)
            {
                Debug.Log("用时：" + time);
                asload.allowSceneActivation = true;
                break;
            }
            yield return null;
        }      
    }
}
