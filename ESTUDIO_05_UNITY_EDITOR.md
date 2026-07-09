# ESTUDIO 05 — Qué se configuró en Unity Editor (sin código)

> Este MD cubre todo lo que hiciste/hicimos en el editor de Unity —
> la parte visual e inspector. Fundamental para explicar "cómo se armó el juego".

---

## Hierarchy — objetos en la escena

```
GameScene
  ├─ GameManager          → script GameManager.cs
  ├─ UIManager            → script UIManager.cs + Canvas UI
  ├─ LevelScroller        → script LevelScroller.cs
  ├─ AudioManager         → script AudioManager.cs + 2 AudioSources
  ├─ DecoracionLateral    → script DecoracionLateral.cs
  ├─ ObjectPooler         → script ObjectPooler.cs
  ├─ AereoSpawner         → script AereoSpawner.cs
  │
  ├─ CamaraPrincipal      → Camera + CameraShake.cs
  │     Position: Y=12, Z=-15
  │     Rotation: X=15 (mirando hacia abajo-adelante)
  │
  ├─ Dino                 → Rigidbody + CapsuleCollider + DinoController.cs
  │     └─ Velociraptor   → modelo FBX + Animator
  │     └─ PuntoSuelo     → Transform vacío en Y=0 (detección suelo)
  │
  ├─ Suelo (x3 tiles)     → GroundScroller.cs los recicla
  │     └─ Plano 3D con material naranja/tierra
  │
  ├─ Luz Direccional      → ilumina toda la escena
  └─ Canvas               → panelMenu + panelHUD + panelGameOver
```

---

## Tags creados en Unity

**Edit → Project Settings → Tags and Layers**

| Tag | Asignado a |
|-----|-----------|
| `Obstaculo` | Prefab Cactus (obstáculo principal) |
| `ObstaculoAereo` | Prefab Pteranodon |
| `Scrolleable` | Objetos de fondo que se mueven |
| `Player` | El Dino |

**Por qué son importantes:** LevelScroller hace `FindGameObjectsWithTag("Obstaculo")`
— si el cactus no tiene ese tag, LevelScroller no lo mueve.

---

## Layer creada: "Suelo"

**Edit → Project Settings → Tags and Layers → Layers**

- El plano del suelo se asignó al Layer **"Suelo"**
- En DinoController Inspector → `capaSuelo` = Layer Suelo
- `Physics.CheckSphere(..., capaSuelo)` solo detecta objetos en ese layer

**Por qué:** Sin LayerMask, la esfera detectaría el propio CapsuleCollider
del Dino y siempre pensaría que está en el suelo.

---

## Prefabs creados

Un prefab es un "molde" de GameObject que se puede reutilizar e instanciar.

| Prefab | Contenido | Quién lo usa |
|--------|-----------|-------------|
| `Obstaculo` | Cactus 3D + BoxCollider (Trigger) + tag Obstaculo | ObjectPooler |
| `ObstaculoAereo` | Pteranodon 3D + BoxCollider (Trigger) + tag ObstaculoAereo + ObstaculoAereo.cs | AereoSpawner |
| `CactusDecoracion` | Cactus 3D sin collider (no mata) | DecoracionLateral |

**¿Cómo se crea un prefab?**
Arrastrás un GameObject desde la Hierarchy a la carpeta Assets/Prefabs en el Project.
Unity crea el archivo `.prefab`.

**Trigger vs Collider normal:**
- Collider normal → rebota, tiene física
- `Is Trigger = true` → no tiene física, solo dispara `OnTriggerEnter`

---

## Materiales creados

