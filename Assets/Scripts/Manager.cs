using UnityEngine;
using System.Collections;
using TMPro;

public class Manager : MonoBehaviour
{
    [Header("Configuración de Spawning")]
    public GameObject gameObjectEnemigo;
    public GameObject itemCuraPrefab;

    public EdgeCollider2D limiteNivel;

    [Header("Dificultad y Niveles")]
    public float tiempoEntreApariciones = 2f;
    public int maxEnemigosInicial = 10;
    public int aumentoEnemigosPorNivel = 5;

    [Header("Seguridad")]
    public float radioSeguridad = 3f;

    private int nivelActual = 1;
    private int maxEnemigosActual;
    private int enemigosGenerados = 0;
    private int enemigosVivos = 0;

    [Header("Estado del Juego")]
    private int puntosPrevios = 0;
    private const int puntosPorCura = 500;
    private bool managerActivo = false;
    public bool jugadorVivo = true;

    [Header("Referencias UI y Jugador")]
    public PlayerSpaceShip playerScript;
    public UIController uiController;

    public GameObject levelUpCanvas;
    public GameObject gameOverCanvas;
    public GameObject interfazJuego;
    public TMP_Text textoPuntuacion;
    public TMP_Text textoPuntuacionGameOver;

    [HideInInspector] public float minX, maxX, minY, maxY;

    void Awake()
    {
        CalcularAreaDesdeCollider();
    }

    void Start()
    {
        if (gameOverCanvas) gameOverCanvas.SetActive(false);
        if (levelUpCanvas) levelUpCanvas.SetActive(false);
        if (interfazJuego) interfazJuego.SetActive(true);

        if (playerScript == null) playerScript = FindFirstObjectByType<PlayerSpaceShip>();
        if (uiController == null) uiController = FindFirstObjectByType<UIController>();

        CalcularAreaDesdeCollider();

        maxEnemigosActual = maxEnemigosInicial;
        nivelActual = 1;

        if (uiController != null) uiController.ActualizarNivel(nivelActual);

        managerActivo = false;
    }

    void CalcularAreaDesdeCollider()
    {
        if (limiteNivel == null) return;

        Bounds limites = limiteNivel.bounds;
        minX = limites.min.x;
        maxX = limites.max.x;
        minY = limites.min.y;
        maxY = limites.max.y;
    }

    public void ActivarManager()
    {
        managerActivo = true;
        enemigosGenerados = 0;
        enemigosVivos = 0;
        StartCoroutine(GenerarEnemigos());
    }

    private IEnumerator GenerarEnemigos()
    {
        while (enemigosGenerados < maxEnemigosActual)
        {
            if (!managerActivo || !jugadorVivo) yield break;
            Vector2 posicionRandom;
            int intentos = 0;
            bool posicionValida = false;

            do
            {
                posicionRandom = new Vector2(
                    Random.Range(minX, maxX),
                    Random.Range(minY, maxY)
                );
                if (playerScript != null)
                {
                    float distanciaAlJugador = Vector2.Distance(posicionRandom, playerScript.transform.position);
                    if (distanciaAlJugador >= radioSeguridad)
                    {
                        posicionValida = true;
                    }
                }
                else
                {
                    posicionValida = true;
                }

                intentos++;
            } while (!posicionValida && intentos < 10);

            Instantiate(gameObjectEnemigo, posicionRandom, Quaternion.identity);

            enemigosGenerados++;
            enemigosVivos++;

            yield return new WaitForSeconds(tiempoEntreApariciones);
        }
    }

    public void EnemigoEliminado()
    {
        enemigosVivos--;
        if (enemigosVivos <= 0 && enemigosGenerados >= maxEnemigosActual && managerActivo)
        {
            StartCoroutine(SecuenciaSubirNivel());
        }
    }

    private IEnumerator SecuenciaSubirNivel()
    {
        managerActivo = false;
        if (levelUpCanvas) levelUpCanvas.SetActive(true);
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        if (levelUpCanvas) levelUpCanvas.SetActive(false);

        nivelActual++;
        maxEnemigosActual += aumentoEnemigosPorNivel;
        if(tiempoEntreApariciones >= 0.5f)
        {
            tiempoEntreApariciones = Mathf.Max(0.5f, tiempoEntreApariciones - 0.15f);
        }
        else
        {
            tiempoEntreApariciones = 0.4f;
        }

        if (uiController != null) uiController.ActualizarNivel(nivelActual);
        ActivarManager();
    }

    public void GameOver()
    {
        managerActivo = false;
        Time.timeScale = 1f;

        if (interfazJuego) interfazJuego.SetActive(false);
        if (gameOverCanvas) gameOverCanvas.SetActive(true);

        if (!jugadorVivo && textoPuntuacionGameOver != null)
        {
            textoPuntuacionGameOver.text = textoPuntuacion.text;
        }
    }

    public void VerificarPuntos(int puntosJugador)
    {
        if (puntosJugador - puntosPrevios >= puntosPorCura)
        {
            puntosPrevios = puntosJugador;
            GenerarCura();
        }
    }

    private void GenerarCura()
    {
        if (itemCuraPrefab == null || limiteNivel == null) return;

        // Usamos un margen de 1 unidad para que no aparezca medio "metida" en la pared
        float margen = 1f;
        float xAleatoria = Random.Range(minX + margen, maxX - margen);
        float yAleatoria = Random.Range(minY + margen, maxY - margen);

        Vector3 posicionCura = new Vector3(xAleatoria, yAleatoria, 0);

        Instantiate(itemCuraPrefab, posicionCura, Quaternion.identity);

        Debug.Log("¡Cura generada en " + posicionCura + "!");
    }
}