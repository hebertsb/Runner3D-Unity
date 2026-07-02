using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Estado { Menu, Jugando, GameOver }
    public Estado EstadoActual { get; private set; } = Estado.Menu;

    public float Puntos { get; private set; }
    public int Record { get; private set; }

    private const string CLAVE_RECORD = "Runner3D_Record";

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
            Puntos += LevelScroller.Instance.VelocidadActual * Time.deltaTime;
    }

    public void IniciarJuego()
    {
        Puntos = 0f;
        EstadoActual = Estado.Jugando;
        LevelScroller.Instance.IniciarJuego();
        UIManager.Instance.MostrarHUD();
    }

    public void TriggerGameOver()
    {
        if (EstadoActual != Estado.Jugando) return;
        EstadoActual = Estado.GameOver;
        LevelScroller.Instance.DetenerJuego();

        int puntosFinales = Mathf.RoundToInt(Puntos);
        if (puntosFinales > Record)
        {
            Record = puntosFinales;
            PlayerPrefs.SetInt(CLAVE_RECORD, Record);
            PlayerPrefs.Save();
        }

        UIManager.Instance.MostrarGameOver();
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
