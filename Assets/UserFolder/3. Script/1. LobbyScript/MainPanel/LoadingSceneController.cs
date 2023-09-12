using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    private static string m_NextScene;
    [SerializeField] private Image m_ProgressBarImage;
    [SerializeField] private float m_LastWaitTime = 1;

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
        AsyncOperation operation = SceneManager.LoadSceneAsync(m_NextScene);
        operation.allowSceneActivation = false;

        float timer = 0;
        while (!operation.isDone)
        {
            yield return null;

            if(operation.progress < 0.9f) m_ProgressBarImage.fillAmount = operation.progress;
            else
            {
                timer += Time.unscaledDeltaTime;
                m_ProgressBarImage.fillAmount = Mathf.Lerp(0.9f, 1, timer);
                if(m_ProgressBarImage.fillAmount >= m_LastWaitTime)
                {
                    operation.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
