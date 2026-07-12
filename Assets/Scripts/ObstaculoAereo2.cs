using UnityEngine;

public class ObstaculoAereo2 : MonoBehaviour
{
   public static bool HayAveActiva = false;

    void OnEnable()  { HayAveActiva = true; }
    void OnDisable() { HayAveActiva = false; }

    void Update()
    {
        if (LevelScroller.Instance == null) return;
        if (GameManager.Instance?.EstadoActual != GameManager.Estado.Jugando) return;

        transform.Translate(Vector3.back * LevelScroller.Instance.VelocidadActual * Time.deltaTime);

        if (transform.position.z < -25f)
        {
            gameObject.SetActive(false);
            AereoSpawner.Instance?.RetornarAlPool(gameObject);
        }
    }
}
