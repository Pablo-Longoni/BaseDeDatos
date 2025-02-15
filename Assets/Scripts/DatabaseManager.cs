using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Models;

public class DatabaseManager : MonoBehaviour
{
    string supabaseUrl = "https://rkzngebjesgwjwywjbxc.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InJrem5nZWJqZXNnd2p3eXdqYnhjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Mzc0MTkwNzEsImV4cCI6MjA1Mjk5NTA3MX0.MHQdqXGZsd9XG_1mTySm8O7C1qhXoDrRIEAnCK8BXSw"; //COMPLETAR

    Supabase.Client clientSupabase;

    public int index;

    //UI
    [SerializeField] private 


    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);
        
        index = PlayerPrefs.GetInt("SelectedIndex");

        //print(_selectedTrivia);

        await LoadTriviaData(index);
    }

    async Task LoadTriviaData(int index)
    {
        var response = await clientSupabase
            .From<question>()
            .Where(question => question.trivia_id == index)
            .Select("id, question, answer1, answer2, answer3, correct_answer, image_url, trivia_id, trivia(id, category)")
            .Get();

        GameManager.Instance.currentTriviaIndex = index;

        GameManager.Instance.responseList = response.Models;

        print("Response from query: "+ response.Models.Count);
        print("ResponseList from GM: "+ GameManager.Instance.responseList.Count);

       /* foreach (var question in response.Models)
        {
            Debug.Log($"Pregunta: {question.QuestionText}");
            Debug.Log($"Respuestas: {question.Answer1}, {question.Answer2}, {question.Answer3}");
        }*/


    }

}
