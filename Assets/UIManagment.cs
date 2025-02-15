using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;
public class UIManagment : MonoBehaviour
{


    [SerializeField] TextMeshProUGUI _categoryText;
    [SerializeField] TextMeshProUGUI _questionText;

    string _correctAnswer;

    public Button[] _buttons = new Button[3];

    [SerializeField] Button _backButton;

    private List<string> _answers = new List<string>();

    public bool queryCalled;

    private Color _originalButtonColor;

    public static UIManagment Instance { get; private set; }

    [SerializeField] GameObject StatPanel;
    private bool statsSaved = false;

    [SerializeField] PointsUI PointsUI;
    [SerializeField] private TimerUI timerUI;
    [SerializeField] QaUI QaUI;
    [SerializeField] private int points;
    [SerializeField] private int questionAnswered;
    [SerializeField] private TextMeshProUGUI messeage;
    [SerializeField] private TextMeshProUGUI pointsStat;
    [SerializeField] private TextMeshProUGUI answeredStat;

    [SerializeField] private Image questionImage;
    [SerializeField] private TextMeshProUGUI questionText;
    void Awake()
    {
        // Configura la instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        queryCalled = false;

        _originalButtonColor = _buttons[0].GetComponent<Image>().color;

        timerUI.ResetTimer();

        StatPanel.SetActive(false);
    }

