using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SupabaseManager : MonoBehaviour

{

    [Header("Campos de Interfaz")]
    [SerializeField] TMP_InputField _userIDInput;
    [SerializeField] TMP_InputField _userPassInput;
    [SerializeField] TextMeshProUGUI _stateText;

    string supabaseUrl = "https://rkzngebjesgwjwywjbxc.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InJrem5nZWJqZXNnd2p3eXdqYnhjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Mzc0MTkwNzEsImV4cCI6MjA1Mjk5NTA3MX0.MHQdqXGZsd9XG_1mTySm8O7C1qhXoDrRIEAnCK8BXSw";

    Supabase.Client clientSupabase;

    private usuarios _usuarios = new usuarios();


    public async void UserLogin()
    {
        // Initialize the Supabase client
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        if (string.IsNullOrEmpty(_userIDInput.text) || string.IsNullOrEmpty(_userPassInput.text))
        {
            _stateText.text = "Complete usuario y contraseña.";
            _stateText.color = new Color(0.9f, 0.3f, 0.3f);
            return;
        }
        // prueba
        var test_response = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Get();
        Debug.Log(test_response.Content);



        // filtro según datos de login
        var login_password = await clientSupabase
          .From<usuarios>()
          .Select("password")
          .Where(usuarios => usuarios.username == _userIDInput.text)
          .Get();


        if (login_password.Model.password.Equals(_userPassInput.text))
        {
            print("LOGIN SUCCESSFUL");
            _stateText.text = "Login exitoso";
            _stateText.color = new Color(0.3f, 0.9f, 0.3f);
            SaveUserIdToPlayerPrefs();
            await Task.Delay(400);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            print("WRONG PASSWORD");
            _stateText.text = "Contraseña incorrecta";
            _stateText.color = new Color(0.9f, 0.3f, 0.3f);
        }
    }

    public async void InsertarNuevoUsuario()
    {

        // Initialize the Supabase client
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        // Verficar si los campos estan sin completar
        if (string.IsNullOrEmpty(_userIDInput.text) || string.IsNullOrEmpty(_userPassInput.text))
        {
            _stateText.text = "Complete usuario y contraseña.";
            _stateText.color = new Color(0.9f, 0.3f, 0.3f);
            return; 
        }

        // Verificar si el usuario ya existe
        var consultaUsuario = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Where(u => u.username == _userIDInput.text)
            .Get();

        if (consultaUsuario.Models != null && consultaUsuario.Models.Count > 0)
        {
            _stateText.text = "El nombre de usuario ya está en uso";
            _stateText.color = new Color(0.9f, 0.3f, 0.3f);
            return; // Se evita la inserción
        }


        // Debug.Log(clientSupabase == null ? "Client no inicializado" : "Client inicializado correctamente");
        // Consultar el último id utilizado (ID = index)
        var ultimoId = await clientSupabase
            .From<usuarios>()
            .Select("id")
            .Order(usuarios => usuarios.id, Postgrest.Constants.Ordering.Descending) // Ordenar en orden descendente para obtener el último id
            .Get();

        int nuevoId = 1; // Valor predeterminado si la tabla está vacía

        if (ultimoId.Models != null && ultimoId.Models.Count > 0)
        {
            nuevoId = ultimoId.Models[0].id + 1; ; // Incrementar el último id
        }

        // Crear el nuevo usuario con el nuevo id
        var nuevoUsuario = new usuarios
        {

            id = nuevoId,
            username = _userIDInput.text,
            age = Random.Range(0, 100), //luego creo el campo que falta en la UI
            password = _userPassInput.text,
        };


        // Insertar el nuevo usuario
        var resultado = await clientSupabase
            .From<usuarios>()
            .Insert(new[] { nuevoUsuario });


        //verifico el estado de la inserción 
        if (resultado.ResponseMessage.IsSuccessStatusCode)
        {
            _stateText.text = "Usuario Correctamente Ingresado";
            _stateText.color = new Color(0.3f, 0.9f, 0.3f);
            SaveUserIdToPlayerPrefs();
            await Task.Delay(400);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            _stateText.text = "Error en el registro de usuario";
            _stateText.text = resultado.ResponseMessage.ToString();
            _stateText.color = new Color(0.3f, 0.9f, 0.3f);
        }
    }

      async void SaveUserIdToPlayerPrefs()
    {
        var userResponse = await clientSupabase
         .From<usuarios>()
         .Select("id")
         .Where(usuarios => usuarios.username == _userIDInput.text)
         .Get();

        if (userResponse.Models.Count > 0)
        {
            string userId = userResponse.Models[0].id.ToString();
            PlayerPrefs.SetString("UserId", userId);
            Debug.Log("Usuario ID guardado: " + userId);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("No se pudo encontrar el ID del usuario.");
        }
    }


}

