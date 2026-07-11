# Runner 3D — Documentación Técnica del Juego

**Materia:** Programación Gráfica ELC102 — UAGRM  
**Motor:** Unity 6 (6000.0.5f1) con Universal Render Pipeline (URP)  
**Género:** Endless Runner 3D en tercera persona  

---

## 1. Clasificación del Juego

| Categoría | Valor |
|-----------|-------|
| Género | Endless Runner |
| Vista | Tercera persona (cámara alta, ángulo diagonal) |
| Jugadores | 1 (single player) |
| Plataforma | PC (Windows) / WebGL |
| Control | Teclado (Space/W/↑ = saltar, S/↓/LCtrl = agachar) |
| Condición de derrota | Colisionar con obstáculo terrestre o aéreo |
| Condición de victoria | No existe — el objetivo es el mayor puntaje posible |

---

## 2. Arquitectura General — Scripts

```
GameManager        → máquina de estados del juego (Menu / Jugando / GameOver)
UIManager          → muestra/oculta paneles HUD, menú, game over
LevelScroller      → mueve todo lo que tiene tag "Obstaculo" y "Scrolleable"
GroundScroller     → recicla los 3 tiles del suelo infinitamente
DinoController     → física del jugador: salto, agacharse, colisiones
ObjectPooler       → pool de cactus (obstáculos terrestres)
AereoSpawner       → pool del Pteranodon (obstáculo aéreo)
ObstaculoAereo     → script del ave: se mueve solo, registra si está activo
DecoracionLateral  → pool de cactus decorativos a los lados (±X)
AudioManager       → música de fondo + efectos de sonido
CameraShake        → vibración de cámara al morir
```

---

## 2b. Mapa de Llamadas — Quién llama a quién

```
INPUT (teclado)
 └─► DinoController.Update()
       ├─► rb.linearVelocity = fuerzaSalto        → aplica físicas de salto
       ├─► AudioManager.ReproducirSalto()         → suena SonidoSalto.mp3
       ├─► capsule.height = alturaAgachado        → achica hitbox
       └─► animator.SetBool("isCrouching", ...)  → activa animación

COLISIÓN con obstáculo
 └─► DinoController.OnTriggerEnter()
       ├─► CameraShake.Instance.Shake()          → vibra cámara
       ├─► animator.SetTrigger("Morir")          → animación de muerte
       └─► GameManager.TriggerGameOver()
             ├─► LevelScroller.DetenerJuego()    → para el movimiento
             ├─► AudioManager.ReproducirGameOver() → para música, suena game over
             └─► UIManager.MostrarGameOver()     → cambia panel

CACTUS pasa al jugador (Z < -20)
 └─► LevelScroller.Update()
       ├─► GameManager.RegistrarEsquive()
       │     ├─► Combo++
       │     ├─► ObstaculosEsquivados++
       │     └─► AudioManager.ReproducirEsquive() → suena SonidoEsquive.mp3
       └─► ObjectPooler.RetornarAlPool(obj)      → cactus vuelve al pool

CADA 5 SEGUNDOS
 └─► LevelScroller.Update() → timerIncremento >= 5f
       └─► VelocidadActual += 0.5f              → aumenta velocidad del juego
```

---

## 3. Máquina de Estados — `GameManager.cs`

El juego tiene **3 estados**:

```
Menu  ──[BotonJugar]──►  Jugando  ──[colisión]──►  GameOver
                                                        │
                                    ◄──[Reiniciar]──────┘
```

### Estados
| Estado | Qué pasa |
|--------|----------|
| `Menu` | Pantalla inicial, nada se mueve |
| `Jugando` | Obstáculos se mueven, puntos suben, input activo |
| `GameOver` | Todo para, se muestra puntaje y record |

### Variables de puntaje
```csharp
float  Puntos              // acumulado en tiempo real
int    Record              // guardado en PlayerPrefs ("Runner3D_Record")
int    Combo               // obstáculos esquivados seguidos sin morir
int    ObstaculosEsquivados // total de la partida
int    NivelActual         // calculado de la velocidad actual
```

### Fórmula de puntaje
```
Puntos += VelocidadActual × ComboMultiplicador × Time.deltaTime
```

### Fórmula de nivel
```
NivelActual = floor((VelocidadActual - 6) / 2) + 1
```
Ejemplo: vel=8 → Nivel 2, vel=12 → Nivel 4, vel=20 → Nivel 8

