using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    private static string m_NextScene = "";
    [SerializeField] private Slider m_LoadingSlider;
    [SerializeField] private float m_FirstWaitTime = 2;     //3
    [SerializeField] private float m_LastWaitTime = 5;      //4

    public static void LoadScene(string sceneName)
    {
        m_NextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        yield return new WaitForSecondsRealtime(m_FirstWaitTime);
        if (m_NextScene == "")
        {
            Debug.LogWarning("Scene name is null");
            yield break;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(m_NextScene);
        operation.allowSceneActivation = false;

        float timer = 0;
        while (!operation.isDone)
        {
            yield return null;

            if(operation.progress < 0.9f) m_LoadingSlider.value = operation.progress * 100;
            else
            {
                timer += Time.unscaledDeltaTime;
                m_LoadingSlider.value = Mathf.Lerp(90f, 100f, timer / m_LastWaitTime);
                if(timer >= m_LastWaitTime)
                {
                    operation.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
