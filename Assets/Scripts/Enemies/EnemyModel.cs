public class EnemyModel
{
    protected int vida = 100;
    protected float velocidad;
    public int pVida
    {
        get { return vida; }
        set { vida = value; }
    }
    public float pVelocidad
    {
        get { return velocidad; }
        set { velocidad = value; }
    }
}