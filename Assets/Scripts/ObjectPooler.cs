using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    [Header("Configuracion del Pool")]
    [SerializeField] private GameObject prefabObstaculo;
    [SerializeField] private int tamanoPool = 10;
    [SerializeField] private float distanciaSpawn = 30f;
    [SerializeField] private float intervaloSpawn = 2f;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private float timerSpawn;

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
        for (int i = 0; i < tamanoPool; i++)
        {
            GameObject obj = Instantiate(prefabObstaculo);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    void Update()
    {
        timerSpawn += Time.deltaTime;
        if (timerSpawn >= intervaloSpawn)
        {
            timerSpawn = 0f;
            SpawnObstaculo();
        }
    }

    void SpawnObstaculo()
    {
        if (pool.Count == 0) return;

        GameObject obstaculo = pool.Dequeue();
        obstaculo.transform.position = new Vector3(0f, 0.5f, distanciaSpawn);
        obstaculo.SetActive(true);
    }

    public void RetornarAlPool(GameObject obstaculo)
    {
        obstaculo.SetActive(false);
        pool.Enqueue(obstaculo);
    }
}
