using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 posOriginal;
    private float timerShake;
    private float magnitud;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        posOriginal = transform.localPosition;
    }

    public void Shake(float intensidad, float duracion)
    {
        magnitud = intensidad;
        timerShake = duracion;
    }

    void Update()
    {
        if (timerShake > 0)
        {
            transform.localPosition = posOriginal + new Vector3(
                Random.Range(-magnitud, magnitud),
                Random.Range(-magnitud, magnitud),
                0f
            );
            timerShake -= Time.deltaTime;
            if (timerShake <= 0)
                transform.localPosition = posOriginal;
        }
    }
}
