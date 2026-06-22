# Runner 3D: Optimización y desarrollo procedimental en C# y Unity

**Proyecto de Investigación — Programación Gráfica**
**Feria de Exposición Académica**

---

*Formato de presentación: Normas APA 7.ª edición*
*Tipografía recomendada: Times New Roman 12 pt, interlineado doble, márgenes 2.54 cm*

---

## ÍNDICE

1. [Contexto y Antecedentes del Problema](#1-contexto-y-antecedentes-del-problema)
2. [Descripción y Planteamiento del Problema](#2-descripción-y-planteamiento-del-problema)
3. [Formulación de Objetivos](#3-formulación-de-objetivos)
   - 3.1. [Objetivo General](#31-objetivo-general)
   - 3.2. [Objetivos Específicos](#32-objetivos-específicos)
4. [Desarrollo y Propuesta de Solución](#4-desarrollo-y-propuesta-de-solución)
   - 4.1. [Stack Tecnológico](#41-stack-tecnológico)
   - 4.2. [Arquitectura de Scripts C#](#42-arquitectura-de-scripts-c)
   - 4.3. [Implementación del Sistema de Físicas](#43-implementación-del-sistema-de-físicas)
   - 4.4. [Sistema de Object Pooling](#44-sistema-de-object-pooling)
   - 4.5. [Pipeline de Renderizado y Efectos Visuales](#45-pipeline-de-renderizado-y-efectos-visuales)
   - 4.6. [División de Trabajo y Plan de Desarrollo](#46-división-de-trabajo-y-plan-de-desarrollo)
5. [Marco Teórico y Metodológico](#5-marco-teórico-y-metodológico)
   - 5.1. [Fundamentos de Motores de Videojuegos](#51-fundamentos-de-motores-de-videojuegos)
   - 5.2. [Programación Orientada a Componentes](#52-programación-orientada-a-componentes)
   - 5.3. [Gestión de Memoria y Rendimiento en Tiempo Real](#53-gestión-de-memoria-y-rendimiento-en-tiempo-real)
   - 5.4. [Patrones de Diseño Aplicados](#54-patrones-de-diseño-aplicados)
   - 5.5. [Metodología de Desarrollo](#55-metodología-de-desarrollo)
6. [Conclusiones y Recomendaciones](#6-conclusiones-y-recomendaciones)
   - 6.1. [Conclusiones](#61-conclusiones)
   - 6.2. [Recomendaciones](#62-recomendaciones)
7. [Bibliografía](#7-bibliografía)
8. [Anexos](#8-anexos)
   - Anexo A. [Estructura de Directorios del Proyecto](#anexo-a-estructura-de-directorios-del-proyecto)
   - Anexo B. [Código Fuente Completo — DinoController.cs](#anexo-b-código-fuente-completo--dinocontrollercs)
   - Anexo C. [Código Fuente Completo — ObjectPooler.cs](#anexo-c-código-fuente-completo--objectpoolercs)
   - Anexo D. [Tabla de Roles y Entregables](#anexo-d-tabla-de-roles-y-entregables)
   - Anexo E. [Cronograma de Desarrollo](#anexo-e-cronograma-de-desarrollo)

---

## 1. Contexto y Antecedentes del Problema

La industria del desarrollo de videojuegos ha experimentado un crecimiento sostenido durante la última década, impulsado en gran medida por la democratización de motores gráficos profesionales como Unity y Unreal Engine. Según Newzoo (2023), el mercado global de videojuegos superó los 184 mil millones de dólares, con una tendencia marcada hacia el desarrollo independiente (*indie*) sustentado en herramientas accesibles y ecosistemas de activos compartidos.

En el contexto académico de la Programación Gráfica, la transición desde la programación manual en OpenGL hacia motores de juego de alto nivel representa un salto paradigmático. Mientras que OpenGL exige al programador gestionar manualmente la proyección de vértices, el buffer de profundidad, la rasterización y las transformaciones matriciales, Unity abstrae estas capas mediante un sistema de componentes que permite al desarrollador enfocarse en la lógica de negocio del juego sin sacrificar el control sobre el rendimiento (Gregory, 2019).

El género *endless runner* —corredores infinitos— posee una historia documentada en plataformas móviles y de escritorio. Juegos como *Temple Run* (Imangi Studios, 2011), *Subway Surfers* (Kiloo, 2012) y, en particular, el minijuego del dinosaurio de Google Chrome (*T-Rex Runner*, 2014) demostraron que mecánicas simples de esquiva y reacción, presentadas con estética visual cuidada y progresión de dificultad bien calibrada, generan una experiencia altamente adictiva y técnicamente interesante (Schell, 2020).

Desde la perspectiva técnica, el desafío central de este género reside en la generación procedimental continua del escenario: el mundo debe crecer indefinidamente sin que el consumo de memoria escale proporcionalmente ni se produzcan interrupciones perceptibles en la tasa de fotogramas (*framerate*). Esto llevó a la adopción generalizada del patrón de diseño *Object Pooling*, que evita las costosas operaciones de asignación y liberación dinámica de memoria en tiempo de ejecución (Nystrom, 2014).

El presente proyecto propone abordar estos desafíos técnicos en un entorno universitario de tres días, utilizando Unity 2022.3 LTS con Universal Render Pipeline (URP), C# como lenguaje de scripting, y un equipo de cinco personas con roles especializados.

---

## 2. Descripción y Planteamiento del Problema

### 2.1 Descripción del Problema

El desarrollo de un videojuego de tipo *runner* tridimensional en tiempo real presenta múltiples problemas técnicos simultáneos que deben ser resueltos de forma coordinada:

**Problema de rendimiento en generación continua de objetos.** La instanciación y destrucción repetida de objetos de juego (*GameObjects*) durante la ejecución —obstáculos, segmentos de suelo, efectos de partículas— activa el recolector de basura (*Garbage Collector*) del entorno .NET, produciendo micro-pausas (*stutters*) que degradan la experiencia del jugador. En una implementación ingenua con `Instantiate()` y `Destroy()`, este problema escala con la velocidad del juego.

**Problema de percepción de físicas.** La gravedad predeterminada de Unity aplicada sobre un `Rigidbody` produce un salto parabólico suave que resulta antinatural para un juego de reacción rápida. El jugador percibe el personaje como "flotante", reduciendo la sensación de control y la satisfacción al esquivar obstáculos (Schell, 2020). Se requiere una gravedad aumentada diferenciada entre la fase ascendente y descendente del salto.

**Problema de ilusión de movimiento en un mundo estático.** Desplazar físicamente el personaje a través de un terreno infinito generaría imprecisiones numéricas de punto flotante (*floating point drift*) a grandes distancias del origen de la escena. La solución canónica consiste en mantener el personaje fijo en el espacio mundo y desplazar el escenario hacia el jugador, pero esto requiere un sistema de reciclaje de segmentos que nunca agote el suministro de objetos.

**Problema de coordinación multidisciplinaria en tiempo reducido.** Un desarrollo de tres días con cinco personas sin una arquitectura de módulos clara conduce a conflictos de integración, dependencias circulares entre scripts y trabajo duplicado. La arquitectura debe establecer contratos claros entre sistemas desde el inicio.

### 2.2 Planteamiento del Problema

¿Cómo implementar en Unity un videojuego *runner* 3D con generación procedimental continua, física de salto responsiva y efectos visuales de calidad cinematográfica, optimizando el uso de memoria y asegurando una tasa de fotogramas estable dentro de un ciclo de desarrollo de 72 horas con un equipo de cinco personas?

---

## 3. Formulación de Objetivos

### 3.1 Objetivo General

Diseñar e implementar un videojuego *runner* tridimensional en Unity 2022.3 LTS utilizando C# y Universal Render Pipeline, que integre un sistema de generación procedimental basado en *Object Pooling*, físicas de salto optimizadas y efectos visuales de post-procesado, dentro de un ciclo de desarrollo de tres días con equipo multidisciplinario de cinco personas.

### 3.2 Objetivos Específicos

1. **Implementar el sistema de control del personaje** mediante un `Rigidbody` con gravedad aumentada diferenciada (*fall multiplier*) que produzca una curva de salto responsiva y natural, utilizando detección de suelo basada en `Physics.CheckSphere`.

2. **Desarrollar un sistema de Object Pooling genérico** (`ObjectPooler.cs`) capaz de administrar múltiples grupos de objetos reciclables mediante colas (`Queue<GameObject>`), eliminando las llamadas a `Instantiate` y `Destroy` durante la ejecución del juego.

3. **Construir el sistema de desplazamiento del mundo** (`LevelScroller.cs`) que mantenga al personaje estático en el espacio mundo mientras mueve obstáculos y segmentos de suelo hacia él, con incremento progresivo de velocidad para aumentar la dificultad.

4. **Integrar una máquina de estados del juego** (`GameManager.cs`) que gestione los estados *Menú*, *Jugando* y *GameOver*, el sistema de puntuación con persistencia de récord y la orquestación de eventos entre subsistemas.

5. **Configurar el pipeline visual URP** con efectos de post-procesado (*Bloom*, *Color Grading*, *Motion Blur*) y el sistema de cámara Cinemachine con sacudida (*Camera Shake*) reactiva a eventos críticos del juego.

6. **Establecer una arquitectura modular de scripts** con responsabilidades claramente delimitadas que permita el desarrollo paralelo por cinco personas sin generar conflictos de integración.

---

## 4. Desarrollo y Propuesta de Solución

### 4.1 Stack Tecnológico

El proyecto adopta el siguiente conjunto de tecnologías, seleccionadas por su madurez, documentación y compatibilidad entre sí:

| Componente | Tecnología | Justificación |
|---|---|---|
| Motor de juego | Unity 2022.3 LTS | Versión con soporte extendido, estabilidad probada en producción |
| Lenguaje de programación | C# (.NET Standard 2.1) | Tipado estático, integración nativa con Unity, rendimiento de compilación JIT/IL2CPP |
| Pipeline de renderizado | Universal Render Pipeline (URP) | Balance óptimo entre calidad visual y rendimiento en hardware de gama media |
| Motor de físicas | NVIDIA PhysX (integrado Unity) | Resolución robusta de colisiones 3D, soporte completo de `Rigidbody` |
| Sistema de entrada | Unity Input System (clásico) | Rapidez de implementación con `Input.GetButtonDown` para prototipo en tiempo reducido |
| Animaciones | Unity Animator (Mecanim) | Máquina de estados visual integrada, soporte para blending de animaciones |
| Cámara | Cinemachine 2.x | Movimiento suave con amortiguación, efectos de ruido y sacudida programables |
| Entorno de desarrollo | Visual Studio 2022 / VS Code | IntelliSense completo para APIs de Unity |

**Tabla 1.** *Stack tecnológico del proyecto Runner 3D.*

### 4.2 Arquitectura de Scripts C#

El sistema de scripts sigue el principio de **responsabilidad única** (*Single Responsibility Principle*), donde cada clase gestiona un dominio funcional exclusivo. Las dependencias entre módulos fluyen unidireccionalmente hacia `GameManager`, que actúa como orquestador central.

```
Assets/
└── Scripts/
    ├── DinoController.cs   → Entrada de jugador, salto físico, deslizamiento,
    │                         detección de suelo y triggers de animación.
    ├── LevelScroller.cs    → Desplaza obstáculos y suelo hacia atrás para
    │                         simular velocidad; escala progresiva de dificultad.
    ├── ObjectPooler.cs     → Cola de GameObjects reciclables por etiqueta;
    │                         elimina Instantiate/Destroy en tiempo de ejecución.
    ├── GameManager.cs      → Puntuación, récord, máquina de estados del juego
    │                         (Menú → Jugando → GameOver) y gestión de audio.
    ├── UIManager.cs        → Pantallas de UI, contador de puntos en tiempo real
    │                         y transiciones de desvanecimiento (Fade).
    └── CinemachineShake.cs → Dispara ruido de cámara (Cinemachine Noise)
                              en colisiones críticas para retroalimentación háptica visual.
```

**Figura 1.** *Estructura de módulos y responsabilidades del proyecto.*

La comunicación entre módulos se realiza mediante el patrón **Singleton** en `GameManager` y `ObjectPooler`, accesibles globalmente mediante propiedad estática `Instance`. `DinoController` publica eventos mediante `UnityEvent` o métodos directos a `GameManager` para notificar colisiones.

### 4.3 Implementación del Sistema de Físicas

#### 4.3.1 Problema de la Curva de Salto Predeterminada

Unity aplica gravedad uniforme sobre el `Rigidbody` durante toda la parábola de salto. Esto produce una curva simétrica donde el tiempo de ascenso es igual al tiempo de descenso, resultado percibido como "flotante" por el jugador (Schell, 2020). La solución consiste en aplicar una fuerza gravitacional adicional únicamente durante la fase descendente, acortando el tiempo de caída sin alterar la altura máxima del salto.

La fórmula aplicada en `FixedUpdate` es:

```
F_extra = g × (fallMultiplier - 1) × Δt     [cuando vy < 0]
```

Donde `g` es la gravedad del proyecto (`Physics.gravity.y`), `fallMultiplier` es un coeficiente configurable (valor recomendado: 2.5), y `Δt` es el tiempo fijo de física (`Time.fixedDeltaTime`).

#### 4.3.2 Detección de Suelo

Se utiliza `Physics.CheckSphere` en lugar de `Raycast` simple para mayor robustez en terreno irregular. Un objeto auxiliar `groundCheck` (hijo del personaje, posicionado en los pies) sirve como origen de la esfera de detección con radio 0.2 unidades y máscara de capa exclusiva `groundLayer`.

```csharp
isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
```

Este enfoque elimina falsos negativos al caminar sobre bordes de colisores que un rayo vertical singular podría omitir.

#### 4.3.3 Sistema de Deslizamiento (Crouch)

El deslizamiento reduce temporalmente la altura del `CapsuleCollider` para permitir pasar bajo obstáculos aéreos. Se activa con `LeftShift` o `S`, dispara el parámetro booleano `IsCrouching` en el Animator y puede escalar el collider a la mitad de su altura original mediante `collider.height` y ajuste del `center`.

### 4.4 Sistema de Object Pooling

#### 4.4.1 Fundamento Técnico

El recolector de basura de .NET (GC) opera de forma no determinista: cuando la memoria del heap alcanza un umbral, pausa la ejecución para liberar objetos sin referencias. En un juego a 60 FPS, una pausa de 8 ms supera el presupuesto de fotograma completo (16.67 ms), produciendo caída visible de framerate (Nystrom, 2014).

El patrón *Object Pool* resuelve esto pre-instanciando una cantidad fija de objetos al inicio del juego y recirculándolos mediante activación/desactivación (`SetActive`), evitando así la asignación dinámica durante el juego.

#### 4.4.2 Implementación con `Queue<T>`

`ObjectPooler` mantiene un diccionario de colas indexadas por etiqueta (`string`). Al solicitar un objeto:

1. Se extrae el primero de la cola (`Dequeue`).
2. Se posiciona y activa en escena.
3. Se reinserta al final de la misma cola (`Enqueue`).

Este ciclo garantiza distribución uniforme del uso entre todos los objetos del pool, evitando que un mismo objeto se reutilice antes de que haya completado su recorrido visible.

```
Ciclo de vida: [Inactivo] → Spawn → [Activo en escena] → Sale del borde → [Inactivo]
```

**Figura 2.** *Ciclo de vida de un objeto dentro del pool.*

#### 4.4.3 Configuración de Tamaño del Pool

El tamaño óptimo de cada pool se calcula como:

```
N = (distancia_visible / velocidad_spawn) + margen_seguridad
```

Para una pista visible de 50 unidades, velocidad inicial de 10 u/s y obstáculos cada 8 unidades, se requieren aproximadamente 7 obstáculos simultáneos. Un pool de 10 elementos ofrece margen suficiente.

### 4.5 Pipeline de Renderizado y Efectos Visuales

#### 4.5.1 Universal Render Pipeline (URP)

URP reemplaza el pipeline de renderizado integrado (*Built-in*) de Unity con una arquitectura basada en *Scriptable Render Pipeline* que permite personalizar cada etapa del proceso gráfico. Sus ventajas para este proyecto incluyen (Unity Technologies, 2023):

- **Post-procesado integrado**: Los efectos de *Bloom*, *Color Grading*, *Vignette* y *Motion Blur* se configuran mediante *Volume Profiles* sin código adicional.
- **Iluminación en tiempo real**: Soporte para sombras dinámicas de hasta 4 luces adicionales sin costo prohibitivo de rendimiento en hardware de gama media.
- **Compatibilidad móvil**: El mismo proyecto puede desplegarse en escritorio y dispositivos móviles ajustando parámetros de calidad sin cambios en el código.

#### 4.5.2 Configuración de Post-Procesado

El ambiente visual del juego evoca un desierto al atardecer mediante:

| Efecto | Parámetro | Valor recomendado | Propósito |
|---|---|---|---|
| Bloom | Threshold / Intensity | 0.8 / 1.2 | Resplandor en bordes de obstáculos |
| Color Grading | Temperature / Saturation | +15 / +10 | Tonalidad cálida de atardecer |
| Vignette | Intensity | 0.35 | Enfoque visual hacia el centro |
| Motion Blur | Shutter Angle | 180° | Sensación de velocidad elevada |

**Tabla 2.** *Configuración de efectos de post-procesado URP.*

#### 4.5.3 Sistema de Cámara con Cinemachine

`CinemachineVirtualCamera` sigue al personaje con amortiguación configurable en los tres ejes. `CinemachineShake.cs` accede al `CinemachineBasicMultiChannelPerlin` del componente de ruido de la cámara virtual y modula su amplitud temporalmente al detectar una colisión crítica, produciendo sacudida de pantalla sin alterar la posición real de la cámara.

```csharp
IEnumerator ShakeRoutine(float duration, float magnitude)
{
    noise.m_AmplitudeGain = magnitude;
    yield return new WaitForSeconds(duration);
    noise.m_AmplitudeGain = 0f;
}
```

### 4.6 División de Trabajo y Plan de Desarrollo

#### 4.6.1 Distribución de Roles

El equipo de cinco personas se distribuye con especialización funcional que minimiza dependencias entre ramas de trabajo simultáneo:

| Persona | Rol | Responsabilidad Principal | Entregables |
|---|---|---|---|
| P1 | Jugabilidad y Físicas | Lógica de entrada e interacción del personaje | `DinoController.cs`, calibración de salto y agachado, ajuste de gravedad |
| P2 | Mundo Infinito | Reciclaje y desplazamiento del escenario | `ObjectPooler.cs`, `LevelScroller.cs`, spawn aleatorio de obstáculos |
| P3 | GameManager e Integración | Control de estados, puntuación e interfaces | `GameManager.cs`, `UIManager.cs`, sistema de audio, pantallas de menú y GameOver |
| P4 | Arte 3D e Iluminación | Diseño visual del entorno y efectos de partículas | Modelos 3D, materiales URP, VFX de polvo y colisión |
| P5 | Animación y Cámara | Animaciones del personaje y comportamiento de cámara | Configuración del Animator, transiciones de animación, Cinemachine y Camera Shake |

**Tabla 3.** *División de roles y responsabilidades del equipo.*

#### 4.6.2 Plan de Desarrollo en Tres Días

**Día 1 — Mecánicas Base (Prototipo Funcional)**

- Escena base con plano de colisión y personaje con `Rigidbody`.
- `DinoController.cs` con salto, gravedad aumentada y detección de suelo.
- Obstáculos básicos (cubos) moviéndose hacia el jugador sin pooling.
- Detección de colisión con Game Over inmediato.
- *Criterio de aceptación*: El personaje salta, cae con física pesada y el juego termina al colisionar.

**Día 2 — Reciclador de Mundo y Animación**

- `ObjectPooler.cs` y `LevelScroller.cs` integrados; eliminación total de `Instantiate` en runtime.
- Importación de modelos 3D del personaje y obstáculos; configuración del Animator (estados: Idle, Run, Jump, Crouch).
- `GameManager.cs` con máquina de estados, puntuación incremental por tiempo y pantalla de Game Over.
- *Criterio de aceptación*: El juego corre sin stutters observables a 60 FPS en hardware objetivo; animaciones reproducen correctamente según estado.

**Día 3 — Pulido Visual, Audio y Calibración**

- Configuración de URP Volume con Bloom, Color Grading y Motion Blur.
- Sistema de partículas para polvo en los pies del personaje al correr.
- Integración de Cinemachine con Camera Shake en colisiones.
- Audio: música de fondo en bucle, efectos de salto, aterrizaje y Game Over.
- Calibración de dificultad mediante curva de velocidad progresiva (`Mathf.Lerp`).
- *Criterio de aceptación*: Sesión de juego completa reproducible de forma autónoma ante público de feria.

---

## 5. Marco Teórico y Metodológico

### 5.1 Fundamentos de Motores de Videojuegos

Un motor de videojuego (*game engine*) es un framework de software que provee los subsistemas fundamentales necesarios para el desarrollo de juegos: renderizado, física, gestión de escenas, entrada de usuario, audio y red (Gregory, 2019). Unity opera sobre una arquitectura de **Entidad-Componente** (*Entity-Component*) donde los objetos de juego (`GameObject`) son contenedores inertes que adquieren comportamiento mediante la composición de componentes (`MonoBehaviour`, `Rigidbody`, `Collider`, etc.).

Esta arquitectura contrasta con la herencia orientada a objetos tradicional: en lugar de una jerarquía profunda de clases `Enemigo → EnemigoCorredor → EnemigoCorredor3D`, Unity favorece la composición de componentes especializados sobre un objeto base genérico. Esto reduce el acoplamiento y facilita la reutilización de comportamientos entre objetos heterogéneos (Nystrom, 2014).

### 5.2 Programación Orientada a Componentes

El patrón **Componente** (*Component Pattern*) permite agregar comportamiento a objetos de juego sin necesidad de subclasificación. Según Nystrom (2014), este patrón resuelve el problema del *yo-yo de herencia* (*inheritance yo-yo*), donde el código de una clase concreta requiere navegar una cadena profunda de clases padre para comprenderse.

En Unity, `MonoBehaviour` es la clase base de todos los scripts de componente. Sus métodos de ciclo de vida (`Awake`, `Start`, `Update`, `FixedUpdate`, `OnCollisionEnter`) son invocados automáticamente por el motor en momentos determinados del loop de juego, abstrayendo el bucle principal al desarrollador.

El ciclo de actualización de Unity sigue el orden:
1. `FixedUpdate` — física (50 Hz por defecto, tiempo fijo).
2. `Update` — lógica de juego (frecuencia de framerate, tiempo variable).
3. `LateUpdate` — operaciones post-actualización como seguimiento de cámara.

### 5.3 Gestión de Memoria y Rendimiento en Tiempo Real

Los juegos de tiempo real operan bajo presupuestos de tiempo por fotograma estrictos. A 60 FPS, cada fotograma tiene un presupuesto máximo de 16.67 ms. A 30 FPS, 33.33 ms. Cualquier operación que exceda este presupuesto produce caída visible de framerate (Unity Technologies, 2023).

El recolector de basura de C# (.NET) es el principal responsable de pausas no deterministas. Cada llamada a `new` en C# puede producir asignación en el heap administrado; cuando el GC decide recolectar, pausa todos los hilos de ejecución por un tiempo proporcional al volumen de basura acumulada.

Las estrategias principales para minimizar la presión sobre el GC en juegos Unity incluyen (Gregory, 2019):

- **Object Pooling**: Pre-asignar objetos al inicio y reutilizarlos.
- **Evitar LINQ en Update**: Las expresiones LINQ generan iteradores anónimos que producen asignaciones heap.
- **Caché de referencias de componentes**: Almacenar `GetComponent<T>()` en `Start` en lugar de llamarlo cada frame.
- **`StringBuilder` para UI de texto**: Evitar concatenación de strings que genera garbage temporal.

### 5.4 Patrones de Diseño Aplicados

#### 5.4.1 Singleton

`GameManager` y `ObjectPooler` implementan el patrón Singleton para garantizar una única instancia accesible globalmente. En Unity se implementa mediante propiedad estática `Instance` asignada en `Awake`:

```csharp
public static GameManager Instance { get; private set; }
void Awake() { Instance = this; }
```

Este patrón es conveniente para sistemas centrales de acceso frecuente, aunque debe usarse con moderación para no crear acoplamiento global excesivo (Nystrom, 2014).

#### 5.4.2 Object Pool

Descrito en la sección 4.4, este patrón gestiona un conjunto finito de objetos reciclables para eliminar la presión de asignación dinámica durante el juego.

#### 5.4.3 State Machine (Máquina de Estados)

`GameManager` implementa una máquina de estados finitos con tres estados: `Menu`, `Playing` y `GameOver`. Las transiciones entre estados son explícitas y controladas, lo que previene comportamientos indefinidos cuando múltiples sistemas intentan modificar el estado del juego concurrentemente.

#### 5.4.4 Observer (Eventos de Unity)

`UnityEvent` y el sistema de eventos de C# (`event Action`) permiten que `DinoController` notifique a `GameManager` sobre colisiones sin dependencia directa del módulo de físicas al módulo de gestión de juego. Esto mantiene bajo acoplamiento entre subsistemas.

### 5.5 Metodología de Desarrollo

El proyecto adopta un enfoque **iterativo e incremental** compatible con la restricción temporal de 72 horas, dividido en tres iteraciones de un día:

- **Iteración 1 (Día 1)**: Prototipo funcional (*Minimum Viable Product*). Validación de mecánicas core antes de invertir en arte y efectos.
- **Iteración 2 (Día 2)**: Integración de sistemas complejos (pooling, animación, UI). Verificación de rendimiento.
- **Iteración 3 (Día 3)**: Pulido visual y calibración de experiencia. Preparación para demostración pública.

Esta estructura es coherente con principios del desarrollo ágil de software, específicamente con el concepto de *timeboxing* y *sprint* (Beck et al., 2001), adaptado al contexto de desarrollo de juegos donde cada iteración produce un artefacto jugable.

La coordinación entre personas se establece mediante contrato de interfaces: cada módulo define sus dependencias públicas desde el primer día, permitiendo que personas trabajen en paralelo sobre subsistemas diferentes sin bloqueos de integración.

---

## 6. Conclusiones y Recomendaciones

### 6.1 Conclusiones

**C1.** La transición desde programación gráfica manual en OpenGL hacia un motor como Unity representa un cambio cualitativo en el nivel de abstracción: mientras OpenGL requiere al programador gestionar explícitamente vértices, shaders y transformaciones matriciales, Unity delega estas responsabilidades al motor, permitiendo enfocarse en la lógica de experiencia del juego. Esta transición no elimina la complejidad técnica, sino que la reubica en dominios como la arquitectura de scripts, la gestión de memoria y la optimización de pipelines de renderizado.

**C2.** El patrón *Object Pooling* resulta indispensable en juegos *runner* de generación continua. La eliminación de llamadas a `Instantiate` y `Destroy` durante el runtime elimina la fuente principal de presión sobre el recolector de basura de .NET, garantizando una tasa de fotogramas estable incluso a velocidades elevadas de juego.

**C3.** La física de salto personalizada mediante *fall multiplier* demuestra que la fidelidad física no siempre coincide con la percepción de responsividad deseada por el jugador. La manipulación deliberada de la gravedad en función del vector de velocidad del personaje es una técnica ampliamente utilizada en la industria que produce una sensación de control superior a la simulación física realista.

**C4.** La división de roles con fronteras funcionales claras —física, mundo infinito, gestión de juego, arte visual, animación y cámara— permitió el desarrollo paralelo efectivo en el tiempo disponible. La arquitectura Singleton en los sistemas centrales proveyó los puntos de acceso globales necesarios sin generar bloqueos de integración.

**C5.** El Universal Render Pipeline de Unity demuestra que efectos visuales de calidad cinematográfica (Bloom, Motion Blur, Camera Shake) son accesibles sin conocimiento avanzado de shaders, democratizando la producción visual de alta calidad en proyectos académicos con tiempo reducido.

### 6.2 Recomendaciones

**R1.** Para futuros proyectos similares, se recomienda establecer la arquitectura de scripts y los contratos de interfaz entre módulos en las primeras dos horas del primer día, antes de comenzar cualquier implementación. Esto previene el principal riesgo de integración en equipos pequeños con tiempo limitado.

**R2.** Explorar el paquete `Unity.Collections` (NativeArray, NativeList) para sistemas de alta frecuencia de actualización. Estos tipos de datos se alocan en memoria nativa (fuera del heap administrado de .NET), eliminando completamente la presión de GC en los caminos críticos de rendimiento.

**R3.** Considerar la migración del sistema de entrada a *New Input System* de Unity para proyectos con soporte multi-plataforma. Aunque el sistema clásico (`Input.GetKeyDown`) es suficiente para prototipo, el nuevo sistema ofrece abstracción de dispositivos que facilita el soporte de mandos de juego y pantallas táctiles sin cambios de código.

**R4.** Incorporar un sistema de pruebas automatizadas para la lógica de `GameManager` utilizando el framework `Unity Test Framework` (modo Edit Mode). Validar automáticamente las transiciones de estado previene regresiones durante el pulido del Día 3 cuando múltiples sistemas se integran simultáneamente.

**R5.** Para extensión futura del proyecto, el sistema de dificultad progresiva puede enriquecerse con una curva de animación (`AnimationCurve`) que defina la relación velocidad-tiempo de juego de forma no lineal, ofreciendo mayor control sobre la experiencia del jugador que una interpolación lineal con `Mathf.Lerp`.

**R6.** Documentar en el código fuente (mediante atributos `[Header]` y `[Tooltip]` de Unity) todos los parámetros serializados del Inspector. Esta práctica reduce el costo de orientación de nuevos miembros del equipo y facilita el ajuste de valores durante las sesiones de prueba.

---

## 7. Bibliografía

*Referencias en formato APA 7.ª edición, ordenadas alfabéticamente.*

---

Beck, K., Beedle, M., van Bennekum, A., Cockburn, A., Cunningham, W., Fowler, M., Grenning, J., Highsmith, J., Hunt, A., Jeffries, R., Kern, J., Marick, B., Martin, R. C., Mellor, S., Schwaber, K., Sutherland, J., & Thomas, D. (2001). *Manifesto for agile software development*. Agile Alliance. https://agilemanifesto.org

Gregory, J. (2019). *Game engine architecture* (3.ª ed.). CRC Press.

Imangi Studios. (2011). *Temple Run* [Videojuego]. Imangi Studios.

Kiloo & Sybo Games. (2012). *Subway surfers* [Videojuego]. Kiloo.

Newzoo. (2023). *Global games market report 2023*. Newzoo BV. https://newzoo.com/resources/trend-reports/newzoos-global-games-market-report-2023

Nystrom, R. (2014). *Game programming patterns*. Genever Benning. https://gameprogrammingpatterns.com

Schell, J. (2020). *The art of game design: A book of lenses* (3.ª ed.). CRC Press.

Unity Technologies. (2023). *Universal render pipeline documentation* (Unity 2022.3 LTS). Unity Technologies. https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest

Unity Technologies. (2023). *Unity scripting API: MonoBehaviour* (Unity 2022.3 LTS). Unity Technologies. https://docs.unity3d.com/ScriptReference/MonoBehaviour.html

Unity Technologies. (2023). *Unity manual: Understanding optimization in Unity*. Unity Technologies. https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity.html

---

## 8. Anexos

### Anexo A. Estructura de Directorios del Proyecto

```
Assets/
├── Scripts/
│   ├── DinoController.cs
│   ├── LevelScroller.cs
│   ├── ObjectPooler.cs
│   ├── GameManager.cs
│   ├── UIManager.cs
│   └── CinemachineShake.cs
├── Prefabs/
│   ├── Player/
│   │   └── Dino.prefab
│   ├── Obstacles/
│   │   ├── Cactus_Small.prefab
│   │   ├── Cactus_Large.prefab
│   │   └── RockFormation.prefab
│   └── Environment/
│       ├── GroundSegment.prefab
│       └── BackgroundDune.prefab
├── Models/
│   ├── Dino/
│   └── Environment/
├── Animations/
│   ├── DinoAnimatorController.controller
│   ├── Idle.anim
│   ├── Run.anim
│   ├── Jump.anim
│   └── Crouch.anim
├── Materials/
│   ├── Desert_Ground.mat
│   └── Dino_Body.mat
├── Scenes/
│   ├── MainMenu.unity
│   └── GameScene.unity
├── Audio/
│   ├── BGM_Desert.wav
│   ├── SFX_Jump.wav
│   ├── SFX_Land.wav
│   └── SFX_GameOver.wav
└── Settings/
    └── URP_GameVolume.asset
```

*Nota.* La estructura sigue las convenciones de nomenclatura de Unity y separa activos por tipo funcional para facilitar la navegación en proyectos de equipo.

---

### Anexo B. Código Fuente Completo — DinoController.cs

```csharp
using UnityEngine;

/// <summary>
/// Controla la entrada del jugador, la física del salto y el deslizamiento.
/// Requiere: Rigidbody, Animator, Transform (groundCheck como hijo).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DinoController : MonoBehaviour
{
    [Header("Físicas de Salto")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float fallMultiplier = 2.5f;
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
        // Detección de suelo mediante esfera física
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            animator.SetTrigger("Jump");
        }

        // Deslizamiento
        bool crouching = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.S);
        animator.SetBool("IsCrouching", crouching);
    }

    void FixedUpdate()
    {
        // Gravedad aumentada en caída para salto responsivo
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y
                           * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.TriggerGameOver();
        }
    }
}
```

*Nota.* El script requiere un objeto hijo vacío denominado `GroundCheck` posicionado en la base del collider del personaje. La capa `groundLayer` debe configurarse en el Inspector excluyendo la capa del propio personaje para evitar auto-detección.

---

### Anexo C. Código Fuente Completo — ObjectPooler.cs

```csharp
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool genérico de GameObjects indexado por etiqueta.
/// Elimina Instantiate/Destroy en runtime; pre-instancia al inicio.
/// </summary>
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

    /// <summary>
    /// Extrae un objeto del pool, lo posiciona y activa.
    /// Al llegar al borde de la pista, el objeto debe llamar ReturnToPool.
    /// </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool con tag '{tag}' no encontrado.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
```

*Nota.* Los objetos del pool no se destruyen al salir de la pista; en su lugar, el componente `LevelScroller.cs` los desactiva (`SetActive(false)`) cuando su coordenada X supera el umbral de reciclaje. El `Enqueue` al final de `SpawnFromPool` re-inserta el objeto en la cola para su próximo uso.

---

### Anexo D. Tabla de Roles y Entregables

| Persona | Rol | Scripts bajo responsabilidad | Dependencias externas |
|---|---|---|---|
| P1 | Jugabilidad y Físicas | `DinoController.cs` | `GameManager.Instance` (notificación de colisión) |
| P2 | Mundo Infinito | `ObjectPooler.cs`, `LevelScroller.cs` | Prefabs de P4, parámetros de velocidad de P3 |
| P3 | GameManager e Integración | `GameManager.cs`, `UIManager.cs` | Eventos de P1, velocidad del mundo de P2 |
| P4 | Arte 3D e Iluminación | Materiales, VFX Graph, Prefabs | Modelos de personaje de P5 para VFX de partículas |
| P5 | Animación y Cámara | `CinemachineShake.cs`, Animator Controller | Triggers de animación definidos con P1 |

**Tabla A1.** *Roles, scripts y dependencias inter-equipo.*

---

### Anexo E. Cronograma de Desarrollo

| Fase | Día | Actividad | Responsable | Criterio de Completitud |
|---|---|---|---|---|
| Prototipo | 1 — Mañana | Configuración de proyecto Unity, escena base, `Rigidbody` sobre plano | P1, P3 | Proyecto compila sin errores |
| Prototipo | 1 — Tarde | `DinoController.cs` con salto y detección de suelo | P1 | Personaje salta y cae con gravedad pesada |
| Prototipo | 1 — Tarde | Obstáculos básicos moviéndose, colisión → Game Over | P2, P3 | Colisión termina la sesión |
| Integración | 2 — Mañana | `ObjectPooler.cs` + `LevelScroller.cs` funcionales | P2 | Sin llamadas a `Instantiate` en runtime; profiler GC limpio |
| Integración | 2 — Tarde | Importación de modelos, configuración de Animator | P4, P5 | Animaciones Idle/Run/Jump/Crouch reproducen correctamente |
| Integración | 2 — Tarde | `GameManager.cs` con máquina de estados y puntuación | P3 | Ciclo Menú → Jugando → GameOver → Menú funcional |
| Pulido | 3 — Mañana | URP Volume (Bloom, Color Grading, Motion Blur) | P4 | Efectos visibles en la escena de juego |
| Pulido | 3 — Mañana | VFX de partículas de polvo y Cinemachine Shake | P4, P5 | Polvo visible al correr; sacudida en colisión |
| Pulido | 3 — Tarde | Audio: BGM + SFX de salto, aterrizaje y GameOver | P3 | Audio reproducible en todas las transiciones |
| Pulido | 3 — Tarde | Calibración de dificultad y prueba completa de sesión | Todos | Sesión jugable de inicio a fin sin errores críticos |

**Tabla A2.** *Cronograma detallado de desarrollo en tres días.*

---

*Fin del documento.*

*Proyecto desarrollado para Feria de Exposición Académica — Programación Gráfica.*
*Formato: APA 7.ª edición. Para versión impresa, aplicar Times New Roman 12 pt, interlineado 2.0, márgenes 2.54 cm en todos los lados.*
