# Runner 3D — Guía de Continuación
## Lo que está hecho y lo que sigue por integrante

---

## CÓMO CLONAR EL PROYECTO (primera vez)

```bash
git clone https://github.com/hebertsb/Runner3D-Unity.git
cd Runner3D-Unity
```
Abrir Unity Hub → Add → seleccionar la carpeta clonada → Unity 6 (6000.0.5f1)

## CÓMO ACTUALIZAR (cada vez que alguien sube cambios)

```bash
git pull origin main
```

---

## ESTADO ACTUAL DEL PROYECTO

### Escena: GameScene
Objetos en la Hierarchy:
```
GameScene
├── CamaraPrincipal       (cámara principal URP)
├── LuzDireccional        (luz direccional)
├── Suelo                 (tile 1 de pista — Scale X=5, Y=1, Z=5)
├── Suelo2                (tile 2 — Position Z=50)
├── Suelo3                (tile 3 — Position Z=100)
├── Dino                  (jugador — Rigidbody, DinoController, MaterialDino)
│   └── DetectorSuelo     (punto para Physics.CheckSphere — Layer: Ground)
├── ObjectPooler          (script ObjectPooler.cs)
├── LevelScroller         (script LevelScroller.cs)
└── GroundScroller        (script GroundScroller.cs — tiles asignados)
```

### Tags creados
- `Obstaculo` — prefab de cubo obstáculo
- `Scrolleable` — objetos que LevelScroller mueve (obstáculos)

### Layers creados
- `Ground` (Layer 6) — usado por DinoController para detectar el suelo

### Prefabs
- `Assets/Prefabs/Obstaculo.prefab` — cubo 1×1.5×1, BoxCollider IsTrigger=true, tag=Obstaculo

---

## LO QUE HIZO CADA UNO HASTA AHORA

### Diego Toledo — Jugabilidad y Físicas ✅
**Script:** `Assets/Scripts/DinoController.cs`
- Salto con Rigidbody.linearVelocity + fuerzaSalto=8
- Fall multiplier (gravedad aumentada en caída ×2.5)
- Detección de suelo con Physics.CheckSphere
- Agachado: S / ↓ / Ctrl → animator.SetBool("isCrouching")
- Colisión: OnTriggerEnter detecta tag "Obstaculo" → Debug.Log("GAME OVER")
- Rigidbody constraints: Freeze Position X/Z, Freeze Rotation XYZ

### Hebert Suárez — Mundo Infinito ✅
**Scripts:** `Assets/Scripts/ObjectPooler.cs`, `LevelScroller.cs`, `GroundScroller.cs`

**ObjectPooler.cs:**
- Singleton: `ObjectPooler.Instance`
- Pre-crea 10 obstáculos en Start() usando Queue<GameObject>
- Spawn cada 2 segundos en z=30
- RetornarAlPool() recicla sin Instantiate/Destroy en runtime

**LevelScroller.cs:**
- Singleton: `LevelScroller.Instance`
- Mueve objetos con tag "Obstaculo" hacia el jugador
- Velocidad inicial=8, sube 0.5 cada 5s, máximo=25
- Obstáculos que pasan z=-20 → RetornarAlPool() automático
- IniciarJuego() / DetenerJuego() para GameManager

**GroundScroller.cs:**
- 3 tiles de suelo reciclados infinitamente
- Cuando tile pasa z=-60 → se reposiciona adelante del último tile
- Velocidad sincronizada con LevelScroller.Instance.VelocidadActual

---

## LO QUE FALTA — GUÍA PARA CADA INTEGRANTE

---

### GABRIEL AGUILAR — GameManager + UIManager

**Archivos a crear:**
- `Assets/Scripts/GameManager.cs`
- `Assets/Scripts/UIManager.cs`

**GameManager.cs debe hacer:**
```
Estados: Menu → Playing → GameOver → Menu

1. Al iniciar escena → estado = Menu
2. Al presionar Play → llamar:
   - LevelScroller.Instance.IniciarJuego()
   - ObjectPooler ya spawnea automático
3. TriggerGameOver() → estado = GameOver
   - Detener LevelScroller.Instance.DetenerJuego()
   - Mostrar pantalla Game Over (UIManager)
4. Reiniciar → recargar escena o resetear estado
```

