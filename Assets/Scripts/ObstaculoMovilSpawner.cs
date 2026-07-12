using System.Collections.Generic;
using UnityEngine;

public class ObstaculoMovilSpawner : MonoBehaviour
{
    public static ObstaculoMovilSpawner Instance { get; private set; }

    [SerializeField] private GameObject prefabMovil;
    [SerializeField] private int tamanoPool = 4;
    [SerializeField] private float distanciaSpawn = 30f;
    [SerializeField] private float intervaloMin = 8f;
    [SerializeField] private float intervaloMax = 15f;
    [SerializeField] private float alturaObstaculo = 0.5f;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private float timer;
    private float intervaloActual;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (prefabMovil == null) return;
        for (int i = 0; i < tamanoPool; i++)
        {
            GameObject obj = Instantiate(prefabMovil);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        intervaloActual = Random.Range(intervaloMin, intervaloMax);
    }

    void Update()
    {
        if (GameManager.Instance?.EstadoActual != GameManager.Estado.Jugando) return;

        timer += Time.deltaTime;
        if (timer >= intervaloActual)
        {
            timer = 0f;
            intervaloActual = Random.Range(intervaloMin, intervaloMax);
            Spawn();
        }
    }

    void Spawn()
    {
        if (ObstaculoAereo.HayAveActiva) return;
        if (pool.Count == 0) return;
        GameObject obj = pool.Dequeue();
        obj.transform.position = new Vector3(0f, alturaObstaculo, distanciaSpawn);
        obj.SetActive(true);
    }

    public void RetornarAlPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
