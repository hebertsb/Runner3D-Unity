using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Paneles")]
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelHUD;
    [SerializeField] private GameObject panelGameOver;

    [Header("Textos HUD")]
    [SerializeField] private TextMeshProUGUI textoPuntaje;

    [Header("Textos GameOver")]
    [SerializeField] private TextMeshProUGUI textoGameOver;
    [SerializeField] private TextMeshProUGUI textoRecord;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (GameManager.Instance.EstadoActual == GameManager.Estado.Jugando && textoPuntaje != null)
            textoPuntaje.text = "Puntos: " + Mathf.RoundToInt(GameManager.Instance.Puntos)
                              + "  |  Vel: " + Mathf.RoundToInt(LevelScroller.Instance.VelocidadActual);
    }

    public void MostrarMenu()
    {
        panelMenu?.SetActive(true);
        panelHUD?.SetActive(false);
        panelGameOver?.SetActive(false);
    }

    public void MostrarHUD()
    {
        panelMenu?.SetActive(false);
        panelHUD?.SetActive(true);
        panelGameOver?.SetActive(false);
    }

    public void MostrarGameOver()
    {
        panelMenu?.SetActive(false);
        panelHUD?.SetActive(false);
        panelGameOver?.SetActive(true);

        int puntos = Mathf.RoundToInt(GameManager.Instance.Puntos);
        int record = GameManager.Instance.Record;

        if (textoGameOver != null)
            textoGameOver.text = "GAME OVER\nPuntos: " + puntos;

        if (textoRecord != null)
            textoRecord.text = (puntos >= record ? "NUEVO RECORD: " : "Record: ") + record;
    }

    public void BotonJugar() => GameManager.Instance.IniciarJuego();
    public void BotonReintentar() => GameManager.Instance.Reiniciar();
}
