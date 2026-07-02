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
    private CapsuleCollider capsule;
    private bool estaEnSuelo;
    private float timerCoyote;
    private const float COYOTE_TIME = 0.15f;

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
        bool enSueloFisico = Physics.CheckSphere(puntoSuelo.position, 0.2f, capaSuelo);
        if (enSueloFisico)
            timerCoyote = COYOTE_TIME;
        else
            timerCoyote -= Time.deltaTime;
        estaEnSuelo = timerCoyote > 0f;

        bool jugando = GameManager.Instance != null &&
                       GameManager.Instance.EstadoActual == GameManager.Estado.Jugando;

        bool salto = jugando && (
            Keyboard.current.spaceKey.wasPressedThisFrame ||
            Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame);

        if (salto && estaEnSuelo)
        {
            timerCoyote = 0f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerzaSalto, rb.linearVelocity.z);
            AudioManager.Instance?.ReproducirSalto();
        }

        bool agachado = jugando && !salto && (
            Keyboard.current.sKey.isPressed ||
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
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
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
