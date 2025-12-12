using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [SerializeField] float amplitud = 0.5f;
    [SerializeField] float velocidad = 1f;
    private Vector3 posicionInicial;

    void Start() => posicionInicial = transform.position;

    void Update()
    {
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidad) * amplitud;
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
    }
}
