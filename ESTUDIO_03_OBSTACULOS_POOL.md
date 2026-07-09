# ESTUDIO 03 — Obstáculos y Object Pooling

---

## ¿Qué es Object Pooling y por qué se usa?

**Sin pooling (malo):**
```
Cada vez que spawna un cactus → Instantiate() → crea objeto en memoria
Cuando sale de pantalla → Destroy() → libera memoria
```
Crear y destruir objetos constantemente es costoso en CPU/memoria
y puede causar micro-stutters (congelamiento de un frame).

**Con pooling (correcto):**
```
Al inicio → crea 10 cactus inactivos (SetActive(false))
Cuando necesita spawnar → activa uno (SetActive(true))
Cuando sale de pantalla → desactiva (SetActive(false)) y vuelve al pool
```
Los objetos NUNCA se crean ni destruyen en runtime. Solo se activan/desactivan.

---

## ObjectPooler.cs — cactus terrestres

### Estructura de la cola
```csharp
Queue<GameObject> pool = new Queue<GameObject>();
// Queue = cola FIFO: primero en entrar, primero en salir
// Como una fila de supermercado
```

### Ciclo de vida de un cactus
```
Start() → crea 10 cactus inactivos → los mete en pool (Enqueue)
    │
    ▼
Update() → timer llega a intervaloSpawn (2s)
    │
    ▼
SpawnObstaculo()
  ├─ verifica: ¿HayAveActiva? → si sí, NO spawna
  ├─ saca un cactus del pool (Dequeue)
  ├─ lo posiciona en Z=30 (adelante del jugador)
  └─ lo activa (SetActive(true))
    │
    ▼ (cactus se mueve con LevelScroller)
    │
    ▼ cuando Z < -20 (pasó al jugador)
LevelScroller llama → ObjectPooler.RetornarAlPool(cactus)
  ├─ SetActive(false)
  └─ Enqueue → vuelve al pool
```

### PausarPorSegundos — coordinación con el ave
```csharp
public void PausarPorSegundos(float segundos)
{
    pausaTimer = segundos;  // bloquea durante N segundos
    timerSpawn = 0f;        // resetea el timer de spawn
}

// En Update():
if (pausaTimer > 0) { pausaTimer -= Time.deltaTime; return; }
// Si pausaTimer > 0, SALTA todo el resto del Update (no spawna)
```

---

## AereoSpawner.cs — Pteranodon aéreo

### ¿Por qué el ave tiene su propio spawner?
El cactus y el ave tienen **reglas distintas**:
- Cactus: intervalo fijo (2s), siempre en el suelo
- Ave: intervalo aleatorio (6-12s), altura variable, nunca junto a cactus

### Lógica de spawn con verificación
```csharp
void Spawn()
{
    // Verifica que no haya cactus en la pista
    foreach (GameObject obs in FindGameObjectsWithTag("Obstaculo"))
    {
        if (obs.activeInHierarchy && obs.transform.position.z > -2f)
        {
            timer = intervaloActual - 1f;  // reintenta en 1 segundo
            return;  // SALE sin spawnar
        }
    }

    // Si llegó acá, la pista está vacía
    GameObject obj = pool.Dequeue();
    obj.transform.position = new Vector3(0f, alturaVuelo, 30f);
    obj.SetActive(true);
    ObjectPooler.Instance?.PausarPorSegundos(8f);  // pausa cactus 8s
}
```

**`obs.transform.position.z > -2f`:**
Verifica si el cactus todavía no pasó al jugador (Z positivo = adelante).
Si Z < -2, ya pasó → no es riesgo → no bloquea.

---

## ObstaculoAereo.cs — el ave se mueve sola

```csharp
public static bool HayAveActiva = false;  // flag ESTÁTICO global

void OnEnable()  { HayAveActiva = true;  }  // al activarse
void OnDisable() { HayAveActiva = false; }  // al desactivarse

void Update()
{
    // Se mueve a sí misma (no depende de LevelScroller para moverla)
    transform.Translate(Vector3.back * LevelScroller.Instance.VelocidadActual * Time.deltaTime);

    if (transform.position.z < -25f)
    {
        gameObject.SetActive(false);            // se desactiva → OnDisable → HayAveActiva=false
        AereoSpawner.Instance?.RetornarAlPool(gameObject);
    }
}
```

**¿Por qué `static bool HayAveActiva`?**
Un bool `static` es compartido por TODOS — no pertenece a una instancia específica.
`ObjectPooler` puede leer `ObstaculoAereo.HayAveActiva` sin tener referencia al objeto.
Es comunicación entre scripts sin dependencia directa.

---

## Garantía de no superposición cactus + ave

```
¿Hay cactus activo con Z > -2?
    SÍ → AereoSpawner retrasa el spawn (reintentar en 1s)
    NO → Ave spawna + pausa cactus 8 segundos

¿HayAveActiva == true?
    SÍ → ObjectPooler.SpawnObstaculo() hace return (no spawna)
    NO → cactus puede spawnar normalmente
```

**Resultado:** Nunca aparecen juntos. El jugador siempre sabe si debe
saltar (cactus) o agacharse (ave).

---

## DecoracionLateral.cs — cactus de los costados

No matan al jugador — solo son visuales para dar profundidad al escenario.

**Diferencia clave con ObjectPooler:**
- No tienen tag `"Obstaculo"` → LevelScroller no los mueve
- `DecoracionLateral.Update()` los mueve manualmente a `±xOffset` (X = ±8)
- Spawnan en **pares** (izquierda y derecha simultáneamente)

**PreSpawn al iniciar:**
```csharp
// En Start(), llena el camino visible desde el inicio
for (float z = 5f; z <= 35f; z += 6f)
{
    PreSpawn(-8f, z);  // lado izquierdo
    PreSpawn( 8f, z);  // lado derecho
}
```
Sin PreSpawn, al inicio el camino aparecería vacío de decoraciones
y los cactus decorativos irían apareciendo de a poco.

---

## Preguntas de autoevaluación

1. ¿Por qué Object Pooling es mejor que Instantiate/Destroy en runtime?
2. ¿Qué estructura de datos usa el pool y cómo funciona una Queue?
3. ¿Qué hace `PausarPorSegundos()` exactamente?
4. ¿Por qué `HayAveActiva` es `static`? ¿Qué problema resuelve?
5. ¿Qué chequea AereoSpawner antes de spawnar el ave?
6. ¿Por qué los cactus decorativos NO tienen tag "Obstaculo"?

---

> **Siguiente:** ESTUDIO_04_PUNTAJE_VELOCIDAD.md
