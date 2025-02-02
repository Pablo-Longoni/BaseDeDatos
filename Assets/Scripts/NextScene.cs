using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void  RankingScene()
    {
        Debug.Log("Cambiando a la escena Rankings");
        SceneManager.LoadScene("Rankings");
    }

    public void BackScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        Debug.Log("Cambiando de escena ");
    }

    public void TriviaScene()
    {
        Debug.Log("Cambiando a la escena Trivia");
        SceneManager.LoadScene("TriviaSelectScene");
    }
}
