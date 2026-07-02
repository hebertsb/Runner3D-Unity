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
        {
            var gm = GameManager.Instance;
            float multi = gm.ComboMultiplicador();
            int puntos = Mathf.RoundToInt(gm.Puntos);
            int record = gm.Record;
            int nivel = gm.NivelActual;
            int combo = gm.Combo;

            string comboTexto = multi > 1f ? $"  <color=#FFD700>x{multi:0}</color>" : "";
            string recordTexto = puntos > record ? "  <color=#39FF14>NUEVO RECORD!</color>" : $"  Mejor: {record}";

            textoPuntaje.text = $"Puntos: {puntos}{comboTexto}\nNivel: {nivel}  |  Esquivados: {gm.ObstaculosEsquivados}{recordTexto}";
        }
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
        bool esRecord = puntos >= record && puntos > 0;

        if (textoGameOver != null)
            textoGameOver.text = $"GAME OVER\nPuntos: {puntos}\nEsquivados: {GameManager.Instance.ObstaculosEsquivados}";

        if (textoRecord != null)
            textoRecord.text = esRecord
                ? $"<color=#FFD700>★ NUEVO RECORD: {record} ★</color>"
                : $"Record: {record}";
    }

    public void BotonJugar() => GameManager.Instance.IniciarJuego();
    public void BotonReintentar() => GameManager.Instance.Reiniciar();
}
