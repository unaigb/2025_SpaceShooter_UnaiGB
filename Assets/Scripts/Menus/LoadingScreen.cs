using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public CanvasGroup fadeCanvas;
    public Canvas startCanvas;
    public float fadeDuration = 1f;
    public float loadingTime = 1.5f;
    public AudioSource backgroundMusic;
    public TMP_Text loadingText;

    public GameObject jugadorPrefab;
    public Transform spawnJugador;
    public Manager manager;

    private bool isGameReady = false;

    void Start()
    {
        startCanvas.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(true);
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        fadeCanvas.alpha = 1; // Pantalla completamente opaca
        yield return new WaitForSeconds(loadingTime); // Tiempo de espera para cargar
        yield return StartCoroutine(Fade(0)); // Realiza el fade-out
        isGameReady = true; // El juego está listo para iniciar
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvas.alpha;
        loadingText.gameObject.SetActive(false); // Desactiva el texto de "loading"

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            yield return null;
        }

        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }

        fadeCanvas.alpha = targetAlpha;
        startCanvas.gameObject.SetActive(true);
    }

    void Update()
    {
        if (isGameReady)
        {
            bool ratonPulsado = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            bool teclaPulsada = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;

            if (ratonPulsado || teclaPulsada)
            {
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        startCanvas.gameObject.SetActive(false);

        manager.ActivarManager(); // Activa el script del Manager para generar enemigos, etc.

        Instantiate(jugadorPrefab, spawnJugador.position, Quaternion.identity); // Genera el jugador en la posición de spawn

        gameObject.SetActive(false); // Desactiva el objeto LoadingScreen (desaparece la pantalla de carga)
    }
}