**Assets/Materials/**

| Material | Shader | Color | Usado en |
|----------|--------|-------|----------|
| `MaterialSuelo` | URP/Lit | #C2651A (naranja tierra) | 3 tiles del suelo |
| `TempDino` | URP/Lit | #39FF14 (verde neón) | Velociraptor |
| `MaterialAve` | URP/Lit | #8B4513 (marrón) | Pteranodon |
| `MaterialObstaculo` | URP/Lit | #3D7A3D (verde oscuro) | Cactus LOD0 |
| `SkyboxDesert` | Mobile/Skybox | textura cielo | Skybox de la escena |

**Por qué el Skybox usa Mobile/Skybox y no URP/Lit:**
URP/Lit es para objetos 3D en la escena. El Skybox necesita un shader
especial para envolver toda la escena como fondo. Mobile/Skybox es
el correcto para skyboxes de 6 caras.

**¿Dónde se asigna el Skybox?**
Window → Rendering → Lighting → Environment → Skybox Material

---

## AnimatorController — DinoAnimator

**Ubicación:** Assets/Animations/DinoAnimator.controller

### Estados
```
[idle] ──IsRunning=true──► [running] ──Morir──► [dying]
           ◄──IsRunning=false──
```

### Parámetros (los que el código controla)
| Parámetro | Tipo | Script que lo cambia |
|-----------|------|---------------------|
| `IsRunning` | Bool | DinoController |
| `isCrouching` | Bool | DinoController |
| `IsGrounded` | Bool | DinoController |
| `Morir` | Trigger | DinoController |

### Clips de animación asignados
- Estado `idle` → `Velociraptor@idle.fbx`
- Estado `running` → `Velociraptor@running.fbx`
- Estado `dying` → `Velociraptor@dying.fbx`

**¿Cómo se asignó al Velociraptor?**
En el Inspector del objeto Velociraptor → componente Animator →
campo "Controller" → se arrastró el archivo DinoAnimator.

---

## Cámara — configuración

**CamaraPrincipal (Main Camera):**
- Position: X=0, Y=12, Z=-15
- Rotation: X=15, Y=0, Z=0
- Background Type: Skybox (para mostrar el cielo)

**Por qué ese ángulo:**
Y=12 y Z=-15 con rotación X=15 da vista diagonal hacia adelante —
el jugador ve la pista y puede anticipar los obstáculos que vienen.

**CameraShake.cs en la cámara:**
Guarda la `posOriginal` y cuando `Shake()` es llamado,
desplaza la cámara aleatoriamente durante 0.4 segundos.

---

## AudioManager — configuración en Inspector

**Necesita 2 AudioSources** en el mismo GameObject:
- AudioSource 1 → música de fondo (loop)
- AudioSource 2 → sonido game over

**Clips asignados en Inspector:**
| Campo | Archivo |
|-------|---------|
| Musica | ES_Powerwalkin.mp3 |
| Sonido Game Over | EMOTIONAL DAMAGE 1.mp3 |
| Sonido Salto | SonidoSalto.mp3 |
| Sonido Esquive | SonidoEsquive.mp3 |

---

## Canvas UI — estructura

**Canvas → Render Mode: Screen Space - Overlay**
(se dibuja encima de todo, siempre visible)

```
Canvas
  ├─ panelMenu (GameObject)
  │    └─ TextoTitulo (TextMeshPro)
  │    └─ BotonJugar (Button → onClick: UIManager.BotonJugar())
  │
  ├─ panelHUD (GameObject)
  │    └─ textoPuntaje (TextMeshPro) ← se actualiza cada frame
  │
  └─ panelGameOver (GameObject)
       ├─ textoGameOver (TextMeshPro) → "GAME OVER\nPuntos: X"
       ├─ textoRecord (TextMeshPro)   → "Record: X" o "★ NUEVO RECORD ★"
       └─ BotonReintentar (Button → onClick: UIManager.BotonReintentar())
```

**¿Por qué TextMeshPro y no Text normal?**
TextMeshPro usa renderizado vectorial (SDF) — el texto se ve nítido
a cualquier tamaño sin pixelarse. El Text normal es obsoleto en Unity 6.

---

## Preguntas de autoevaluación

1. ¿Para qué sirve el objeto vacío `PuntoSuelo`? ¿Por qué no se usa el Dino directamente?
2. ¿Qué es un Tag en Unity y cómo se usa en este proyecto?
3. ¿Qué diferencia hay entre un Collider normal y uno con `Is Trigger = true`?
4. ¿Por qué el Skybox no puede usar shader URP/Lit?
5. ¿Qué es un Prefab y para qué se usan en este juego?
6. ¿Por qué el AudioManager necesita 2 AudioSources?
7. ¿Qué es un AnimatorController y cómo se conecta al modelo 3D?

---

## Resumen del orden de estudio recomendado

```
01 → Visión general y flujo de partida    (entender el mapa)
02 → DinoController y físicas             (lo más técnico)
03 → Obstáculos y Object Pooling          (patrón de diseño)
04 → Puntaje, velocidad y UI              (sistemas de juego)
05 → Unity Editor                         (cómo se armó todo)
```

Respondé las preguntas de cada MD sin mirar el código.
Si no podés responder una, volvé al MD y releé esa sección.