    void Update()
    {
      /*  if (GameManager.Instance.randomQuestionIndex == -1)
        {
            //  PreviousScene();
            ShowStats();
            // Debug.Log("Stats guardadas en Update UImanagement");
        }
        else if (GameManager.Instance != null && GameManager.Instance.responseList != null && GameManager.Instance.responseList.Count > 0)
        {
            _categoryText.text = PlayerPrefs.GetString("SelectedTrivia");
            _questionText.text = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].QuestionText;

            GameManager.Instance.CategoryAndQuestionQuery(queryCalled);
        }
        else
        {
            // Debug.Log("Esperando a que los datos de la trivia se carguen");
        }*/


        if (GameManager.Instance.randomQuestionIndex == -1)
        {
            ShowStats();
        }
        else if (GameManager.Instance != null && GameManager.Instance.responseList != null && GameManager.Instance.responseList.Count > 0)
        {
            _categoryText.text = PlayerPrefs.GetString("SelectedTrivia");
            _questionText.text = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].QuestionText;
            GameManager.Instance.CategoryAndQuestionQuery(queryCalled);
        }
    }

    public void OnButtonClick(int buttonIndex)
    {
        string selectedAnswer = _buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text.Trim();

        // Obtén el índice de la respuesta correcta desde la base de datos, ajustado a 0 (basado en 1)
        int correctIndex = int.Parse(GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].CorrectOption) - 1;

        // Asigna la respuesta correcta desde la lista de respuestas
        _correctAnswer = GameManager.Instance._answers[correctIndex].Trim();

        Debug.Log($"SelectedAnswer: '{selectedAnswer}' | CorrectAnswer: '{_correctAnswer}'");

        // Compara las respuestas ignorando mayúsculas y minúsculas
        if (selectedAnswer == _correctAnswer)//string.Equals(selectedAnswer, _correctAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            timerUI.isTimerRunning = false;
            Debug.Log("¡Respuesta correcta!");
            ChangeButtonColor(buttonIndex, Color.green);
            Invoke("RestoreButtonColor", 2f);
            GameManager.Instance._answers.Clear();
            Invoke("NextQuestion", 2f);
            PointsUI.IncreasePoints();
            QaUI.IncreaseQA();
        }
        else
        {
            Debug.Log("Respuesta incorrecta. Inténtalo de nuevo." + _correctAnswer + selectedAnswer);

            ChangeButtonColor(buttonIndex, Color.red);
            Invoke("RestoreButtonColor", 2f);
            // PreviousScene();
            ShowStats();
        }
        /* string selectedAnswer = _buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;

         _correctAnswer = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].CorrectOption;

         if (selectedAnswer == _correctAnswer)
         {
             Debug.Log("¡Respuesta correcta!");
             ChangeButtonColor(buttonIndex, Color.green);
             Invoke("RestoreButtonColor", 2f);
             GameManager.Instance._answers.Clear();
             Invoke("NextAnswer", 2f);

         }
         else
         {
             Debug.Log("Respuesta incorrecta. Inténtalo de nuevo." + _correctAnswer + selectedAnswer);

             ChangeButtonColor(buttonIndex, Color.red);
             Invoke("RestoreButtonColor", 2f);
         }*/


    }

    private void ChangeButtonColor(int buttonIndex, Color color)
    {
        Image buttonImage = _buttons[buttonIndex].GetComponent<Image>();
        buttonImage.color = color;
    }

    private void RestoreButtonColor()
    {
        foreach (Button button in _buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = _originalButtonColor;
        }
    }

    private void NextQuestion()
    {
        queryCalled = false;
        timerUI.ResetTimer();
        questionImage.sprite = null;
        questionImage.gameObject.SetActive(false);
        questionText.rectTransform.anchoredPosition = new Vector2(28, 310);
    }

    public void OnTimerExpired()
    {
        Debug.Log("El tiempo para responder se agotooooooo.");
        // PreviousScene();
        ShowStats();
        Debug.Log("stats guardadas OnTimerExpired");
    }

    /* public async void PreviousScene()
     {
         Destroy(GameManager.Instance);
         Destroy(UIManagment.Instance);
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
         await   GameManager.Instance.SavePlayerStats(points, questionAnswered);
     }*/

    public async void ShowStats()
    {
        questionImage.gameObject.SetActive(false);
        if (statsSaved) return; // Evitar ejecución repetida
        statsSaved = true;

        timerUI.isTimerRunning = false;

        float currentPoints = PointsUI.GetPoints();
        int currentQuestionsAnswered = QaUI.GetQuestionsAnswered();

        Debug.Log("Current Points: " + currentPoints);
        Debug.Log("Current Questions Answered: " + currentQuestionsAnswered);

        pointsStat.text = currentPoints.ToString("F3");
        answeredStat.text = $"{currentQuestionsAnswered.ToString()}" + "/10";



        await Task.Delay(500);

        StatPanel.SetActive(true);


        if (QaUI.questionsAnswered == 10)
        {
            messeage.text = "GANASTE";
        }
        else
        {
            messeage.text = "PERDISTE";
        }

        await GameManager.Instance.SavePlayerStats(points, questionAnswered);

        // await Task.Delay(3000);
        Destroy(GameManager.Instance);
        Destroy(UIManagment.Instance);
    }

    public void ShowRankings()
    {
        // Si quieres mostrar el ranking global
        RankingUI.Instance.DisplayGlobalRanking();

        // O mostrar el ranking de una trivia específica (por ejemplo, con el ID 1)

        RankingUI.Instance.DisplayTriviaRanking(1);
    }

    public void LoadImageForCurrentQuestion()
    {
        if (GameManager.Instance.responseList != null && GameManager.Instance.responseList.Count > GameManager.Instance.randomQuestionIndex)
        {
            string imageUrl = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].Image_url;

            Debug.Log($"Intentando cargar imagen para la pregunta: {GameManager.Instance.randomQuestionIndex}, URL: {imageUrl}");

            if (!string.IsNullOrEmpty(imageUrl))
            {
                StartCoroutine(LoadQuestionImage(imageUrl));
                questionText.rectTransform.anchoredPosition = new Vector2(28, 31);
            }
            else
            {
                questionImage.gameObject.SetActive(false);
                Debug.Log("No hay imagen para esta pregunta.");
                questionImage.sprite = null;
            }
        }
        else
        {
            Debug.LogError("Error: El índice de la pregunta está fuera de rango o la lista de preguntas es nula.");
        }
    }

    private IEnumerator LoadQuestionImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            questionImage.gameObject.SetActive(false); // Asegurar que se oculta antes de actualizar
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            questionImage.gameObject.SetActive(true); // Se vuelve a activar
        }
        else
        {
            questionImage.gameObject.SetActive(false); // En caso de error, asegurarse de que la imagen no aparezca
        }
    }

}