**IMPORTANTE:** Cuando GameManager esté listo, en `DinoController.cs` línea 22:
```csharp
LevelScroller.Instance.IniciarJuego(); // TEMPORAL — reemplazar por lógica de GameManager
```
También descomentar línea 49:
```csharp
// GameManager.Instance.TriggerGameOver();
```

**UIManager.cs debe hacer:**
- Panel de menú principal (botón Play)
- Panel de puntaje en pantalla (texto con score)
- Panel de Game Over (botón Reintentar)
- El puntaje puede tomarse de `LevelScroller.Instance.VelocidadActual` o un contador propio

**En Unity — pasos:**
1. Crear Canvas en Hierarchy → UI → Canvas
2. Agregar TextMeshPro para puntaje
3. Agregar paneles para Menú y GameOver
4. Crear GameObject vacío "GameManager" → Add Component → GameManager
5. Crear GameObject vacío "UIManager" → Add Component → UIManager

---

### LUIS ANGULO — Arte e Iluminación

**Qué hacer:**
1. Reemplazar el Dino (esfera verde) por modelo 3D real
   - Importar modelo .fbx a `Assets/Models/`
   - Arrastrar a la escena y reemplazar el GameObject "Dino"
   - Mantener los componentes: Rigidbody, DinoController, Animator
   - Mantener hijo "DetectorSuelo" en la misma posición

2. Reemplazar obstáculos (cubos grises) por modelo 3D
   - Importar modelo obstáculo
   - Actualizar `Assets/Prefabs/Obstaculo.prefab` con el nuevo modelo
   - Mantener BoxCollider con IsTrigger=true y tag="Obstaculo"

3. Reemplazar suelo (plano gris) por textura de desierto
   - Crear Material en `Assets/Materials/`
   - Aplicar a Suelo, Suelo2, Suelo3

4. Color Grading URP (atardecer desierto):
   - Hierarchy → Create → Volume → Global Volume
   - Add Override → Color Adjustments
   - Tonos naranjas/cálidos para simular desierto

5. Skybox de desierto:
   - Window → Rendering → Lighting → Environment → Skybox Material

---

### DOUGLAS PADILLA — Animación y Cámara

**ESTADO ACTUAL (lo que ya existe):**
- Dino tiene componente `Animator` pero sin AnimatorController asignado
- `DinoController.cs` ya llama `animator.SetBool("isCrouching", agachado)` — el parámetro DEBE llamarse exactamente `isCrouching`
- `CamaraPrincipal` está en la escena en Position X=0 Y=3 Z=-8, Rotation X=15

---

**PASO 1 — git pull**
```bash
git clone https://github.com/hebertsb/Runner3D-Unity.git   # si es primera vez
# o si ya tienes el repo:
git pull origin main
```
Abrir Unity Hub → Add → carpeta clonada → Unity 6 (6000.0.5f1)

---

**PASO 2 — Crear Animator Controller**

En Project panel → carpeta `Assets/Animations/` (créala si no existe):
- Click derecho → Create → Animator Controller → nombre: `DinoAnimator`

Doble click en `DinoAnimator` para abrir la ventana Animator.

**Crear parámetros** (panel izquierdo → tab Parameters → click +):
- `isCrouching` → Bool
- `IsGrounded` → Bool

**Crear estados** (click derecho en grid → Create State → Empty):
- `Idle` — estado por defecto (naranja)
- `Run`
- `Crouch`

**Transiciones:**
- `Idle` → `Run`: click derecho en Idle → Make Transition → Run. Condition: `IsGrounded = true`
- `Run` → `Idle`: click derecho en Run → Make Transition → Idle. Condition: `IsGrounded = false`
- `Run` → `Crouch`: Condition: `isCrouching = true`
- `Crouch` → `Run`: Condition: `isCrouching = false`

> Si no tienes clips de animación todavía, los estados quedan vacíos — está bien, el controlador igual funciona para cuando Luis agregue los clips.

---

**PASO 3 — Asignar AnimatorController al Dino**

En Hierarchy → click `Dino` → Inspector → componente `Animator`:
- Campo **Controller** → arrastra `DinoAnimator` desde Project panel

