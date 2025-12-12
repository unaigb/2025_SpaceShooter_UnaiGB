using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerShot : MonoBehaviour
{
    public float velocidad = 10.0f;
    private Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(mousePos);
        cursorPos.z = 0;

        Vector2 direccion = (cursorPos - transform.position).normalized;
        rigid.linearVelocity = direccion * velocidad;
    }

    void Update()
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(Vector2.zero);
        Vector2 max = Camera.main.ViewportToWorldPoint(Vector2.one);

        if (transform.position.x < min.x || transform.position.x > max.x ||
            transform.position.y < min.y || transform.position.y > max.y)
        {
            Destroy(gameObject);
        }
    }
}
