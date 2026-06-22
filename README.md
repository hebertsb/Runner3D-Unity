# Runner 3D — Unity 6

Videojuego de tipo *endless runner* 3D desarrollado en Unity 6 con Universal Render Pipeline (URP) y C#.

---

## ¿De qué trata el juego?

El jugador controla a **Dino**, un personaje que corre automáticamente por un desierto 3D infinito. El objetivo es sobrevivir el mayor tiempo posible esquivando obstáculos que aparecen en el camino. La velocidad del juego aumenta progresivamente, haciendo cada vez más difícil reaccionar a tiempo.

Inspirado en el clásico *T-Rex Runner* de Google Chrome, pero llevado a un entorno 3D con efectos visuales modernos.

---

## Controles

| Tecla | Acción |
|---|---|
| `Espacio` | Saltar |
| `W` o `↑` | Saltar |
| `S` o `LeftShift` | Agacharse (deslizarse) |

---

## Cómo funciona — Estructura del proyecto

```
Assets/
├── Scripts/
│   ├── DinoController.cs   → Control del jugador: salto, gravedad, colisión
│   ├── LevelScroller.cs    → Mueve obstáculos hacia el jugador (ilusión de velocidad)
│   ├── ObjectPooler.cs     → Recicla obstáculos sin crear/destruir en runtime
│   ├── GameManager.cs      → Estados del juego: Menú, Jugando, GameOver + puntuación
│   ├── UIManager.cs        → Pantallas e interfaz de usuario
│   └── CinemachineShake.cs → Sacudida de cámara al morir
├── Scenes/
│   └── GameScene.unity     → Escena principal del juego
├── Materials/
│   └── MaterialDino.mat    → Color del personaje
├── Prefabs/                → Obstáculos y segmentos de suelo prefabricados
├── Models/                 → Modelos 3D del personaje y entorno
├── Audio/                  → Música de fondo y efectos de sonido
└── Animations/             → Animaciones: Idle, Run, Jump, Crouch
```

---

## Sistemas implementados

### Física del salto
El personaje usa un `Rigidbody` con gravedad aumentada en la caída (`fallMultiplier = 2.5`). Esto produce un salto más responsivo y pesado que la física predeterminada, dando mayor sensación de control.

### Mundo infinito (Object Pooling)
En lugar de crear y destruir obstáculos continuamente (lo que genera pausas por el recolector de basura de C#), se pre-instancian objetos al inicio y se reciclan mediante una cola (`Queue<GameObject>`). Esto garantiza 60 FPS estables sin micro-pausas.

### Ilusión de velocidad
El Dino permanece fijo en el espacio. Son los obstáculos y el suelo los que se mueven hacia él con `LevelScroller.cs`. La velocidad aumenta progresivamente con el tiempo.

### Máquina de estados
`GameManager` controla los estados del juego:
```
Menu → Jugando → GameOver → Menu
```

---

## Stack tecnológico

| Componente | Tecnología |
|---|---|
| Motor | Unity 6.5 (6000.5.0f1) |
| Lenguaje | C# con .NET |
| Render Pipeline | Universal Render Pipeline (URP) |
| Físicas | NVIDIA PhysX (Rigidbody) |
| Entrada | New Input System (Keyboard.current) |
| Cámara | Cinemachine |

---

## Cómo abrir el proyecto

1. Instalar **Unity Hub** y **Unity 6.5**
2. Clonar el repositorio:
```bash
git clone https://github.com/hebertsb/Runner3D-Unity.git
```
3. Unity Hub → **Projects → Add → Add project from disk** → seleccionar la carpeta clonada
4. Abrir `GameScene` desde `Assets/Scenes/`
5. Presionar ▶ Play

Ver [GUIA_SETUP_PROYECTO.md](../GUIA_SETUP_PROYECTO.md) para instrucciones detalladas.

---

## Equipo de desarrollo

Proyecto universitario — Programación Gráfica, Feria de Exposición Académica.

| Persona | Rol |
|---|---|
| P1 | Jugabilidad y Físicas (DinoController) |
| P2 | Mundo Infinito (ObjectPooler, LevelScroller) |
| P3 | GameManager e Interfaces (UI, audio, estados) |
| P4 | Arte 3D, Iluminación y VFX |
| P5 | Animaciones y Cámara (Cinemachine) |
