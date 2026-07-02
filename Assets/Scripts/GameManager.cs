using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Estado { Menu, Jugando, GameOver }
    public Estado EstadoActual { get; private set; } = Estado.Menu;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        UIManager.Instance.MostrarMenu();
    }

    public void IniciarJuego()
    {
        EstadoActual = Estado.Jugando;
        LevelScroller.Instance.IniciarJuego();
        UIManager.Instance.MostrarHUD();
    }

    public void TriggerGameOver()
    {
        if (EstadoActual != Estado.Jugando) return;
        EstadoActual = Estado.GameOver;
        LevelScroller.Instance.DetenerJuego();
        UIManager.Instance.MostrarGameOver();
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
