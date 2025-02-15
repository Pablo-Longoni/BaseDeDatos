using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using Supabase.Gotrue;

public class GameManager : MonoBehaviour
{
    //public TriviaManager triviaManager;

    public List<question> responseList; //lista donde guardo la respuesta de la query hecha en la pantalla de selección de categoría

    public int currentTriviaIndex = 0;

    public int randomQuestionIndex = 0;

    public List<string> _answers = new List<string>();

    public bool queryCalled;

    private int _points;

    // private int _maxAttempts = 10;

    public int _numQuestionAnswered = 0;

    string _correctAnswer;


    public HashSet<int> answeredQuestions = new HashSet<int>();
    public static GameManager Instance { get; private set; }

    Supabase.Client clientSupabase;
    string supabaseUrl = "https://rkzngebjesgwjwywjbxc.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InJrem5nZWJqZXNnd2p3eXdqYnhjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Mzc0MTkwNzEsImV4cCI6MjA1Mjk5NTA3MX0.MHQdqXGZsd9XG_1mTySm8O7C1qhXoDrRIEAnCK8BXSw";
    void Awake()
    {
        // Configura la instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializar clientSupabase aquí
            clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);
        }
        else
        {
            Destroy(gameObject);
        }

    }


    void Start()
    {

        StartTrivia();

        queryCalled = false;

    }

    void StartTrivia()
    {
        // Cargar la trivia desde la base de datos
        //triviaManager.LoadTrivia(currentTriviaIndex);

        //print(responseList.Count);

    }

    /* public void CategoryAndQuestionQuery(bool isCalled)
     {
         isCalled = UIManagment.Instance.queryCalled;

         if (!isCalled)
         {
             // Encuentra un índice no respondido
             randomQuestionIndex = GetNextQuestionIndex();

             // Si no quedan preguntas, maneja el fin de la trivia
             if (randomQuestionIndex == -1)
             {
                 Debug.Log("¡No quedan más preguntas por responder!");
                 // Lógica para terminar la trivia (mostrar resultados, etc.)

             }

             //_questionText.text = GameManager.Instance.responseList[randomQuestionIndex].QuestionText;
             _correctAnswer = GameManager.Instance.responseList[randomQuestionIndex].CorrectOption;

             //agrego a la lista de answers las 3 answers

             _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer1);
             _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer2);
             _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer3);

             // la mixeo con el método Shuffle (ver script Shuffle List)

             // _answers.Shuffle();

             // asigno estos elementos a los textos de los botones

             for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
             {
                 UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];

                 int index = i; // Captura el valor actual de i en una variable local -- SINO NO FUNCA!

                 UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
             }

             // Marca la pregunta como respondida
             answeredQuestions.Add(randomQuestionIndex);

             UIManagment.Instance.queryCalled = true;
         }

     }*/

    public void CategoryAndQuestionQuery(bool isCalled)
    {
      //  Debug.Log("CategoryAndQuestionQuery ha sido llamada.");
        isCalled = UIManagment.Instance.queryCalled;
        if (!isCalled)
        {
            randomQuestionIndex = GetNextQuestionIndex();

            if (randomQuestionIndex == -1)
            {
                Debug.Log("¡No quedan más preguntas por responder!");
                return;
            }

            _correctAnswer = GameManager.Instance.responseList[randomQuestionIndex].CorrectOption;

            _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer1);
            _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer2);
            _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer3);

            for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
            {
                UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];

                int index = i;
                UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
            }

            answeredQuestions.Add(randomQuestionIndex);
            UIManagment.Instance.queryCalled = true;

            UIManagment.Instance.LoadImageForCurrentQuestion();
        }
    }


    private int GetNextQuestionIndex()
    {
        // Genera una lista de índices no respondidos
        var unansweredIndices = Enumerable.Range(0, responseList.Count)
                                          .Where(index => !answeredQuestions.Contains(index))
                                          .ToList();

        // Si no quedan índices, devuelve -1
        if (unansweredIndices.Count == 0)
        {
            return -1; // Fin de la trivia
        }

        // Selecciona un índice aleatorio de los no respondidos
      //  int randomIndex = Random.Range(0, unansweredIndices.Count);
        int randomIndex = UnityEngine.Random.Range(0, unansweredIndices.Count);
        return unansweredIndices[randomIndex];
    }
    private void Update()
    {

    }

    // ESTADISTICA Y RANKING
    public async Task SavePlayerStats(float points, int questionsAnswered)
    {
        if (clientSupabase == null)
        {
            clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);
        }

        // Recuperar el ID del usuario desde PlayerPrefs
        string userId = PlayerPrefs.GetString("UserId", null);
        points = PlayerPrefs.GetFloat("Points");
        questionsAnswered = PlayerPrefs.GetInt("QuestionsAnswered");
        Debug.Log("Puntaje GM: " + points);
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No se puede guardar las estadísticas porque el ID del usuario no está disponible.");
            return;
        }

        var stat = new Stat
        {
            usuarios_id = userId,
            trivia_id = currentTriviaIndex,
            points = points,
            questions_answered = questionsAnswered
        };

        var response = await clientSupabase.From<Stat>().Insert(new[] { stat });

        if (response.ResponseMessage.IsSuccessStatusCode)
        {
            Debug.Log("Estadísticas guardadas correctamente.");
        }
        else
        {
            Debug.LogError("Error al guardar las estadísticas: " + response.ResponseMessage);
            string responseContent = await response.ResponseMessage.Content.ReadAsStringAsync();
            Debug.LogError("Respuesta del servidor: " + responseContent);
        }
    }

     public async Task<List<Stat>> GetGlobalRanking()
     {
         var response = await clientSupabase
             .From<Stat>()
             .Select("*")
             .Order(stat => stat.points, Postgrest.Constants.Ordering.Descending)
             .Limit(10) // Obtener los 10 mejores
             .Get();

         return response.Models;
     }


    public async Task<List<Stat>> GetTriviaRanking(int triviaId)
    {
        var response = await clientSupabase
            .From<Stat>()
            .Select("*")
            .Where(stat => stat.trivia_id == triviaId)
            .Order(stat => stat.points, Postgrest.Constants.Ordering.Descending)
            .Limit(10)
            .Get();

        return response.Models;
    }

    public async Task<string> GetUsernameById(string userId)
    {
        int user_Id = int.Parse(userId);
        var response = await clientSupabase
            .From<usuarios>()  // Asumiendo que tienes una clase Usuario que representa la tabla "usuarios"
            .Select("username")
            .Where(u => u.id == user_Id)  // Asumiendo que "id" es la columna primaria
            .Single(); // Esto obtiene solo un resultado, si el usuario existe


        if (response != null)
        {
            return response.username;  // Devuelve el nombre de usuario
        }
        else
        {
            Debug.LogError("No se pudo encontrar el nombre del usuario.");
            return "Desconocido";  // En caso de error, devuelve un nombre por defecto
        }
    }
} 