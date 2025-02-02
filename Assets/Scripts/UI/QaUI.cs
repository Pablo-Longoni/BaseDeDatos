using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QaUI : MonoBehaviour
{
    [SerializeField] public int questionsAnswered;
    [SerializeField] private TextMeshProUGUI questionsAnsweredText;
    [SerializeField] private TimerUI timer;
    void Start()
    {
        questionsAnswered = 0;
    }

    void Update()
    {
        UpdateQA();
    }

    public void IncreaseQA()
    {
        if (timer != null)
        {
            questionsAnswered++;
        }
        UpdateQA();
    }

    public void UpdateQA()
    {
        questionsAnsweredText.text = $"{questionsAnswered}" + "/10";
        PlayerPrefs.SetInt("QuestionsAnswered", questionsAnswered);
     //   Debug.Log("Preguntas: " + questionsAnswered);
        PlayerPrefs.Save();
    }

    public int GetQuestionsAnswered()
    {
        return questionsAnswered;
    }
}
