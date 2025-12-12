using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    // Método para cargar la escena inicial
    public void VolverAlInicio()
    {
        SceneManager.LoadScene(0);
    }
}
