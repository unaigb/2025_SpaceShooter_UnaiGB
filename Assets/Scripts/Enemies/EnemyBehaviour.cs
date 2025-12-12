using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class EnemyBehaviour : MonoBehaviour
{
    public float velocidad = 5f;
    public float tiempoEntreDisparos = 1.5f;
    private float tiempoDisparo;
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRender;
    private EnemyModel enemyModel;
    public GameObject DisaroEnemigo;
    protected Animator anim;

    public int enemigosMuertos = 0;

    private Transform jugador;
    private Vector2 destino;
    public float rangoMovimientoX = 4f;
    public float rangoMovimientoY = 4f;

    private bool estaVivo = true;
    private bool haMuerto = false;

    private UIController uiController;

    // Inicialización
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        enemyModel = new EnemyModel();
        anim = GetComponent<Animator>();
        tiempoDisparo = Time.time + Random.Range(0.5f, tiempoEntreDisparos);

        uiController = FindFirstObjectByType<UIController>();

        GameObject jugadorObjeto = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObjeto != null)
        {
            jugador = jugadorObjeto.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró ningún objeto con el tag 'Player'");
        }

        CalcularDestinoAleatorio();
    }

    void Update()
    {
        if (!estaVivo) return;

        MoverHaciaDestino();

        MirarAlJugador();

        Vector2 posicionEnViewport = Camera.main.WorldToViewportPoint(transform.position);
        if (posicionEnViewport.x > 0 && posicionEnViewport.x < 1 && posicionEnViewport.y > 0 && posicionEnViewport.y < 1)
        {
            if (Time.time > tiempoDisparo)
            {
                StartCoroutine(DispararConAnimacion());
                tiempoDisparo = Time.time + tiempoEntreDisparos;
            }
        }

        if (Vector2.Distance(transform.position, destino) < 0.5f)
        {
            CalcularDestinoAleatorio();
        }
    }

    IEnumerator DispararConAnimacion()
    {
        if (!estaVivo) yield break;

        anim.SetBool("disparoEnemigo", true);

        float duracionAnimacion = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duracionAnimacion);
        anim.SetBool("disparoEnemigo", false);

        Disparar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (haMuerto) return;

        if (other.gameObject.name.StartsWith("PlayerShot"))
        {
            enemyModel.pVida -= 5;
            Destroy(other.gameObject);
        }

        if (enemyModel.pVida <= 0 || other.gameObject.name.Equals("ColliderExtra"))
        {
            Manager manager = FindFirstObjectByType<Manager>();
            haMuerto = true;

            estaVivo = false;
            rigid.linearVelocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Kinematic;

            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;

            anim.SetBool("Explotar", true);

            if (jugador != null)
            {
                PlayerSpaceShip jugadorScript = jugador.GetComponent<PlayerSpaceShip>();
                jugadorScript.getJugador().pPuntuacion += 100;
                uiController.ActualizarPuntuacion(jugadorScript.getJugador().pPuntuacion);

                manager.VerificarPuntos(jugadorScript.getJugador().pPuntuacion);
            }

            float duracionAnimacion = anim.GetCurrentAnimatorStateInfo(0).length;
            manager.EnemigoEliminado();
            Destroy(gameObject, duracionAnimacion);
        }
    }

    void Disparar()
    {
        if (!estaVivo || jugador == null) return;

        GameObject bala = Instantiate(DisaroEnemigo, transform.position, Quaternion.identity);
        Vector2 direccionHaciaJugador = (jugador.position - transform.position).normalized;
        bala.GetComponent<EnemyShot>().InicializarDireccion(direccionHaciaJugador);
    }

    void CalcularDestinoAleatorio()
    {
        if (!estaVivo) return;

        Manager manager = FindFirstObjectByType<Manager>();

        if (manager != null)
        {
            float margen = 0.7f;
            float randomX = Random.Range(manager.minX + margen, manager.maxX - margen);
            float randomY = Random.Range(manager.minY + margen, manager.maxY - margen);
            destino = new Vector2(randomX, randomY);
        }
        else
        {
            destino = transform.position;
            Debug.LogWarning("Enemigo no encontró el Manager para calcular límites.");
        }
    }

    void MoverHaciaDestino()
    {
        if (!estaVivo) return;

        Vector2 posicionActual = transform.position;
        Vector2 direccion = (destino - posicionActual).normalized;

        rigid.linearVelocity = direccion * velocidad;
    }

    void MirarAlJugador()
    {
        if (!estaVivo || jugador == null) return;

        Vector3 direccion = jugador.position - transform.position;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angulo + 90);
    }
}