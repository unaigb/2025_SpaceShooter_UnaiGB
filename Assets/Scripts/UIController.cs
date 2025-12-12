using TMPro;
using UnityEngine;
using UnityEngine.UI; // Necesario para manipular Imágenes (Barra y Vidas)

public class UIController : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text textoNivel;
    public TMP_Text textoPuntuacion;

    [Header("Salud (Barra)")]
    public Image barraSalud;

    [Header("Vidas (Iconos)")]
    public Image[] iconosVidas;

    // Colores para cuando tienes vida o la pierdes
    private Color colorVidaActiva = Color.white;
    private Color colorVidaPerdida = Color.black;

    // Referencia al jugador (igual que antes)
    protected PlayerModel playerModel;

    public PlayerModel getJugador()
    {
        return playerModel;
    }

    public void ActualizarNivel(int nivel)
    {
        textoNivel.text = "" + nivel;
    }

    public void ActualizarPuntuacion(int puntuacion)
    {
        textoPuntuacion.text = "" + puntuacion;
    }

    public void ActualizarSalud(float saludActual, float saludMaxima)
    {
        if (saludMaxima == 0) return;

        barraSalud.fillAmount = saludActual / saludMaxima;
    }

    public void ActualizarVidas(int vidasActuales)
    {
        for (int i = 0; i < iconosVidas.Length; i++)
        {
            if (i < vidasActuales)
            {
                iconosVidas[i].color = colorVidaActiva;
            }
            else
            {
                iconosVidas[i].color = colorVidaPerdida;
            }
        }
    }
}