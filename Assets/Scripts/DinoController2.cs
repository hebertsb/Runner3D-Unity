using UnityEngine;
using UnityEngine.InputSystem;
public class DinoController2 : MonoBehaviour
{

    [Header("Movimiento")]
    [SerializeField] private float velocidadCorrer = 5f;

    [Header("Fisicas de Salto")]
    [SerializeField] private float fuerzaSalto = 8f;
    [SerializeField] private float multiplicadorCaida = 2.5f;

    private Rigidbody rb;
    private Animator animator;
    private CapsuleCollider capsule;
    private bool estaEnSuelo;

    private float alturaColNormal;
    private float alturaColAgachado;
    private Vector3 centroNormal;
    private Vector3 centroAgachado;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            alturaColNormal = capsule.height;
            centroNormal = capsule.center;
            alturaColAgachado = capsule.height * 0.5f;
            centroAgachado = new Vector3(capsule.center.x, capsule.center.y * 0.5f, capsule.center.z);
        }
    }

    void Update()
    {
        // El dinosaurio está en el suelo si no se está moviendo verticalmente (Y cercano a 0)
        estaEnSuelo = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        bool jugando = GameManager.Instance != null &&
                       GameManager.Instance.EstadoActual == GameManager.Estado.Jugando;

        // Movimiento constante hacia adelante
        float velocidadX = jugando ? velocidadCorrer : 0f;
        float velocidadY = rb.linearVelocity.y;

        // Saltar con la tecla S o Flecha Arriba
        bool salto = jugando && (
            Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame);

        if (salto && estaEnSuelo)
        {
            velocidadY = fuerzaSalto;
            AudioManager.Instance?.ReproducirSalto();
        }

        // Aplicar vector de velocidad limpio
        rb.linearVelocity = new Vector3(velocidadX, velocidadY, rb.linearVelocity.z);

        // Control del agachado
        bool agachado = jugando && !salto && (
            Keyboard.current.downArrowKey.isPressed ||
            Keyboard.current.leftCtrlKey.isPressed);

        if (animator != null)
        {
            animator.SetBool("IsRunning", jugando);
            animator.SetBool("isCrouching", agachado);
            animator.SetBool("IsGrounded", estaEnSuelo);
        }

        if (capsule != null)
        {
            capsule.height = agachado ? alturaColAgachado : alturaColNormal;
            capsule.center = agachado ? centroAgachado : centroNormal;
        }
    }

    void FixedUpdate()
    {
        // Gravedad aumentada para una caída más pesada/arcade
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstaculo") || other.CompareTag("ObstaculoAereo"))
        {
            CameraShake.Instance?.Shake(0.3f, 0.4f);
            if (animator != null) animator.SetTrigger("Morir");
            GameManager.Instance.TriggerGameOver();
        }
    }

}
