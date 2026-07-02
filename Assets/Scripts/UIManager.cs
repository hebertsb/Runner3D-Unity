using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Paneles")]
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelHUD;
    [SerializeField] private GameObject panelGameOver;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI textoPuntaje;
    [SerializeField] private TextMeshProUGUI textoGameOver;

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
            textoPuntaje.text = "Velocidad: " + Mathf.RoundToInt(LevelScroller.Instance.VelocidadActual);
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
        if (textoGameOver != null)
            textoGameOver.text = "GAME OVER\nVelocidad: " + Mathf.RoundToInt(LevelScroller.Instance.VelocidadActual);
    }

    public void BotonJugar() => GameManager.Instance.IniciarJuego();
    public void BotonReintentar() => GameManager.Instance.Reiniciar();
}
