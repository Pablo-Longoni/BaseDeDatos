using Supabase.Gotrue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsUI : MonoBehaviour
{
    [SerializeField] private float points ;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TimerUI timer;
    void Start()
    {
        points = 0;
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
        }
        UpdatePointsUI();
    }
    private void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            int roundedPoints = Mathf.RoundToInt(points); 
            pointsText.text = $"{roundedPoints}";
            PlayerPrefs.SetFloat("Points", points);
            PlayerPrefs.Save();
        }
    }

    public float GetPoints()
    {
        return points;
    }
}