---

## 4. Sistema de Combo

Los combos multiplican los puntos obtenidos mientras el jugador no muera:

| Obstáculos esquivados seguidos | Multiplicador |
|-------------------------------|---------------|
| 0 – 4 | x1 (normal) |
| 5 – 9 | x2 (dorado en HUD) |
| 10+ | x3 (dorado en HUD) |

- El combo **se resetea a 0 al morir**
- Se acumula en `GameManager.Combo`
- `LevelScroller` llama `GameManager.RegistrarEsquive()` cuando un cactus pasa al jugador (Z < -20)

---

## 5. Velocidad y Dificultad — `LevelScroller.cs`

### Variables configurables en Inspector
```csharp
float velocidadInicial    = 8f    // velocidad al iniciar
float incrementoVelocidad = 0.5f  // cuánto sube cada intervalo
float velocidadMaxima     = 25f   // techo de velocidad
float intervaloIncremento = 5f    // segundos entre cada aumento
```

### Función exacta que aumenta la velocidad
```csharp
// En LevelScroller.Update() — se ejecuta cada frame
timerIncremento += Time.deltaTime;
if (timerIncremento >= intervaloIncremento)   // cada 5 segundos
{
    timerIncremento = 0f;
    VelocidadActual = Mathf.Min(VelocidadActual + incrementoVelocidad, velocidadMaxima);
    // Mathf.Min garantiza que nunca supere 25
}
```

### Función exacta que calcula el Nivel (en GameManager.Update)
```csharp
float vel = LevelScroller.Instance.VelocidadActual;
NivelActual = Mathf.Max(1, Mathf.FloorToInt((vel - 6f) / 2f) + 1);
// vel=8 → (8-6)/2 + 1 = 2  → Nivel 2
// vel=12 → (12-6)/2 + 1 = 4 → Nivel 4
// vel=20 → (20-6)/2 + 1 = 8 → Nivel 8
```

### Funcionamiento
- Cada **5 segundos** de juego, `VelocidadActual += 0.5`
- Tope en **25 unidades/segundo**
- `VelocidadActual` es una propiedad pública que todos los scripts leen
- El suelo, obstáculos y decoraciones se mueven a esta velocidad

### Qué mueve LevelScroller
1. Objetos con tag **"Obstaculo"** → los mueve y recicla en el pool cuando `Z < -20`
2. Objetos con tag **"Scrolleable"** → solo los mueve (sin reciclar)

---

## 6. Suelo Infinito — `GroundScroller.cs`

Usa **3 tiles** de suelo (planos 3D) que se reciclan en bucle:

```
Tile A → Tile B → Tile C → [Tile A reaparece adelante] → ...
```

- Cuando un tile llega a `Z < -60`, se teletransporta al frente del tile más adelantado
- `longitudTile = 50f` → separación entre tiles
- Se mueven a la misma `VelocidadActual` de `LevelScroller`

---

## 7. Jugador — `DinoController.cs`

### Componentes requeridos
- `Rigidbody` — física
- `CapsuleCollider` — hitbox
- `Animator` (en hijo "Velociraptor") — animaciones

### Sistema de Salto

#### Detección de suelo
```csharp
// Raycast esférico desde puntoSuelo (Transform hijo al piso)
bool enSueloFisico = Physics.CheckSphere(puntoSuelo.position, 0.2f, capaSuelo);
```

#### Coyote Time (0.15 segundos)
```csharp
// Permite saltar hasta 0.15s DESPUÉS de salir del borde
if (enSueloFisico)
    timerCoyote = 0.15f;   // resetea mientras toca suelo
else
    timerCoyote -= Time.deltaTime; // cuenta hacia abajo en el aire

estaEnSuelo = timerCoyote > 0f;  // true si recién estuvo en suelo
```
> **Por qué existe:** Sin coyote time, si el jugador presiona saltar en el borde de un frame donde la física no detectó el suelo, el salto se ignora. Con 0.15s de gracia, el salto siempre responde.

#### Aplicar salto
```csharp
if (salto && estaEnSuelo)
{
    timerCoyote = 0f;  // cancela la gracia inmediatamente
    rb.linearVelocity = new Vector3(vel.x, fuerzaSalto, vel.z);
    // fuerzaSalto = 8f por defecto
}
```