---

**PASO 4 — Instalar Cinemachine**

Window → Package Manager → busca `Cinemachine` → Install (versión 3.x)

---

**PASO 5 — Crear Virtual Camera**

Hierarchy → click derecho → Cinemachine → **Cinemachine Camera** → nombre: `VirtualCamera`

Inspector de VirtualCamera:
- **Follow**: arrastra `Dino` desde Hierarchy
- **Look At**: arrastra `Dino` desde Hierarchy
- En **Body** → selecciona `Orbital Follow` o `3rd Person Follow`
  - Offset: X=0, Y=3, Z=-8 (mismo que la cámara actual)

Esto convierte `CamaraPrincipal` en "brain" automáticamente.

---

**PASO 6 — Crear CinemachineShake.cs**

En Project panel → `Assets/Scripts/` → click derecho → Create → MonoBehaviour Script → nombre: `CinemachineShake`

Reemplazar contenido completo con:

```csharp
using UnityEngine;
using Unity.Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }

    [SerializeField] private CinemachineCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float timerShake;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        noise = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensidad, float duracion)
    {
        if (noise == null) return;
        noise.AmplitudeGain = intensidad;
        timerShake = duracion;
    }

    void Update()
    {
        if (timerShake > 0)
        {
            timerShake -= Time.deltaTime;
            if (timerShake <= 0 && noise != null)
                noise.AmplitudeGain = 0f;
        }
    }
}
```

---

**PASO 7 — Configurar CinemachineShake en escena**

Hierarchy → click derecho → Create Empty → nombre: `CinemachineShake`
- Inspector → Add Component → `CinemachineShake`
- Campo **Virtual Camera** → arrastra `VirtualCamera` desde Hierarchy

En `VirtualCamera` → Add Component → `Cinemachine Basic Multi Channel Perlin`
- Noise Profile: `6D Shake` (o cualquiera disponible)
- Amplitude Gain: `0` (empieza en 0)

---

**PASO 8 — Conectar shake con DinoController**

Abrir `Assets/Scripts/DinoController.cs` y modificar el método `OnTriggerEnter`:

```csharp
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Obstaculo"))
    {
        Debug.Log("GAME OVER");
        CinemachineShake.Instance?.ShakeCamera(3f, 0.3f);
        GameManager.Instance.TriggerGameOver();
    }
}
```

---

**PASO 9 — Probar**

▶ Play → jugar → chocar con obstáculo → cámara debe temblar 0.3 segundos → aparece GAME OVER.

---

**PASO 10 — Commit y push**

Ctrl+S en Unity primero, luego:

```bash
git add Assets/Animations/DinoAnimator.controller
git add Assets/Animations/DinoAnimator.controller.meta
git add Assets/Scripts/CinemachineShake.cs
git add Assets/Scripts/CinemachineShake.cs.meta
git add Assets/Scenes/GameScene.unity
git commit -m "feat(douglas): agregar AnimatorController y CinemachineShake"
git push origin main
```

**IMPORTANTE:** Después de que Douglas haga push → avisar a Luis para que haga `git pull` antes de tocar la escena.

---

## FLUJO DE INTEGRACIÓN RECOMENDADO

```
COMPLETADO:
✅ Hebert  → ObjectPooler, LevelScroller, GroundScroller, DinoController
✅ Gabriel → GameManager, UIManager, Canvas (Menu/HUD/GameOver)

ORDEN PARA EVITAR CONFLICTOS EN GameScene.unity:
1. Douglas → AnimatorController + CinemachineShake → push
2. Luis    → git pull → modelos 3D + materiales + iluminación → push
3. Todos   → git pull → prueba conjunta → fix bugs → build final
```

---

## REGLAS DEL REPOSITORIO

- Rama principal: `main`
- Cada uno trabaja en sus archivos asignados
- **GameScene.unity** — coordinarse antes de editar (solo uno a la vez)
- Commits en español, descriptivos
- Siempre `git pull` antes de empezar a trabajar

```bash
# Flujo diario
git pull origin main          # actualizar
# ... hacer cambios ...
git add Assets/Scripts/MiScript.cs
git commit -m "feat: descripcion de lo que hice"
git push origin main
```
