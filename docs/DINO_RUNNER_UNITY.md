# DINO RUNNER 3D — Proyecto de Feria con Unity
> Base: Motor Unity (3D / 2.5D), Universal Render Pipeline (URP). 3 días, 5 personas.

---

## ANÁLISIS DEL PROYECTO EN UNITY

### Stack de Desarrollo
| Componente | Detalle |
|---|---|
| Motor de Juego | Unity 2022.3 LTS o superior |
| Lenguaje | C# (.NET Standard 2.1) |
| Render Pipeline | Universal Render Pipeline (URP) — Iluminación elegante y post-procesado |
| Físicas | Nvidia PhysX (Unity 3D Physics) — `Rigidbody` y `Colliders` |
| Entrada | Input System clásico (`Input.GetKeyDown`) o New Input System |
| Animaciones | Unity Animator (Mecanim) — Máquina de estados integrada |
| Cámara | Cinemachine — Movimiento suave y efectos de impacto |

### Sistemas a Implementar
| Sistema | Script Principal | Complejidad |
|---|---|---|
| Control de Físicas y Movimiento | `DinoController.cs` | Media |
| Reciclador de Escenario (Object Pooling) | `ObjectPooler.cs` | Media-Alta |
| Desplazamiento del Mundo (Scrolling) | `LevelScroller.cs` | Baja |
| Máquina de Estados del Juego | `GameManager.cs` | Media |
| Interfaz de Usuario (UI elegante) | `UIManager.cs` | Baja-Media |
| Efectos de Post-Procesado y Luces | URP Volume / VFX Graph | Media |

---

## ESTRUCTURA DE CLASES (SCRIPTS C#)

```
Assets/
└── Scripts/
    ├── DinoController.cs   → Entrada de jugador, salto físico, deslizamiento y triggers de animación.
    ├── LevelScroller.cs    → Mueve los obstáculos y el suelo hacia atrás para simular velocidad.
    ├── ObjectPooler.cs     → Cola de obstáculos y segmentos de suelo para reciclaje (evita usar Instantiate/Destroy).
    ├── GameManager.cs      → Puntuación, récords, control de estados (Menú, Jugando, GameOver) e inicio/reinicio.
    ├── UIManager.cs        → Controla las pantallas de UI, el contador de puntos y transiciones de desvanecimiento (Fade).
    └── CinemachineShake.cs → Lógica para sacudir la cámara en colisiones críticas usando Cinemachine Noise.
```

---

## FÍSICA Y LÓGICA CLAVE — IMPLEMENTACIÓN EN UNITY

### 1. Control del Salto y Gravedad Personalizada (`DinoController.cs`)
En Unity, usar la gravedad por defecto puede hacer que el salto se sienta flotante. Aplicamos una gravedad adicional en la caída para que el control se sienta responsivo y preciso.

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DinoController : MonoBehaviour
{
    [Header("Físicas")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float fallMultiplier = 2.5f; // Salto más pesado al caer
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Detección de suelo usando una pequeña esfera física
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            animator.SetTrigger("Jump");
        }

        // Agacharse
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.S))
        {
            animator.SetBool("IsCrouching", true);
            // Opcional: Reducir altura del collider
        }
        else
        {
            animator.SetBool("IsCrouching", false);
        }
    }

    void FixedUpdate()
    {
        // Gravedad pesada en caída libre
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
}
```

### 2. Reciclador de Escenario (`ObjectPooler.cs`)
En un juego de carrera infinita, crear e instanciar obstáculos constantemente en tiempo real genera picos de lag (recolección de basura). Reutilizamos los elementos usando una cola.

```csharp
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
```

---

## LO QUE CAMBIA RESPECTO A OPENGL 2D MANUAL

*   **Físicas Automatizadas:** No necesitas escribir fórmulas de gravedad ni resolver colisiones AABB con lógica personalizada. El motor físico de Unity (`Rigidbody` + `OnCollisionEnter`) lo maneja de forma eficiente y precisa.
*   **Renderizado y Luces:** El dibujado de formas por coordenadas NDC desaparece. Colocas modelos 3D directamente en la escena, y URP se encarga de proyectar sombras dinámicas y reflejos en tiempo real.
*   **Efectos Cinematográficos:** Puedes agregar efectos de desenfoque de movimiento (*Motion Blur*), resplandor (*Bloom*), y sacudida de pantalla (*Camera Shake*) sin escribir shaders avanzados desde cero.
*   **Fondo Infinito Real:** En lugar de capas 2D con paralaje simple, el fondo puede ser un desierto tridimensional con dunas reales que se mueven detrás del jugador, lo que le da una profundidad visual espectacular.

---

## DIVISIÓN DE TRABAJO PARA 5 PERSONAS

| Rol / Persona | Responsabilidad Principal | Entregables clave |
|---|---|---|
| **P1 — Jugabilidad y Físicas** | Lógica e Input del Dino | `DinoController.cs`, control del salto, agachado, y ajuste de gravedad. |
| **P2 — Mundo Infinito** | Reciclaje y Scrolling del Escenario | `ObjectPooler.cs`, `LevelScroller.cs`, generación infinita de suelo y spawn aleatorio de obstáculos. |
| **P3 — GameManager e Integración** | Control de Estados, Puntos e Interfaces | `GameManager.cs`, `UIManager.cs`, sistema de puntuación, pantallas de Menú y GameOver, y control del audio. |
| **P4 — Arte 3D, Iluminación y VFX** | Diseño del desierto 3D y Efectos Visuales | Creación/importación de modelos, configuración de URP Post-processing, efectos de partículas de polvo al correr y explosiones al chocar. |
| **P5 — Animador y Cámara** | Animación del personaje y Cinemachine | Configuración de las animaciones del Dino en el Animator, transiciones fluidas, cámara lenta en la colisión final y efecto de vibración en la cámara. |

---

## PLAN DE TRABAJO (3 DÍAS)

*   **Día 1: Mecánicas Base**
    *   Jugador con salto y gravedad responsiva en un plano.
    *   Obstáculos básicos moviéndose hacia el jugador.
    *   Detección de colisiones básica (Game Over inmediato al chocar).
*   **Día 2: Reciclador de Mundo y Animación**
    *   Generador infinito implementado (Object Pooler).
    *   Importación de modelos 3D y configuración del Animator (Idle, Run, Jump, Crouch).
    *   Integración de interfaz de usuario básica y contador de puntuación.
*   **Día 3: Pulido Visual, Audio y Post-Procesamiento**
    *   Configuración del URP Volume (Bloom, Color Grading para simular atardecer atmosférico).
    *   Sistema de partículas para el polvo en los pies del Dino al correr.
    *   Efectos de cámara (Cinemachine) y sacudidas en colisiones.
    *   Ajuste final de dificultad (lerp de velocidad progresiva).
