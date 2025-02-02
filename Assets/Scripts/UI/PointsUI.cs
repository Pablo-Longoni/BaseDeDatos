using Supabase.Gotrue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float points ;
 //   [SerializeField] private float questionsAnswered;
    [SerializeField] private TextMeshProUGUI pointsText;
    // [SerializeField] private TextMeshProUGUI questionsAnsweredText;
    [SerializeField] private TimerUI timer;
    void Start()
    {
        points = 0;
      //  questionsAnswered = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePointsUI();
    }

    public void IncreasePoints()
    {
        if (timer != null)
        {
            float timeRemaining = timer.GetCurrentTime();
            points += /*Mathf.Max(0, 10 -*/ timeRemaining;
          //  questionsAnswered++;
        }
        UpdatePointsUI();
    }
    private void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            int roundedPoints = Mathf.RoundToInt(points); 
            pointsText.text = $"{roundedPoints}";
            //   questionsAnsweredText.text = $"{questionsAnswered}"; 

            //   Debug.Log("Preguntas: " + questionsAnswered);
            PlayerPrefs.SetFloat("Points", points);
          //  PlayerPrefs.SetFloat("QuestionsAnswered", questionsAnswered);
         //   Debug.Log("Puntaje: " + points);
            PlayerPrefs.Save();
        }
    }

    public float GetPoints()
    {
        return points;
    }
}
