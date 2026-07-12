using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    [Header("Configuracion del Pool")]
    [SerializeField] private GameObject prefabObstaculo;
    [SerializeField] private GameObject prefabMovil;
    [SerializeField] private int tamanoPool = 10;
    [SerializeField] private float distanciaSpawn = 30f;
    [SerializeField] private float intervaloSpawn = 2f;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private Queue<GameObject> poolMovil = new Queue<GameObject>();
    private float timerSpawn;
    private float pausaTimer;
    private int contadorSpawn = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < tamanoPool; i++)
        {
            GameObject obj = Instantiate(prefabObstaculo);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        if (prefabMovil != null)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject obj = Instantiate(prefabMovil);
                obj.SetActive(false);
                poolMovil.Enqueue(obj);
            }
        }
    }

    public void PausarPorSegundos(float segundos)
    {
        pausaTimer = segundos;
        timerSpawn = 0f;
    }

    void Update()
    {
        if (GameManager.Instance?.EstadoActual != GameManager.Estado.Jugando) return;
        if (pausaTimer > 0) { pausaTimer -= Time.deltaTime; return; }

        timerSpawn += Time.deltaTime;
        if (timerSpawn >= intervaloSpawn)
        {
            timerSpawn = 0f;
            SpawnObstaculo();
        }
    }

    void SpawnObstaculo()
    {
        if (ObstaculoAereo.HayAveActiva) return;

        contadorSpawn++;

        // Cada 3 cactus spawna 1 cubo movil
        if (contadorSpawn % 2 == 0 && poolMovil.Count > 0 && prefabMovil != null)
        {
            GameObject movil = poolMovil.Dequeue();
            movil.transform.position = new Vector3(0f, 0.5f, distanciaSpawn);
            movil.SetActive(true);
            return;
        }

        if (pool.Count == 0) return;
        GameObject obstaculo = pool.Dequeue();
        obstaculo.transform.position = new Vector3(0f, 0.5f, distanciaSpawn);
        obstaculo.SetActive(true);
    }

    public void RetornarAlPool(GameObject obstaculo)
    {
        obstaculo.SetActive(false);
        // detecta si es movil o cactus por el nombre del prefab
        if (obstaculo.name.Contains("Movil") || obstaculo.name.Contains("movil"))
            poolMovil.Enqueue(obstaculo);
        else
            pool.Enqueue(obstaculo);
    }
}
