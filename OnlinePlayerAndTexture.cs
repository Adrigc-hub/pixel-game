using Unity.Netcode;
using UnityEngine;

public class OnlinePlayerAndTexture : NetworkBehaviour
{
    public float speed = 6f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        // Configuración de físicas para la esfera en 2D
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // El código dibuja la textura automáticamente al iniciar
        GenerarTexturaEsfera();
    }

    void Update()
    {
        // Si este personaje es de otro jugador en red, no lo movemos nosotros
        if (!IsOwner) return;

        // Capturar controles de flechas, WASD o joystick móvil
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 direccion = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = direccion * speed; 
    }

    // Cambia el color del jugador según quién lo controla en internet
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            spriteRenderer.color = Color.green; // Mi personaje es Verde
        }
        else
        {
            spriteRenderer.color = Color.red; // Los enemigos son Rojos
        }
    }

    // Algoritmo matemático para dibujar la textura de un círculo con borde negro
    private void GenerarTexturaEsfera()
    {
        int tamano = 64; 
        Texture2D textura = new Texture2D(tamano, tamano);
        float centro = tamano / 2f;
        float radioMax = tamano / 2f;
        float espesorBorde = 4f; 

        for (int y = 0; y < tamano; y++)
        {
            for (int x = 0; x < tamano; x++)
            {
                float distancia = Vector2.Distance(new Vector2(x, y), new Vector2(centro, centro));

                if (distancia < radioMax - espesorBorde)
                {
                    textura.SetPixel(x, y, Color.white); // Centro relleno
                }
                else if (distancia < radioMax)
                {
                    textura.SetPixel(x, y, Color.black); // Borde de la esfera
                }
                else
                {
                    textura.SetPixel(x, y, Color.clear); // Fondo transparente
                }
            }
        }

        textura.Apply();

        // Convertir los píxeles generados en un Sprite que el juego pueda renderizar
        Sprite nuevoSprite = Sprite.Create(textura, new Rect(0, 0, tamano, tamano), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = nuevoSprite;
    }
}
