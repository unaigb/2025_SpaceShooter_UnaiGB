using UnityEngine;
using System.Collections;

public class PlayerModel
{
    protected int vidas = 3;
    protected int salud = 100;
    protected int puntuacion = 0;
    protected float velocidad;
    public int pVidas
    {
        get { return vidas; }
        set { vidas = value; }
    }
    public int pSalud
    {
        get { return salud; }
        set { salud = value; }
    }
    public int pPuntuacion
    {
        get { return puntuacion; }
        set { puntuacion = value; }
    }
    public float pVelocidad
    {
        get { return velocidad; }
        set { velocidad = value; }
    }
}