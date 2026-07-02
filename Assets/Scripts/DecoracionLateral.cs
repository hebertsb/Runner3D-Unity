using System.Collections.Generic;
using UnityEngine;

public class DecoracionLateral : MonoBehaviour
{
    public static DecoracionLateral Instance { get; private set; }

    [SerializeField] private GameObject prefabDecoracion;
    [SerializeField] private int cantidad = 10;
    [SerializeField] private float distanciaSpawn = 35f;
    [SerializeField] private float limiteRetorno = -25f;
    [SerializeField] private float intervaloMin = 1.5f;
    [SerializeField] private float intervaloMax = 4f;
    [SerializeField] private float xOffset = 5f;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activos = new List<GameObject>();
    private float timer;
    private float intervaloActual;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (prefabDecoracion == null) return;
        for (int i = 0; i < cantidad; i++)
        {
            GameObject obj = Instantiate(prefabDecoracion);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        intervaloActual = Random.Range(intervaloMin, intervaloMax);

        // Pre-coloca decoraciones a lo largo del camino visible
        for (float z = 5f; z <= distanciaSpawn; z += 6f)
        {
            PreSpawn(-xOffset, z);
            PreSpawn(xOffset, z);
        }
    }

    void PreSpawn(float x, float z)
    {
        if (pool.Count == 0) return;
        GameObject obj = pool.Dequeue();
        obj.transform.position = new Vector3(x, 0f, z);
        obj.SetActive(true);
        activos.Add(obj);
    }

    void Update()
    {
        if (GameManager.Instance?.EstadoActual != GameManager.Estado.Jugando) return;

        for (int i = activos.Count - 1; i >= 0; i--)
        {
            GameObject obj = activos[i];
            if (obj == null || !obj.activeSelf) { activos.RemoveAt(i); continue; }

            obj.transform.Translate(Vector3.back * LevelScroller.Instance.VelocidadActual * Time.deltaTime);

            if (obj.transform.position.z < limiteRetorno)
            {
                obj.SetActive(false);
                pool.Enqueue(obj);
                activos.RemoveAt(i);
            }
        }

        timer += Time.deltaTime;
        if (timer >= intervaloActual)
        {
            timer = 0f;
            intervaloActual = Random.Range(intervaloMin, intervaloMax);
            Spawn(-xOffset);
            Spawn(xOffset);
        }
    }

    void Spawn(float x)
    {
        if (pool.Count == 0) return;
        GameObject obj = pool.Dequeue();
        obj.transform.position = new Vector3(x, 0f, distanciaSpawn);
        obj.SetActive(true);
        activos.Add(obj);
    }
}
