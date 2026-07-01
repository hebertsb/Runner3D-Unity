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

**Qué hacer:**

**Animator Controller:**
1. Assets → Create → Animator Controller → nombre "DinoAnimator"
2. Asignarlo al componente Animator del Dino
3. Crear estados: Idle, Run, Jump, Crouch
4. Parámetros a crear:
   - `IsGrounded` (bool) → transición Run↔Jump
   - `isCrouching` (bool) → YA está siendo seteado por DinoController
   - `Jump` (trigger) → opcional para salto

**CinemachineShake.cs:**
1. Instalar Cinemachine: Package Manager → Cinemachine
2. Hierarchy → Cinemachine → Virtual Camera
3. Crear script `Assets/Scripts/CinemachineShake.cs`
4. Efecto: cuando Dino colisiona con obstáculo → cámara tiembla 0.3s
5. Llamarlo desde DinoController en OnTriggerEnter:
```csharp
CinemachineShake.Instance.ShakeCamera(3f, 0.3f);
```

**Cámara actual:**
- `CamaraPrincipal` ya está en escena
- Posición recomendada para endless runner: detrás y arriba del Dino
- Si usas Cinemachine Virtual Camera, la CamaraPrincipal se convierte en "brain"

---

## FLUJO DE INTEGRACIÓN RECOMENDADO

```
Semana actual:
1. Gabriel → GameManager básico (menú + game over)
2. Douglas → Animator Controller básico (Idle + Run)
3. Luis → reemplazar modelos principales (Dino + Suelo)

Siguiente:
4. Douglas → CinemachineShake
5. Gabriel → puntaje + UIManager completo
6. Luis → iluminación URP + skybox + VFX partículas

Integración final:
7. Todos hacen git pull → prueba conjunta
8. Fix de bugs de integración
9. Build final para Feria Expociencia
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
