using UnityEngine;

public class LevelScroller : MonoBehaviour
{
    public static LevelScroller Instance { get; private set; }

    [Header("Velocidad")]
    [SerializeField] private float velocidadInicial = 8f;
    [SerializeField] private float incrementoVelocidad = 0.5f;
    [SerializeField] private float velocidadMaxima = 25f;
    [SerializeField] private float intervaloIncremento = 5f;

    [Header("Reciclado de Pista")]
    [SerializeField] private float limiteRetorno = -20f;

    public float VelocidadActual { get; private set; }

    private bool enJuego = false;
    private float timerIncremento;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        VelocidadActual = velocidadInicial;
    }

    public void IniciarJuego()
    {
        enJuego = true;
        VelocidadActual = velocidadInicial;
        timerIncremento = 0f;
    }

    public void DetenerJuego()
    {
        enJuego = false;
    }

    void Update()
    {
        if (!enJuego) return;

        // Mover todos los objetos con tag "Scrolleable" hacia el jugador
        GameObject[] objetos = GameObject.FindGameObjectsWithTag("Scrolleable");
        foreach (GameObject obj in objetos)
        {
            obj.transform.Translate(Vector3.back * VelocidadActual * Time.deltaTime);

            // Reciclar obstáculos que pasaron al jugador
            if (obj.transform.position.z < limiteRetorno && obj.CompareTag("Obstaculo"))
                ObjectPooler.Instance.RetornarAlPool(obj);
        }

        // Incrementar velocidad progresivamente
        timerIncremento += Time.deltaTime;
        if (timerIncremento >= intervaloIncremento)
        {
            timerIncremento = 0f;
            VelocidadActual = Mathf.Min(VelocidadActual + incrementoVelocidad, velocidadMaxima);
        }
    }
}
