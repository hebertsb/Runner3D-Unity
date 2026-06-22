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
   - 4.1. [Herramientas y Tecnologías Utilizadas](#41-herramientas-y-tecnologías-utilizadas)
   - 4.2. [Cómo se Organizó el Código](#42-cómo-se-organizó-el-código)
   - 4.3. [El Sistema de Físicas del Personaje](#43-el-sistema-de-físicas-del-personaje)
   - 4.4. [El Mundo Infinito: Reciclaje de Objetos](#44-el-mundo-infinito-reciclaje-de-objetos)
   - 4.5. [Ilusión de Movimiento: El Truco del Mundo que se Mueve](#45-ilusión-de-movimiento-el-truco-del-mundo-que-se-mueve)
   - 4.6. [Aspecto Visual: Universal Render Pipeline](#46-aspecto-visual-universal-render-pipeline)
   - 4.7. [Desafíos Técnicos Encontrados y Cómo se Resolvieron](#47-desafíos-técnicos-encontrados-y-cómo-se-resolvieron)
   - 4.8. [Organización de la Escena y Estructura del Proyecto](#48-organización-de-la-escena-y-estructura-del-proyecto)
   - 4.9. [Control de Versiones con Git y GitHub](#49-control-de-versiones-con-git-y-github)
   - 4.10. [División de Trabajo y Plan de Desarrollo](#410-división-de-trabajo-y-plan-de-desarrollo)
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

El presente proyecto propone abordar estos desafíos técnicos en un entorno universitario de tres días, utilizando Unity 6.5 (6000.5.0f1) con Universal Render Pipeline (URP), C# como lenguaje de scripting, y un equipo de cinco personas con roles especializados.

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

Diseñar e implementar un videojuego *runner* tridimensional en Unity 6.5 utilizando C# y Universal Render Pipeline, que integre un sistema de generación procedimental basado en *Object Pooling*, físicas de salto optimizadas y efectos visuales de post-procesado, dentro de un ciclo de desarrollo de tres días con equipo multidisciplinario de cinco personas.

### 3.2 Objetivos Específicos

1. **Implementar el sistema de control del personaje** mediante un `Rigidbody` con gravedad aumentada diferenciada (*fall multiplier*) que produzca una curva de salto responsiva y natural, utilizando detección de suelo basada en `Physics.CheckSphere`.

2. **Desarrollar un sistema de Object Pooling genérico** (`ObjectPooler.cs`) capaz de administrar múltiples grupos de objetos reciclables mediante colas (`Queue<GameObject>`), eliminando las llamadas a `Instantiate` y `Destroy` durante la ejecución del juego.

3. **Construir el sistema de desplazamiento del mundo** (`LevelScroller.cs`) que mantenga al personaje estático en el espacio mundo mientras mueve obstáculos y segmentos de suelo hacia él, con incremento progresivo de velocidad para aumentar la dificultad.

4. **Integrar una máquina de estados del juego** (`GameManager.cs`) que gestione los estados *Menú*, *Jugando* y *GameOver*, el sistema de puntuación con persistencia de récord y la orquestación de eventos entre subsistemas.

5. **Configurar el pipeline visual URP** con efectos de post-procesado (*Bloom*, *Color Grading*, *Motion Blur*) y el sistema de cámara Cinemachine con sacudida (*Camera Shake*) reactiva a eventos críticos del juego.

6. **Establecer una arquitectura modular de scripts** con responsabilidades claramente delimitadas que permita el desarrollo paralelo por cinco personas sin generar conflictos de integración.

---

## 4. Desarrollo y Propuesta de Solución

### 4.1 Herramientas y Tecnologías Utilizadas

Para llevar adelante este proyecto se eligió Unity 6.5 como motor de desarrollo, principalmente porque ofrece todo lo necesario para un juego 3D de calidad sin requerir configuraciones complejas desde cero. Unity es ampliamente utilizado tanto en la industria como en el ámbito académico, y su versión 6 incorpora mejoras importantes en rendimiento y en el sistema de renderizado visual.

El lenguaje de programación utilizado es C#, que es el lenguaje oficial de scripting en Unity. C# permite escribir código claro y organizado, con un sistema de tipos estricto que ayuda a detectar errores antes de ejecutar el juego. Para el apartado visual se utilizó el Universal Render Pipeline (URP), que es el pipeline de renderizado moderno de Unity. Su principal ventaja para este proyecto es que incluye efectos visuales de postprocesado —como destellos de luz, desenfoque de movimiento y corrección de color— listos para usar sin necesidad de programar shaders desde cero.

Una decisión importante durante el desarrollo fue el sistema de entrada de teclado. Unity 6 utiliza por defecto el New Input System, que es más moderno y compatible con múltiples dispositivos. Sin embargo, esto generó una incompatibilidad con código de versiones anteriores de Unity que usaba la clase `Input` clásica. La solución aplicada fue migrar completamente al nuevo sistema, usando `Keyboard.current` para detectar las pulsaciones del jugador. Este cambio, aunque requirió ajustar el código, resultó en un sistema de entrada más robusto y preparado para el futuro.

| Componente | Tecnología elegida | Por qué se eligió |
|---|---|---|
| Motor de juego | Unity 6.5 (6000.5.0f1) | Motor estándar de la industria, documentación abundante, gratuito para proyectos académicos |
| Lenguaje de programación | C# (.NET) | Lenguaje oficial de Unity, tipado estático, fácil de leer y mantener |
| Pipeline de renderizado | Universal Render Pipeline (URP) | Efectos visuales modernos sin necesidad de programar shaders |
| Motor de físicas | NVIDIA PhysX (integrado en Unity) | Resolución de colisiones 3D incluida, no requiere implementación manual |
| Sistema de entrada | New Input System (Unity 6) | Nativo en Unity 6, compatible con teclado, gamepad y pantalla táctil |
| Animaciones | Unity Animator (Mecanim) | Editor visual de estados de animación, integrado en el motor |
| Cámara | Cinemachine | Seguimiento suave del personaje y efectos de vibración programables |
| Editor de código | Visual Studio Code | Autocompletado inteligente para las APIs de Unity |
| Control de versiones | Git + GitHub | Trabajo colaborativo entre los cinco integrantes del equipo |

**Tabla 1.** *Herramientas y tecnologías del proyecto Runner 3D.*

### 4.2 Cómo se Organizó el Código

Uno de los primeros desafíos en cualquier proyecto de videojuego en equipo es decidir cómo dividir el código para que varias personas puedan trabajar al mismo tiempo sin pisarse entre sí. La solución adoptada fue asignar a cada script una sola responsabilidad clara y bien definida. De esta manera, la persona que trabaja en las físicas del personaje no necesita tocar el código del sistema de puntuación, y quien trabaja en la interfaz no interfiere con el código de los obstáculos.

El proyecto se estructura en seis scripts principales, cada uno con una función específica:

```
Assets/Scripts/
├── DinoController.cs   → Todo lo relacionado con el personaje: salto,
│                         detección de suelo, agacharse y colisiones.
├── LevelScroller.cs    → Mueve los obstáculos hacia el jugador para
│                         crear la ilusión de que el personaje avanza.
├── ObjectPooler.cs     → Administra un grupo de obstáculos reutilizables
│                         para evitar pausas por memoria.
├── GameManager.cs      → Controla el estado del juego (menú, jugando,
│                         game over), la puntuación y el récord.
├── UIManager.cs        → Maneja las pantallas visibles: marcador,
│                         pantalla de inicio y pantalla de fin de juego.
└── CinemachineShake.cs → Hace temblar la cámara cuando el personaje
                          choca, dando retroalimentación visual al jugador.
```

**Figura 1.** *Organización de scripts y su función en el proyecto.*

Para que estos scripts puedan comunicarse entre sí sin crear dependencias complicadas, los sistemas centrales (`GameManager` y `ObjectPooler`) se implementan como instancias únicas accesibles desde cualquier parte del código. Cuando el personaje choca con un obstáculo, `DinoController` le avisa a `GameManager` con una sola línea de código, y el GameManager se encarga del resto.

### 4.3 El Sistema de Físicas del Personaje

#### 4.3.1 El Problema del Salto Flotante

Cuando se usa la física predeterminada de Unity para hacer saltar un personaje, el resultado suele sentirse poco natural. El personaje sube y baja a la misma velocidad, lo que produce una sensación de que está flotando en el aire. Este problema es muy conocido en el desarrollo de videojuegos y afecta negativamente la experiencia del jugador, que pierde sensación de control (Schell, 2020).

La solución consiste en aplicar una fuerza de gravedad extra únicamente durante la caída, sin afectar la subida. De esta manera el personaje sube con normalidad pero cae más rápido, produciendo un salto que se siente más firme y responsivo. El coeficiente que controla qué tan intensa es esta caída extra se llama `multiplicadorCaida` y está configurado en 2.5, lo que significa que el personaje cae 2.5 veces más fuerte de lo que sube. Este valor fue calibrado durante las pruebas del prototipo para encontrar el punto donde el salto se siente natural.

```
Gravedad extra en caída = gravedad × (multiplicadorCaida - 1) × tiempo
```

Esta operación se ejecuta en el método `FixedUpdate`, que corre a una frecuencia fija de 50 veces por segundo independientemente de la tasa de fotogramas, garantizando que la física sea consistente en cualquier computadora.

#### 4.3.2 Detección de Suelo

Para que el personaje solo pueda saltar cuando está pisando el suelo, se necesita un mecanismo que detecte si hay terreno debajo de sus pies. La solución implementada usa un objeto vacío llamado `DetectorSuelo`, colocado justo en los pies del personaje, que emite una pequeña esfera invisible hacia abajo. Si esa esfera toca un objeto marcado como suelo, el sistema sabe que el personaje puede saltar.

```csharp
estaEnSuelo = Physics.CheckSphere(puntoSuelo.position, 0.2f, capaSuelo);
```

Esta técnica es más confiable que usar un simple rayo porque cubre un área pequeña en lugar de un punto exacto, evitando situaciones donde el personaje está justo en el borde de una superficie y el rayo no lo detecta. El suelo del juego está marcado con una capa llamada `Ground` para que el detector no confunda el suelo con otros objetos de la escena como los obstáculos.

#### 4.3.3 El Sistema de Entrada del Jugador

Como se mencionó en la sección 4.1, Unity 6 utiliza el New Input System, que requiere una forma distinta de leer las teclas presionadas. En lugar de `Input.GetButtonDown("Jump")`, el código usa `Keyboard.current.spaceKey.wasPressedThisFrame`, que es más explícito y no depende de configuraciones de teclas predefinidas. El juego acepta tres teclas diferentes para saltar —Espacio, W y flecha arriba— para que sea cómodo para distintos jugadores:

```csharp
bool salto = Keyboard.current.spaceKey.wasPressedThisFrame ||
             Keyboard.current.wKey.wasPressedThisFrame ||
             Keyboard.current.upArrowKey.wasPressedThisFrame;
```

#### 4.3.4 Restricciones de Rotación

Un detalle importante que no es evidente a primera vista: cuando el personaje choca con un obstáculo, la física de Unity podría hacer que se voltee o gire de maneras inesperadas, lo que rompería visualmente el juego. Para evitarlo, el componente `Rigidbody` del personaje tiene bloqueada la rotación en los tres ejes (X, Y y Z). Así, sin importar la fuerza del choque, el personaje siempre se mantiene erguido.

### 4.4 El Mundo Infinito: Reciclaje de Objetos

#### 4.4.1 Por Qué No se Puede Crear y Destruir Obstáculos Continuamente

La primera idea para generar obstáculos de forma continua sería crear uno nuevo cada vez que se necesita y destruirlo cuando sale de la pantalla. Sin embargo, en un juego en tiempo real esto genera un problema serio: cada vez que C# crea o destruye un objeto en memoria, el sistema puede pausar brevemente la ejecución para reorganizar esa memoria. Estas pausas son muy cortas —a veces apenas unos milisegundos— pero en un juego donde todo ocurre a 60 fotogramas por segundo, incluso 8 ms de pausa es suficiente para que el jugador note una pequeña interrupción (Nystrom, 2014).

#### 4.4.2 La Solución: Reutilizar en Lugar de Crear

El patrón *Object Pooling* resuelve este problema de una forma elegante: en lugar de crear obstáculos durante el juego, se crean todos al inicio y se guardan desactivados. Cuando el juego necesita un obstáculo nuevo, simplemente toma uno de los guardados, lo posiciona donde corresponde y lo activa. Cuando ese obstáculo sale de la pantalla, en lugar de destruirlo se desactiva nuevamente y vuelve a la reserva para usarlo otra vez.

```
Estado del obstáculo:
[Desactivado en reserva] → se necesita → [Activo en pantalla] → sale del borde → [Desactivado en reserva]
```

**Figura 2.** *Ciclo de vida de un obstáculo dentro del sistema de pool.*

La implementación usa una cola (`Queue`) para cada tipo de objeto, lo que garantiza que los obstáculos se reutilicen en orden justo y ninguno sea usado dos veces seguidas antes de que haya completado su recorrido.

#### 4.4.3 Cuántos Objetos Pre-crear

Para determinar cuántos obstáculos pre-crear al inicio, se calcula cuántos pueden estar visibles en pantalla al mismo tiempo. Si la pista visible mide 50 unidades y los obstáculos aparecen cada 8 unidades aproximadamente, el máximo simultáneo es alrededor de 7. Pre-crear 10 objetos por tipo ofrece un margen cómodo sin desperdiciar memoria.

### 4.5 Ilusión de Movimiento: El Truco del Mundo que se Mueve

Una de las decisiones más importantes del diseño técnico es que el personaje no se mueve realmente hacia adelante. En cambio, los obstáculos y el suelo se desplazan hacia él. Esto resuelve un problema matemático: cuando un objeto en Unity se aleja demasiado del centro de coordenadas (el punto 0,0,0), los cálculos de física y posición pueden acumular pequeños errores numéricos que con el tiempo producen comportamientos extraños. Manteniendo el personaje cerca del origen y moviendo el escenario, el juego puede durar indefinidamente sin este problema.

El script `LevelScroller.cs` implementa este movimiento con solo unas líneas:

```csharp
void Update()
{
    transform.Translate(Vector3.back * velocidad * Time.deltaTime);
}
```

Al multiplicar por `Time.deltaTime` —el tiempo transcurrido desde el último fotograma— el movimiento es consistente independientemente de la velocidad del procesador. Un computador lento y uno rápido verán los obstáculos moverse a la misma velocidad real, solo que el rápido lo renderizará con más fluidez.

La dificultad aumenta progresivamente incrementando esta velocidad con el tiempo, usando interpolación lineal (`Mathf.Lerp`) para que el aumento sea gradual y no abrupto.

### 4.6 Aspecto Visual: Universal Render Pipeline

#### 4.6.1 Qué es URP y por qué se usó

El Universal Render Pipeline (URP) es el sistema que controla cómo Unity convierte la escena 3D en imágenes que se ven en pantalla. Comparado con el pipeline clásico de Unity, URP produce imágenes de mayor calidad visual con mejor rendimiento, y viene con un conjunto de efectos visuales de postprocesado ya integrados que se configuran visualmente sin necesidad de escribir código de shaders (Unity Technologies, 2023).

Para este proyecto, URP permite que el ambiente del desierto tenga un aspecto atractivo mediante efectos que se configuran directamente desde el editor:

| Efecto visual | Para qué sirve en el juego |
|---|---|
| Bloom (destellos) | Hace que los bordes de los obstáculos brillen ligeramente |
| Color Grading | Da tonos cálidos anaranjados que evocan un atardecer en el desierto |
| Vignette | Oscurece los bordes de la pantalla para enfocar la mirada al centro |
| Motion Blur | A velocidades altas, crea un ligero desenfoque que da sensación de rapidez |

**Tabla 2.** *Efectos visuales y su propósito en la experiencia del jugador.*

#### 4.6.2 La Cámara con Cinemachine

En lugar de posicionar la cámara manualmente y programar su seguimiento, el proyecto usa Cinemachine, un sistema de cámara avanzado que viene incluido en Unity. Cinemachine sigue al personaje con un movimiento suavizado y amortiguado, eliminando los saltos bruscos. Además, cuando el personaje choca con un obstáculo, el script `CinemachineShake.cs` hace vibrar la cámara durante una fracción de segundo, dando al jugador una retroalimentación visual clara del impacto sin necesidad de sonido ni texto en pantalla.

### 4.7 Desafíos Técnicos Encontrados y Cómo se Resolvieron

Durante el proceso de configuración y desarrollo del proyecto se encontraron varios problemas técnicos que no estaban previstos inicialmente. Documentar estos desafíos y sus soluciones es valioso porque refleja la realidad del trabajo de desarrollo de software, donde no todo funciona a la primera.

**Problema 1 — Permisos de escritura bloqueados por Windows.**
Al crear el proyecto, Unity intentó escribir archivos de caché en la carpeta del proyecto, pero Windows bloqueó el acceso. Esto se debió a restricciones de seguridad del antivirus sobre carpetas de usuario. La solución fue ejecutar Unity Hub con privilegios de administrador, lo que le dio permiso de escribir en las carpetas necesarias.

**Problema 2 — Incompatibilidad del sistema de entrada.**
Al implementar el primer salto del personaje con el código clásico de Unity (`Input.GetButtonDown`), la consola mostró el error: *"You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package"*. Esto ocurrió porque Unity 6 usa por defecto el New Input System, que es incompatible con la clase `Input` antigua. La solución fue reescribir el código de entrada usando `Keyboard.current`, que es la forma correcta en Unity 6.

**Problema 3 — El personaje era invisible por escala cero.**
Durante una sesión de prueba, el personaje no aparecía en pantalla aunque estaba correctamente configurado. Al revisar el Inspector de Unity se descubrió que los valores de escala en X y Z eran 0 (`Scale: X=0, Y=1, Z=0`), lo que colapsaba el modelo a un plano invisible. Corregir los valores a `Scale: X=1, Y=1, Z=1` solucionó el problema inmediatamente.

**Problema 4 — La etiqueta "Obstaculo" no existía.**
El código de detección de colisiones busca objetos con la etiqueta `Obstaculo` para identificar cuándo el personaje choca. Sin embargo, Unity no crea etiquetas automáticamente; deben registrarse manualmente en la configuración del proyecto. Al no existir, Unity lanzaba el error *"Tag: Obstaculo is not defined"* en cada colisión. La solución fue agregarla desde `Edit → Project Settings → Tags and Layers`.

Estos problemas ilustran que el trabajo de desarrollo no es solo escribir código, sino también resolver incompatibilidades, configurar entornos y depurar comportamientos inesperados del motor y del sistema operativo.

### 4.8 Organización de la Escena y Estructura del Proyecto

La escena principal del juego (`GameScene`) sigue una jerarquía de objetos clara que facilita la navegación para todos los integrantes del equipo:

```
GameScene
├── CamaraPrincipal     → Cámara con Cinemachine, posición fija detrás del personaje
├── LuzDireccional      → Iluminación principal de la escena (simula el sol)
├── Suelo               → Plano extendido con Layer "Ground", escala 5×1×50
└── Dino                → Personaje controlable
    └── DetectorSuelo   → Objeto vacío en los pies, detecta si está en el suelo
```

**Figura 3.** *Jerarquía de la escena principal.*

Todos los objetos de la escena usan nombres en español para mantener consistencia con el idioma del equipo, mientras que los scripts y sus variables internas mantienen convenciones de nomenclatura estándar de C# en inglés. Esta separación entre lo que es configuración de escena (español) y código (inglés) es una práctica que reduce la confusión en equipos donde el lenguaje natural de trabajo no es el inglés.

El proyecto se organiza además con carpetas temáticas dentro de `Assets/` que separan los diferentes tipos de archivos: scripts, materiales, modelos, sonidos, animaciones y prefabs. Esta separación facilita que cada integrante del equipo encuentre rápidamente los archivos de su área de trabajo sin necesidad de revisar todo el proyecto.

### 4.9 Control de Versiones con Git y GitHub

Para que los cinco integrantes del equipo puedan trabajar simultáneamente sin perder cambios ni sobrescribir el trabajo de otro, el proyecto se gestiona con Git y se aloja en un repositorio de GitHub. Git permite que cada persona trabaje en su propio script de forma independiente y luego combine los cambios de forma controlada.

El repositorio se configuró con un archivo `.gitignore` específico para proyectos Unity, que excluye las carpetas `Library/` y `Temp/` que Unity genera automáticamente y pueden pesar varios gigabytes. Subir estas carpetas sería innecesario porque cualquier computadora con Unity las regenera sola al abrir el proyecto. Solo se sube el código fuente, las escenas, los materiales y la configuración del proyecto.

El flujo de trabajo diario del equipo es:
1. Descargar los cambios más recientes antes de empezar a trabajar (`git pull`).
2. Modificar únicamente los archivos del área propia.
3. Subir los cambios con un mensaje que describa qué se hizo (`git commit` + `git push`).

El repositorio está disponible en: **github.com/hebertsb/Runner3D-Unity**

### 4.10 División de Trabajo y Plan de Desarrollo

#### 4.10.1 Distribución de Roles

El equipo de cinco personas se organizó de manera que cada uno tuviera un área específica que no dependiera completamente de que otro terminara primero. Esto permitió avanzar en paralelo desde el primer día:

| Persona | Área de trabajo | Qué hace concretamente |
|---|---|---|
| P1 | Jugabilidad y Físicas | Controla cómo se mueve y salta el personaje (`DinoController.cs`) |
| P2 | Mundo Infinito | Hace que los obstáculos aparezcan y se reciclen (`ObjectPooler.cs`, `LevelScroller.cs`) |
| P3 | Gestión del Juego | Controla los estados, la puntuación y las pantallas (`GameManager.cs`, `UIManager.cs`) |
| P4 | Arte e Iluminación | Diseña el ambiente visual, modelos 3D y efectos de partículas |
| P5 | Animación y Cámara | Anima al personaje y configura el comportamiento de la cámara |

**Tabla 3.** *División de roles del equipo de desarrollo.*

#### 4.10.2 Plan de Desarrollo en Tres Días

El desarrollo se organiza en tres iteraciones de un día cada una, donde cada iteración produce algo jugable que se puede probar:

**Día 1 — Prototipo funcional**

El objetivo del primer día es tener algo que se pueda jugar, aunque sea visualmente básico. El personaje debe poder saltar sobre obstáculos que se mueven, y el juego debe terminar cuando hay una colisión. No importa si todo se ve con cubos grises; lo importante es que la mecánica central funcione correctamente.

Lo que se construye el Día 1:
- Escena con suelo y personaje (cápsula simple con física)
- Script de salto con gravedad personalizada
- Obstáculos básicos (cubos) que se mueven hacia el jugador
- Detección de colisión que muestra "Game Over" en consola

**Día 2 — Integración de sistemas y arte**

Con la mecánica base funcionando, el segundo día se enfoca en hacer el juego más completo: el mundo infinito con reciclaje de obstáculos, las animaciones del personaje, y la interfaz de usuario básica.

Lo que se construye el Día 2:
- Sistema de Object Pooling en funcionamiento (sin pausas visibles)
- Modelos 3D del personaje y obstáculos importados
- Animaciones configuradas (correr, saltar, agacharse)
- Pantallas de menú, juego y Game Over con puntuación visible

**Día 3 — Pulido y presentación**

El último día se dedica a hacer que el juego se vea y suene bien para la presentación. Se agregan los efectos visuales, el audio y se calibra la dificultad.

Lo que se construye el Día 3:
- Efectos de postprocesado URP (Bloom, Color Grading, Motion Blur)
- Partículas de polvo en los pies del personaje al correr
- Vibración de cámara al chocar (Cinemachine Shake)
- Música de fondo y efectos de sonido
- Ajuste fino de la curva de dificultad

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

**R3.** El proyecto adoptó el *New Input System* de Unity 6 desde el inicio, lo que demostró ser la decisión correcta a largo plazo. Se recomienda mantener este sistema en futuras extensiones del proyecto, ya que permite agregar soporte para mandos de juego y pantallas táctiles sin modificar el código de lógica existente, simplemente configurando nuevos mapeos de entrada en el editor.

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
