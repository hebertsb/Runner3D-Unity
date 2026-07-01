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
    private Animator animator;
    private bool estaEnSuelo;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        LevelScroller.Instance.IniciarJuego(); // TEMPORAL
    }

    void Update()
    {
        estaEnSuelo = Physics.CheckSphere(puntoSuelo.position, 0.2f, capaSuelo);

        bool salto = Keyboard.current.spaceKey.wasPressedThisFrame ||
                     Keyboard.current.wKey.wasPressedThisFrame ||
                     Keyboard.current.upArrowKey.wasPressedThisFrame;

        if (salto && estaEnSuelo)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerzaSalto, rb.linearVelocity.z);

        bool agachado = Keyboard.current.sKey.isPressed ||
                        Keyboard.current.downArrowKey.isPressed ||
                        Keyboard.current.leftCtrlKey.isPressed;

        if (animator != null)
            animator.SetBool("isCrouching", agachado);
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstaculo"))
        {
            Debug.Log("GAME OVER");
            // GameManager.Instance.TriggerGameOver();
        }
    }
}
