using UnityEngine;

public class ObstaculoMovil : MonoBehaviour
{
    private float tiempoInicio;
    private float amplitud;
    private float frecuencia;

    void OnEnable()
    {
        tiempoInicio = Time.time;
        amplitud  = Random.Range(1.5f, 3f);
        frecuencia = Random.Range(1.5f, 3f);
    }

    void Update()
    {
        if (GameManager.Instance?.EstadoActual != GameManager.Estado.Jugando) return;

        // Solo oscila en X — LevelScroller ya mueve el Z porque tiene tag "Obstaculo"
        float x = Mathf.Sin((Time.time - tiempoInicio) * frecuencia) * amplitud;
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
