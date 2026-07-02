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

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Scrolleable"))
            obj.transform.Translate(Vector3.back * VelocidadActual * Time.deltaTime);

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Obstaculo"))
        {
            obj.transform.Translate(Vector3.back * VelocidadActual * Time.deltaTime);
            if (obj.transform.position.z < limiteRetorno)
            {
                GameManager.Instance?.RegistrarEsquive();
                ObjectPooler.Instance.RetornarAlPool(obj);
            }
        }

        timerIncremento += Time.deltaTime;
        if (timerIncremento >= intervaloIncremento)
        {
            timerIncremento = 0f;
            VelocidadActual = Mathf.Min(VelocidadActual + incrementoVelocidad, velocidadMaxima);
        }
    }
}
