# ESTUDIO 01 — Visión General del Proyecto

> Empezá acá. Este MD te da el mapa completo antes de entrar al detalle.

---

## ¿Qué es el juego?

Runner 3D es un **endless runner** — el jugador nunca para, los obstáculos vienen solos,
y el objetivo es sobrevivir el mayor tiempo posible acumulando puntos.

No hay niveles, no hay fin. La dificultad aumenta sola con el tiempo.

---

## Los 11 scripts y su rol en UNA línea

| Script | Qué hace |
|--------|----------|
| `GameManager` | Controla si el juego está en Menú, Jugando o GameOver |
| `UIManager` | Muestra/oculta los paneles de pantalla (HUD, menú, game over) |
| `DinoController` | Maneja el input del jugador: salto, agacharse, colisiones |
| `LevelScroller` | Mueve los obstáculos y aumenta la velocidad cada 5 segundos |
| `GroundScroller` | Recicla los 3 tiles del suelo para que parezca infinito |
| `ObjectPooler` | Administra el pool de cactus (spawna y recicla) |
| `AereoSpawner` | Administra el pool del Pteranodon (spawna y recicla) |
| `ObstaculoAereo` | El Pteranodon se mueve solo y avisa si está activo |
| `DecoracionLateral` | Spawna cactus decorativos a los costados (no matan) |
| `AudioManager` | Reproduce música de fondo y efectos de sonido |
| `CameraShake` | Vibra la cámara cuando el jugador muere |

---

## Cómo se conectan — diagrama de dependencias

```
                    ┌─────────────────┐
                    │   GameManager   │ ← cerebro central
                    │  (estado del    │
                    │    juego)       │
                    └────────┬────────┘
                             │ controla
              ┌──────────────┼──────────────┐
              ▼              ▼              ▼
        UIManager      LevelScroller   AudioManager
        (pantallas)    (velocidad +    (sonidos)
                        mover objetos)
                             │
                    ┌────────┴────────┐
                    ▼                 ▼
             ObjectPooler       AereoSpawner
             (cactus pool)      (ave pool)
                    │                 │
                    ▼                 ▼
              [Cactus 3D]      [Pteranodon 3D]
                    │                 │
                    └────────┬────────┘
                             ▼
                      DinoController
                      (detecta colisión
                       → llama GameManager)
```

---

## Flujo de una partida completa

```
INICIO
  │
  ▼
GameManager.Start() → UIManager.MostrarMenu()
  │
  │ [jugador presiona Jugar]
  ▼
GameManager.IniciarJuego()
  ├─ Estado = Jugando
  ├─ LevelScroller.IniciarJuego() → velocidad = 8
  └─ UIManager.MostrarHUD()
  │
  │ [loop del juego]
  ▼
┌─────────────────────────────────────────┐
│ • GroundScroller recicla el suelo       │
│ • LevelScroller mueve cactus            │
│ • ObjectPooler spawna cactus cada 2s    │
│ • AereoSpawner spawna ave cada 6-12s    │
│ • DinoController lee input del teclado  │
│ • GameManager suma puntos × combo       │
│ • Cada 5s: velocidad += 0.5             │
└─────────────────────────────────────────┘
  │
  │ [jugador toca obstáculo]
  ▼
DinoController.OnTriggerEnter()
  ├─ CameraShake.Shake()
  ├─ Animator → animación morir
  └─ GameManager.TriggerGameOver()
       ├─ LevelScroller.DetenerJuego()
       ├─ Guardar record en PlayerPrefs
       ├─ AudioManager.ReproducirGameOver()
       └─ UIManager.MostrarGameOver()
  │
  │ [jugador presiona Reintentar]
  ▼
SceneManager.LoadScene() → reinicia todo
```

---

## Patrón más importante: Singleton

Casi todos los scripts usan este patrón:

```csharp
public static GameManager Instance { get; private set; }

void Awake()
{
    if (Instance != null && Instance != this) { Destroy(gameObject); return; }
    Instance = this;
}
```

**Por qué:** Permite que cualquier script llame `GameManager.Instance.TriggerGameOver()`
sin necesidad de arrastrar referencias en el Inspector.

**Riesgo:** Si hay dos GameManager en la escena, el segundo se destruye solo.

---

## Preguntas de autoevaluación

1. ¿Cuáles son los 3 estados del juego y qué los cambia?
2. ¿Qué script mueve físicamente los cactus?
3. ¿Qué script decide cuándo spawnar el Pteranodon?
4. ¿Por qué usamos Singleton? ¿Qué problema resuelve?
5. ¿Qué pasa cuando el jugador toca un cactus? Nombrá los scripts involucrados en orden.

---

> **Siguiente:** ESTUDIO_02_PLAYER_FISICAS.md
