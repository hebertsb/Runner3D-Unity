# Runner 3D

Juego endless runner 3D desarrollado en Unity 6 con Universal Render Pipeline (URP).

Proyecto presentado en Feria Expociencia — UAGRM, materia Programación Gráfica ELC102.

## Descripción

El jugador controla un velociraptor que corre automáticamente y debe esquivar tres tipos de obstáculos:

- **Cactus** — obstáculo fijo, requiere saltar
- **Pteranodon** — obstáculo aéreo, requiere agacharse
- **Cubo móvil** — obstáculo que zigzaguea lateralmente, requiere saltar o esquivar por timing

El juego aumenta de velocidad con el tiempo, incrementando la dificultad progresivamente.

## Controles

| Acción | Teclas |
|--------|--------|
| Saltar | `Espacio` / `W` / `↑` |
| Agacharse | `S` / `↓` / `Ctrl izq` |

## Características

- Sistema de puntuación con multiplicador de combo (x1 / x2 / x3 / x4)
- Niveles basados en velocidad actual
- Registro de mejor puntaje (PlayerPrefs)
- Object Pooling para optimización de rendimiento
- Coyote Time en el salto (150ms de gracia)
- Tres tipos de obstáculos con comportamiento distinto

## Tecnologías

- Unity 6 (6000.0.5f1)
- Universal Render Pipeline (URP)
- New Input System
- C#

## Autor

<<<<<<< HEAD
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
# Runner3dUnity
# Runner3dUnity
=======
**Hebert Suárez Burgos**  
Universidad Autónoma Gabriel René Moreno (UAGRM)  
Programación Gráfica — ELC102
>>>>>>> eb81686ba21f4dee0076eafe1a076af0f67e195c
