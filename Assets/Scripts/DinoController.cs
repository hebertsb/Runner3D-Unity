using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DinoController : MonoBehaviour
{
    [Header("Fisicas de Salto")]
    [SerializeField] private float fuerzaSalto = 8f;
    [SerializeField] private float multiplicadorCaida = 2.5f;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform puntoSuelo;

    private Rigidbody rb;
    private Animator animator; // Agregado para el requerimiento de agachado
    private bool estaEnSuelo;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Obtener el componente Animator
    }

    void Update()
    {
        estaEnSuelo = Physics.CheckSphere(puntoSuelo.position, 0.2f, capaSuelo);

        // --- LÓGICA DE SALTO ---
        bool salto = Keyboard.current.spaceKey.wasPressedThisFrame ||
                     Keyboard.current.wKey.wasPressedThisFrame ||
                     Keyboard.current.upArrowKey.wasPressedThisFrame;

        if (salto && estaEnSuelo)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerzaSalto, rb.linearVelocity.z);
        }

        // --- LÓGICA DE AGACHADO (Input.GetKey -> animator.SetBool) ---
        bool agachado = Keyboard.current.sKey.isPressed || 
                        Keyboard.current.downArrowKey.isPressed || 
                        Keyboard.current.leftCtrlKey.isPressed;

        if (animator != null)
        {
            animator.SetBool("isCrouching", agachado);
        }
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Nota: Asegúrate de que los obstáculos tengan la etiqueta "Obstaculo"
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            Debug.Log("GAME OVER");
            
            // Cuando el GameManager esté listo, tu equipo descomentará esto:
            // GameManager.Instance.TriggerGameOver();
        }
    }
}
