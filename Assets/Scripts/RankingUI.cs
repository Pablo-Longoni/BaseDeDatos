using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RankingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rankingText;

    public static RankingUI Instance { get; private set; }

    [SerializeField] private TMP_Dropdown categoryDropdown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        categoryDropdown.onValueChanged.AddListener(CategoryChanged);

        // Llenar  Dropdown con las categorías disponibles
        categoryDropdown.ClearOptions();
        var categories = new List<string> { "Global", "Historia", "Ciencia", "Arte", "Geografia" };
        categoryDropdown.AddOptions(categories);

        // Mostrar ranking inicial
        DisplayGlobalRanking();
    }


    private void CategoryChanged(int index)
    {
        switch (index)
        {
            case 0:
                DisplayGlobalRanking();
                break;
            case 1:
                DisplayTriviaRanking(1); 
                break;
            case 2:
                DisplayTriviaRanking(2); 
                break;
            case 3:
                DisplayTriviaRanking(3); 
                break;
            case 4:
                DisplayTriviaRanking(4); 
                break;
            default:
                Debug.LogError("Categoría desconocida.");
                break;
        }
    }


    public async void DisplayGlobalRanking()
    {
        if (rankingText == null)
        {
            Debug.LogError("El campo rankingText no está asignado en el Inspector.");
            return;
        }

        var ranking = await GameManager.Instance.GetGlobalRanking();
        rankingText.text = "";

        foreach (var stat in ranking)
        {
            // Obtener el nombre del usuario usando el usuarios_id
            string username = await GameManager.Instance.GetUsernameById(stat.usuarios_id);
            rankingText.text += $"{username}: {stat.points.ToString("F3")}\n";
        }
    }

    public async void DisplayTriviaRanking(int triviaId)
    {
        var ranking = await GameManager.Instance.GetTriviaRanking(triviaId);
        rankingText.text = "";

        foreach (var stat in ranking)
        {
            // Obtener el nombre del usuario usando el usuarios_id
            string username = await GameManager.Instance.GetUsernameById(stat.usuarios_id);
            rankingText.text += $"{username}: {stat.points.ToString("F3")}\n";
        }
    }
} 