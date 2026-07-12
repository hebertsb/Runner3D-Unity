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

**Hebert Suárez Burgos**  
Universidad Autónoma Gabriel René Moreno (UAGRM)  
Programación Gráfica — ELC102