#### Gravedad extra en caída
```csharp
// En FixedUpdate: hace la caída más pesada/rápida que la subida
if (rb.linearVelocity.y < 0)
    rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
    // multiplicadorCaida = 2.5f → caída 1.5x más rápida que subida
```

### Sistema de Agacharse

#### Teclas
```csharp
bool agachado = jugando && !salto && (
    S || ↓ || LCtrl);  // !salto evita bloquear el salto
```

#### Cambio de hitbox
```csharp
// En Start() lee los valores originales del CapsuleCollider:
alturaColNormal    = capsule.height;           // ej: 2.0
centroNormal       = capsule.center;           // ej: Y=1.0
alturaColAgachado  = capsule.height * 0.5f;   // ej: 1.0
centroAgachado     = center con Y * 0.5f;     // ej: Y=0.5

// En Update(), aplica según estado:
capsule.height = agachado ? alturaColAgachado : alturaColNormal;
capsule.center = agachado ? centroAgachado    : centroNormal;
```
> El collider se **encoge a la mitad** al agacharse, permitiendo que el Pteranodon (volando alto) pase por encima sin golpear.

### Animaciones — parámetros del Animator
| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `IsRunning` | Bool | true mientras `estado == Jugando` |
| `isCrouching` | Bool | true mientras tecla agachar presionada |
| `IsGrounded` | Bool | true mientras en suelo |
| `Morir` | Trigger | se activa una vez al morir |

### Colisiones
```csharp
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Obstaculo") || other.CompareTag("ObstaculoAereo"))
    {
        CameraShake.Instance?.Shake(0.3f, 0.4f); // vibración 0.4s intensidad 0.3
        animator.SetTrigger("Morir");
        GameManager.Instance.TriggerGameOver();
    }
}
```

---

## 8. Pool de Obstáculos Terrestres — `ObjectPooler.cs`

### Concepto: Object Pooling
En vez de crear/destruir objetos cada vez (costoso en CPU), se mantiene un **pool** de objetos inactivos que se reutilizan.

```
Pool (cola de inactivos)
    ↓ spawn
Objeto activo → se mueve → pasa al jugador → regresa al pool
    ↑_______________________________________________|
```

### Variables Inspector
```csharp
GameObject prefabObstaculo   // el cactus prefab
int        tamanoPool = 10   // cuántos cactus existen en total
float      distanciaSpawn = 30f  // Z donde aparece cada cactus
float      intervaloSpawn = 2f   // segundos entre cada spawn
```

### Control de pausa (para coordinar con el ave)
```csharp
public void PausarPorSegundos(float segundos)
{
    pausaTimer = segundos;  // bloquea spawn durante N segundos
    timerSpawn = 0f;        // resetea el timer de spawn
}
```

### Bloqueo por ave activa
```csharp
void SpawnObstaculo()
{
    if (ObstaculoAereo.HayAveActiva) return; // NO spawnar si ave en vuelo
    // ...
}
```

---

## 9. Obstáculo Aéreo — `AereoSpawner.cs` + `ObstaculoAereo.cs`

### AereoSpawner — variables Inspector
```csharp
GameObject prefabAereo        // Pteranodon prefab
int        tamanoPool = 4
float      distanciaSpawn = 30f
float      intervaloMin = 6f  // mínimo segundos entre aves
float      intervaloMax = 12f // máximo segundos entre aves
float      alturaVuelo = 2.5f // Y donde vuela el Pteranodon
```

### Lógica de spawn con coordinación
```csharp
void Spawn()
{
    // Solo spawna si NO hay cactus entre Z=-2 y Z=+∞ (pista despejada)
    foreach (GameObject obs in FindGameObjectsWithTag("Obstaculo"))
    {
        if (obs.activeInHierarchy && obs.transform.position.z > -2f)
        {
            timer = intervaloActual - 1f; // reintento en 1 segundo
            return;
        }
    }
    // Spawna ave y pausa cactus por 8 segundos
    obj.transform.position = new Vector3(0f, alturaVuelo, distanciaSpawn);
    ObjectPooler.Instance?.PausarPorSegundos(8f);
}
```

### Garantía de no superposición
| Condición | Resultado |
|-----------|-----------|
| Cactus activo en pista | Ave NO spawna |
| Ave activa (HayAveActiva=true) | Cactus NO spawna |
| Ave spawneada | Cactus pausado 8s más tiempo de cooldown |

