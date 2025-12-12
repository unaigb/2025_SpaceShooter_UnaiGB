using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class PlayerSpaceShip : MonoBehaviour
{
    [SerializeField] float velocidad = 5f;

    [Header("Configuración del Juego")]
    [SerializeField] GameObject prefabDisparoJugador;
    [SerializeField] float tiempoEntreDisparos = 0.05f;
    private float temporizadorDisparo = 0f;

    [Header("Input System")]
    [SerializeField] InputActionReference move;   // Referencia a 'Move' (Vector2)
    [SerializeField] InputActionReference shoot;  // Referencia a 'Fire' (Button)

    private UIController uiController;
    private AudioSource audioS;
    private Animator anim;
    private PlayerModel playerModel;
    private Manager manager;

    private bool estaVivo = true;
    private bool esInvulnerable = false;
    private Vector2 posicionInicial;

    public PlayerModel getJugador() => playerModel;

    private void OnEnable() { move.action.Enable(); shoot.action.Enable(); }
    private void OnDisable() { move.action.Disable(); shoot.action.Disable(); }

    void Start()
    {
        anim = GetComponent<Animator>();
        audioS = GetComponent<AudioSource>();
        posicionInicial = transform.position;

        playerModel = new PlayerModel();
        playerModel.pVelocidad = velocidad;

        uiController = FindFirstObjectByType<UIController>();
        manager = FindFirstObjectByType<Manager>();

        if (uiController != null)
        {
            uiController.ActualizarSalud(playerModel.pSalud, 100);
            uiController.ActualizarVidas(playerModel.pVidas);
            uiController.ActualizarPuntuacion(playerModel.pPuntuacion);
        }
    }

    void Update()
    {
        if (!estaVivo) return;

        Vector2 inputMovimiento = move.action.ReadValue<Vector2>();
        inputMovimiento.Normalize();

        temporizadorDisparo += Time.deltaTime;
        bool estaDisparando = shoot.action.IsPressed();

        if (estaDisparando && temporizadorDisparo >= tiempoEntreDisparos)
        {
            anim.SetBool("disparar", true);
            disparar();
            temporizadorDisparo = 0f;
        }
        else
            anim.SetBool("disparar", false);

        if (inputMovimiento != Vector2.zero)
        {
            mover(inputMovimiento);
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 cursorGlobal = Camera.main.ScreenToWorldPoint(mousePos);
        cursorGlobal.z = 0;

        Vector3 directionToCursor = (cursorGlobal - transform.position).normalized;
        float angulo = Mathf.Atan2(directionToCursor.y, directionToCursor.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo - 90);
    }

    void mover(Vector2 vector2)
    {
        float posX = transform.position.x + (vector2.x * velocidad * Time.deltaTime);
        float posY = transform.position.y + (vector2.y * velocidad * Time.deltaTime);

        if (manager != null)
        {
            float margen = 0.5f;
            posX = Mathf.Clamp(posX, manager.minX + margen, manager.maxX - margen);
            posY = Mathf.Clamp(posY, manager.minY + margen, manager.maxY - margen);
        }
        else
        {
            Vector2 min = Camera.main.ViewportToWorldPoint(Vector2.zero);
            Vector2 max = Camera.main.ViewportToWorldPoint(Vector2.one);
            posX = Mathf.Clamp(posX, min.x, max.x);
            posY = Mathf.Clamp(posY, min.y, max.y);
        }

        transform.position = new Vector3(posX, posY, 0);
    }

    void disparar()
    {
        if (transform.childCount >= 2)
        {
            Instantiate(prefabDisparoJugador, transform.GetChild(0).position, transform.rotation);
            Instantiate(prefabDisparoJugador, transform.GetChild(1).position, transform.rotation);
        }
        if (audioS && !audioS.isPlaying) audioS.Play();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (esInvulnerable || !estaVivo) return;

        if (other.gameObject.name.StartsWith("EnemyShot"))
        {
            playerModel.pSalud -= 10;
            uiController.ActualizarSalud(playerModel.pSalud, 100);
            Destroy(other.gameObject);
            VerificarMuerte();
        }
        else if (other.CompareTag("Enemy"))
        {
            playerModel.pSalud -= 25;
            uiController.ActualizarSalud(playerModel.pSalud, 100);
            Destroy(other.gameObject);
            VerificarMuerte();
        }
        else if (other.CompareTag("Heal"))
        {
            playerModel.pSalud = Mathf.Min(playerModel.pSalud + 50, 100);
            uiController.ActualizarSalud(playerModel.pSalud, 100);
            Destroy(other.gameObject);
        }
    }

    void VerificarMuerte()
    {
        if (playerModel.pSalud <= 0)
        {
            playerModel.pVidas--;
            uiController.ActualizarVidas(playerModel.pVidas);

            if (playerModel.pVidas > 0)
                StartCoroutine(Respawn());
            else
                MuerteDefinitiva();
        }
    }

    IEnumerator Respawn()
    {
        estaVivo = false;

        anim.SetBool("explotar", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("explotar", false);

        transform.position = posicionInicial;
        playerModel.pSalud = 100;
        uiController.ActualizarSalud(100, 100);

        estaVivo = true;
        esInvulnerable = true;

        float tiempoParpadeo = 1.5f;
        float intervalo = 0.05f;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        while (tiempoParpadeo > 0)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(intervalo);
            tiempoParpadeo -= intervalo;
        }
        
        spriteRenderer.enabled = true;
        esInvulnerable = false;
    }

    void MuerteDefinitiva()
    {
        estaVivo = false;
        anim.SetBool("explotar", true);
        if(manager != null)
        {
            manager.jugadorVivo = false;
            manager.GameOver();
        }
        float duracionAnimacion = anim.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, duracionAnimacion);
    }
}