//using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TimerUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float timer;
  //  [SerializeField] private TextMeshProUGUI timerText;
    public bool isTimerRunning;
    [SerializeField] private RectTransform timerBar;
    private float maxTime = 10f;
    private float initialWidth;

    private Coroutine blinkingCoroutine;
    [SerializeField] private Image TimerBar;
    void Start()
    {
        if (timerBar == null)
        {
            Debug.LogError("TimerBar no está asignado en el Inspector.");
            return;
        }
        // Asegurar que el Pivot está centrado en el Inspector (0.5, 0.5)
        timerBar.pivot = new Vector2(0.5f, 0.5f);
        initialWidth = timerBar.sizeDelta.x;
        if (initialWidth <= 0)
        {
            initialWidth = 1000f; // Valor por defecto si no tiene ancho en el Inspector
            timerBar.sizeDelta = new Vector2(initialWidth, timerBar.sizeDelta.y);
        }
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                isTimerRunning = false;
                TimerEnded();
            }

            UpdateTimerUI();
        }

        if (timer <= 5 && timer > 0 && blinkingCoroutine == null)
        {
            blinkingCoroutine = StartCoroutine(BlinkerBar());
        }
        else if ((timer > 5 || !isTimerRunning) && blinkingCoroutine != null)
        {
            StopCoroutine(blinkingCoroutine);
            blinkingCoroutine = null;
            // Restablecer el alfa a 1 para asegurarse de que la barra quede opaca
            if (TimerBar != null)
            {
                Color currentColor = TimerBar.color;
                TimerBar.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
            }
        }
    }

    public void ResetTimer()
    {
        timer = maxTime;
        timer = Mathf.Max(0, timer);
        isTimerRunning = true;
        UpdateTimerUI();
        initialWidth = 1000f;
        if (blinkingCoroutine != null)
        {
            StopCoroutine(blinkingCoroutine);
            blinkingCoroutine = null;
        }
        // Restablecer el alfa a 1
        if (TimerBar != null)
        {
            Color currentColor = TimerBar.color;
            TimerBar.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        }
    }

    public void TimerEnded()
    {
        Debug.Log("Tiempo agotado. Funcion TimerEnded");
        UIManagment.Instance.OnTimerExpired();
    }

    public float GetCurrentTime()
    {
        return timer;
    }

    private void UpdateTimerUI()
    {
        UpdateBar();
    }


    //Barra timer
    private void UpdateBar()
    {
        float normalizedTime = Mathf.Clamp(timer / maxTime, 0f, 1f);
        float newWidth = initialWidth * normalizedTime;

        if (newWidth <= 0)
        {
            timerBar.gameObject.SetActive(false); 
        }
        else
        {
            timerBar.gameObject.SetActive(true); 
            timerBar.sizeDelta = new Vector2(newWidth, timerBar.sizeDelta.y);
        }
    }

    private IEnumerator BlinkerBar()
    {
        float maxAlpha = 1f;
        float minAlpha = 0.3f;
        float duration = 0.5f; // Duración de cada fade

        //barra parpadee cuando el tiempo es menor a 5
        while (isTimerRunning && timer <= 5 && timer > 0)
        {
            // Fade out 
            yield return StartCoroutine(FadeAlpha(maxAlpha, minAlpha, duration));
            // Fade in 
            yield return StartCoroutine(FadeAlpha(minAlpha, maxAlpha, duration));
        }
    }

    //Controlar transicion de transparencia
    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color currentColor = TimerBar.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(from, to, elapsed / duration);
            TimerBar.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }
        
        TimerBar.color = new Color(currentColor.r, currentColor.g, currentColor.b, to);
    }

}