### ObstaculoAereo
```csharp
public static bool HayAveActiva = false; // flag global estático

void OnEnable()  { HayAveActiva = true;  } // al activarse
void OnDisable() { HayAveActiva = false; } // al desactivarse

void Update()
{
    // Se mueve solo (no usa LevelScroller para moverlo externamente)
    transform.Translate(Vector3.back * LevelScroller.Instance.VelocidadActual * Time.deltaTime);
    if (transform.position.z < -25f)
    {
        gameObject.SetActive(false); // regresa al pool
        AereoSpawner.Instance?.RetornarAlPool(gameObject);
    }
}
```

---

## 10. Decoraciones Laterales — `DecoracionLateral.cs`

Cactus decorativos que aparecen a los lados del camino (no matan al jugador):

```csharp
float xOffset = 8f      // distancia del centro (±8 unidades)
int   cantidad = 20     // tamaño del pool (pares)
float distanciaSpawn = 35f
float intervaloMin = 1.5f
float intervaloMax = 4f
```

### PreSpawn al inicio
Al iniciar la escena, llena el camino visible inmediatamente:
```csharp
for (float z = 5f; z <= distanciaSpawn; z += 6f)
{
    PreSpawn(-xOffset, z);  // lado izquierdo
    PreSpawn( xOffset, z);  // lado derecho
}
```

Los decorativos **no tienen tag "Obstaculo"** — `LevelScroller` no los mueve. `DecoracionLateral` los mueve manualmente en su propio `Update()`.

---

## 11. Audio — `AudioManager.cs`

Usa **múltiples AudioSources** en el mismo GameObject:

| AudioSource | Uso |
|-------------|-----|
| `sources[0]` | Música de fondo (loop) |
| `sources[1]` | Sonido Game Over (no interrumpe) |

### Archivos de audio asignados en Inspector
| Campo Inspector | Archivo | Cuándo suena |
|----------------|---------|--------------|
| `Musica` | ES_Powerwalkin.mp3 | Al iniciar → loop continuo |
| `Sonido Game Over` | EMOTIONAL DAMAGE 1.mp3 | Al morir |
| `Sonido Salto` | SonidoSalto.mp3 | Cada vez que el jugador salta |
| `Sonido Esquive` | SonidoEsquive.mp3 | Cada vez que un cactus pasa |

### Dónde se captura/dispara cada sonido
```
SonidoSalto.mp3
  Disparado en: DinoController.Update()
  Condición: salto == true && estaEnSuelo == true
  Función: AudioManager.Instance.ReproducirSalto()
  Cómo: sources[0].PlayOneShot(sonidoSalto, 0.7f)

SonidoEsquive.mp3
  Disparado en: GameManager.RegistrarEsquive()
  Condición: un cactus pasó Z < -20 (LevelScroller lo detecta)
  Función: AudioManager.Instance.ReproducirEsquive()
  Cómo: sources[0].PlayOneShot(sonidoEsquive, 0.5f)

EMOTIONAL DAMAGE (GameOver)
  Disparado en: GameManager.TriggerGameOver()
  Condición: colisión con Obstaculo o ObstaculoAereo
  Función: AudioManager.Instance.ReproducirGameOver()
  Cómo: para sources[0] (música), luego sources[1].PlayOneShot(sonidoGameOver)
```

### Métodos
```csharp
ReproducirSalto()    // PlayOneShot sobre sources[0] — no interrumpe música
ReproducirEsquive()  // PlayOneShot sobre sources[0] — al esquivar obstáculo
ReproducirGameOver() // para música, reproduce Game Over en sources[1]
PararMusica()        // solo para sources[0]
```

> `PlayOneShot()` permite reproducir un clip **encima** del clip actual sin interrumpirlo.  
> Diferencia con `Play()`: `Play()` corta lo que estaba sonando; `PlayOneShot()` mezcla ambos.

---

## 12. Vibración de Cámara — `CameraShake.cs`

Singleton en la CámaraPrincipal:

```csharp
public void Shake(float intensidad, float duracion)
{
    magnitud = intensidad;   // 0.3f al morir
    timerShake = duracion;   // 0.4f segundos
}

// En Update(), durante timerShake > 0:
transform.localPosition = posOriginal + Random.insideUnitSphere × magnitud;
```

Al terminar el timer vuelve a `posOriginal`.

---

