using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDeathUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] m_HidingCanvas;
    [SerializeField] private CanvasGroup m_DisplayCanvas;
    [SerializeField] private CanvasGroup m_AfterDisplayCanvas;

    [SerializeField] private float m_BetweenHidingAndDisplayTime = 1;
    [SerializeField] private float m_BetweenDisplayAndAfterTime = 4;

    public void SetGameDeathEnd()
    {
        gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;

        foreach (CanvasGroup cg in m_HidingCanvas)
            StartCoroutine(TimingProcess(cg, 1.5f, false));
        StartCoroutine(TimingProcess(m_DisplayCanvas, 2.5f, true, m_BetweenHidingAndDisplayTime));
        StartCoroutine(TimingProcess(m_AfterDisplayCanvas, 2, true, m_BetweenDisplayAndAfterTime));
    }

    private IEnumerator TimingProcess(CanvasGroup fadeInCanvas, float time, bool isInOut, float waitTime = 0)
    {
        yield return new WaitForSecondsRealtime(waitTime);

        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.unscaledDeltaTime;
            if (isInOut) fadeInCanvas.alpha = Mathf.Lerp(0, 1, elapsedTime / time);
            else fadeInCanvas.alpha = Mathf.Lerp(1, 0, elapsedTime / time);
            yield return null;
        }
        fadeInCanvas.blocksRaycasts = isInOut;
    }

    public void OnClickedRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
