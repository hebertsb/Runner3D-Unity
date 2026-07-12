using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Estado { Menu, Jugando, GameOver }
    public Estado EstadoActual { get; private set; } = Estado.Menu;

    public float Puntos { get; private set; }
    public int Record { get; private set; }
    public int Combo { get; private set; }
    public int ObstaculosEsquivados { get; private set; }
    public int NivelActual { get; private set; } = 1;

    private const string CLAVE_RECORD = "Runner3D_Record";
    private const int COMBO_NIVEL2 = 5;
    private const int COMBO_NIVEL3 = 10;
    private const int COMBO_NIVEL4 = 20;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Record = PlayerPrefs.GetInt(CLAVE_RECORD, 0);
    }

    void Start()
    {
        UIManager.Instance.MostrarMenu();
    }

    void Update()
    {
        if (EstadoActual == Estado.Jugando)
        {
            float multiplicador = ComboMultiplicador();
            Puntos += LevelScroller.Instance.VelocidadActual * multiplicador * Time.deltaTime;

            // Nivel basado en velocidad (cada 2 unidades = 1 nivel)
            float vel = LevelScroller.Instance.VelocidadActual;
            NivelActual = Mathf.Max(1, Mathf.FloorToInt((vel - 6f) / 2f) + 1);
        }
    }

    public float ComboMultiplicador()
    {
        if (Combo >= COMBO_NIVEL4) return 4f;
        if (Combo >= COMBO_NIVEL3) return 3f;
        if (Combo >= COMBO_NIVEL2) return 2f;
        return 1f;
    }

    public void RegistrarEsquive()
    {
        if (EstadoActual != Estado.Jugando) return;
        ObstaculosEsquivados++;
        Combo++;
        AudioManager.Instance?.ReproducirEsquive();
    }

    public void IniciarJuego()
    {
        Puntos = 0f;
        Combo = 0;
        ObstaculosEsquivados = 0;
        NivelActual = 1;
        EstadoActual = Estado.Jugando;
        LevelScroller.Instance.IniciarJuego();
        UIManager.Instance.MostrarHUD();
    }

    public void TriggerGameOver()
    {
        if (EstadoActual != Estado.Jugando) return;
        EstadoActual = Estado.GameOver;
        Combo = 0;
        LevelScroller.Instance.DetenerJuego();

        int puntosFinales = Mathf.RoundToInt(Puntos);
        if (puntosFinales > Record)
        {
            Record = puntosFinales;
            PlayerPrefs.SetInt(CLAVE_RECORD, Record);
            PlayerPrefs.Save();
        }

        AudioManager.Instance?.ReproducirGameOver();
        UIManager.Instance.MostrarGameOver();
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