## 13. HUD — `UIManager.cs`

Actualiza el texto cada frame en `Update()`:

```
Puntos: 247  x2
Nivel: 3  |  Esquivados: 12  Mejor: 180
```

- `x2` / `x3` aparecen en **amarillo dorado** (`<color=#FFD700>`)
- "NUEVO RECORD!" aparece en **verde neón** (`<color=#39FF14>`) mientras jugás
- Game Over muestra `★ NUEVO RECORD: 500 ★` en amarillo si batiste el récord

### Paneles
| Panel | Visible cuando |
|-------|----------------|
| `panelMenu` | Estado == Menu |
| `panelHUD` | Estado == Jugando |
| `panelGameOver` | Estado == GameOver |

---

## 14. Tags de Unity usados

| Tag | Qué objeto | Para qué |
|-----|-----------|----------|
| `Obstaculo` | Cactus principal | LevelScroller lo mueve y recicla; DinoController detecta colisión |
| `ObstaculoAereo` | Pteranodon | DinoController detecta colisión |
| `Scrolleable` | Decorados del fondo | LevelScroller solo lo mueve |
| `Player` | Velociraptor | Referencia general |

---

## 15. Flujo completo de una partida

```
1. Escena carga → GameManager.Start() → UIManager.MostrarMenu()
2. Jugador presiona "Jugar" → UIManager.BotonJugar() → GameManager.IniciarJuego()
   - Estado = Jugando
   - LevelScroller.IniciarJuego() → VelocidadActual = 8
   - UIManager.MostrarHUD()

3. Loop de juego:
   - GroundScroller recicla suelo infinitamente
   - LevelScroller mueve cactus y los recicla al pasar (llama RegistrarEsquive)
   - ObjectPooler spawna cactus cada 2 segundos (si no hay ave)
   - AereoSpawner spawna ave cada 6-12s (solo si pista vacía)
   - DinoController lee input → salto / agachar
   - GameManager acumula puntos × combo × deltaTime
   - Cada 5s: velocidad += 0.5 (hasta máx 25)

4. Colisión con obstáculo → DinoController.OnTriggerEnter()
   - CameraShake.Shake(0.3f, 0.4f)
   - Animator.SetTrigger("Morir")
   - GameManager.TriggerGameOver()
     - Estado = GameOver
     - Guarda record en PlayerPrefs
     - AudioManager.ReproducirGameOver()
     - UIManager.MostrarGameOver()

5. Jugador presiona "Reintentar" → SceneManager.LoadScene(escenaActual)
   - Reinicia todo desde cero
```

---

## 16. Patrones de diseño utilizados

| Patrón | Donde | Por qué |
|--------|-------|---------|
| **Singleton** | GameManager, UIManager, LevelScroller, ObjectPooler, AereoSpawner, AudioManager, CameraShake, DecoracionLateral | Acceso global sin necesidad de referencias en Inspector |
| **Object Pool** | ObjectPooler, AereoSpawner, DecoracionLateral | Evita crear/destruir objetos en runtime (costoso) |
| **State Machine** | GameManager.Estado (enum) | Control claro de qué puede pasar en cada momento |
| **Observer implícito** | HayAveActiva (static bool) | Comunicación entre ObstaculoAereo y ObjectPooler sin referencia directa |

---

## 17. Valores por defecto del Inspector (referencia rápida)

| Script | Campo | Valor |
|--------|-------|-------|
| DinoController | Fuerza Salto | 8 |
| DinoController | Multiplicador Caída | 2.5 |
| DinoController | Coyote Time | 0.15s |
| LevelScroller | Velocidad Inicial | 8 |
| LevelScroller | Incremento | 0.5 cada 5s |
| LevelScroller | Velocidad Máxima | 25 |
| ObjectPooler | Tamaño Pool | 10 |
| ObjectPooler | Distancia Spawn | 30 |
| ObjectPooler | Intervalo Spawn | 2s |
| AereoSpawner | Tamaño Pool | 4 |
| AereoSpawner | Intervalo Min/Max | 6s – 12s |
| AereoSpawner | Altura Vuelo | 2.5 |
| AereoSpawner | Pausa post-ave | 8s |
| DecoracionLateral | Cantidad | 20 |
| DecoracionLateral | X Offset | 8 |
| CameraShake | Intensidad al morir | 0.3 |
| CameraShake | Duración al morir | 0.4s |
