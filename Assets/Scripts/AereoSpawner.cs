using System.Collections.Generic;
using UnityEngine;

public class AereoSpawner : MonoBehaviour
{
    public static AereoSpawner Instance { get; private set; }

    [SerializeField] private GameObject prefabAereo;
    [SerializeField] private int tamanoPool = 4;
    [SerializeField] private float distanciaSpawn = 30f;
    [SerializeField] private float intervaloMin = 6f;
    [SerializeField] private float intervaloMax = 12f;
    [SerializeField] private float alturaVuelo = 2.5f;

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
        if (prefabAereo == null) return;
        for (int i = 0; i < tamanoPool; i++)
        {
            GameObject obj = Instantiate(prefabAereo);
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
        // Esperar hasta que NINGÚN cactus esté en la pista (todos pasaron al jugador)
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Obstaculo"))
        {
            if (obs.activeInHierarchy && obs.transform.position.z > -2f)
            {
                timer = intervaloActual - 1f; // reintentar en ~1 segundo
                return;
            }
        }

        if (pool.Count == 0) return;
        GameObject obj = pool.Dequeue();
        obj.transform.position = new Vector3(0f, alturaVuelo, distanciaSpawn);
        obj.SetActive(true);
        ObjectPooler.Instance?.PausarPorSegundos(8f);
    }

    public void RetornarAlPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
