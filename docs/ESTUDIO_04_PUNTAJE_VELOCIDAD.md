# ESTUDIO 04 — Puntaje, Velocidad, Combo y UI

---

## LevelScroller — motor del juego

Es el script más central después de GameManager.
**Todo lo que se mueve en el juego depende de `VelocidadActual`.**

### Función que aumenta la velocidad
```csharp
// En Update(), cada frame:
timerIncremento += Time.deltaTime;

if (timerIncremento >= 5f)       // cada 5 segundos reales
{
    timerIncremento = 0f;
    VelocidadActual = Mathf.Min(VelocidadActual + 0.5f, 25f);
    //                Mathf.Min → nunca supera 25
}
```

**`Time.deltaTime`:** tiempo que tardó el último frame en segundos.
Multiplicar por deltaTime hace que el movimiento sea igual
sin importar si el juego corre a 30fps o 120fps.

### Qué mueve y qué no mueve

| Tag | LevelScroller lo mueve | Lo recicla |
|-----|------------------------|------------|
| `"Obstaculo"` | SÍ | SÍ (Z < -20 → pool) |
| `"Scrolleable"` | SÍ | NO |
| Decoraciones | NO | NO (DecoracionLateral lo mueve) |
| Ave | NO | NO (ObstaculoAereo se mueve solo) |
| Suelo | NO | NO (GroundScroller lo maneja) |

---

## GroundScroller — suelo infinito

Usa **3 tiles** (planos) que se reciclan:

```
Posiciones iniciales:
Tile A: Z=0
Tile B: Z=50
Tile C: Z=100

→ Se mueven hacia atrás (Z negativo)

Cuando Tile A llega a Z < -60:
→ Se teletransporta a Z = (posición del tile más adelantado) + 50
→ Ahora: Tile A: Z=150, Tile B: Z=50, Tile C: Z=100
```

**Por qué 3 tiles y no 2:**
Con 2 podría haber un gap visible entre tiles cuando uno se teletransporta.
Con 3 siempre hay solapamiento.

---

## GameManager — puntaje y combo

### Cómo suben los puntos
```csharp
// En Update(), cada frame mientras Estado == Jugando:
Puntos += VelocidadActual × ComboMultiplicador × Time.deltaTime
```

- A mayor velocidad → suben más rápido los puntos
- A mayor combo → el multiplicador amplifica los puntos

**Ejemplo:** vel=12, combo x2, 1 segundo → 12 × 2 × 1 = 24 puntos

### Sistema de Combo
```csharp
// Cuando un cactus pasa al jugador (LevelScroller lo detecta):
GameManager.RegistrarEsquive()
  → Combo++
  → ObstaculosEsquivados++
  → AudioManager.ReproducirEsquive()

// Cuando el jugador muere:
TriggerGameOver()
  → Combo = 0  ← se resetea
```

| Combo | Multiplicador |
|-------|--------------|
| 0-4 | x1 |
| 5-9 | x2 (dorado en HUD) |
| 10+ | x3 (dorado en HUD) |

### Cálculo del Nivel
```csharp
NivelActual = Mathf.Max(1, Mathf.FloorToInt((VelocidadActual - 6f) / 2f) + 1)
```

| Velocidad | Nivel |
|-----------|-------|
| 8 | 2 |
| 10 | 3 |
| 12 | 4 |
| 20 | 8 |
| 25 | 10 |

### Record persistente
```csharp
// Al cargar el juego (Awake):
Record = PlayerPrefs.GetInt("Runner3D_Record", 0);
// Si no existe la clave, devuelve 0

// Al morir, si batió el record:
if (puntosFinales > Record)
{
    Record = puntosFinales;
    PlayerPrefs.SetInt("Runner3D_Record", Record);
    PlayerPrefs.Save();  // escribe al disco inmediatamente
}
```

**PlayerPrefs:** sistema de Unity que guarda datos entre sesiones
(como un mini-archivo de configuración). El record persiste aunque
cerrés y abras el juego.

---

## UIManager — lo que ve el jugador

### 3 paneles en la escena
```
Canvas (UI)
  ├─ panelMenu       → "Jugar" button
  ├─ panelHUD        → puntaje en tiempo real
  └─ panelGameOver   → score final + record + "Reintentar"
```

Solo uno está activo a la vez. Los otros tienen `SetActive(false)`.

### HUD en tiempo real (Update)
```csharp
// Cada frame mientras jugando:
string comboTexto = multi > 1f ? $"  x{multi:0}" : "";  // solo si combo activo
string recordTexto = puntos > record
    ? "  NUEVO RECORD!"        // verde neón mientras jugás
    : $"  Mejor: {record}";

textoPuntaje.text = $"Puntos: {puntos}{comboTexto}\nNivel: {nivel}  |  Esquivados: {esquivados}{recordTexto}";
```

**Interpolación de strings (`$"..."`):**
`$"Puntos: {puntos}"` → inserta el valor de `puntos` dentro del string.
Equivale a `"Puntos: " + puntos.ToString()` pero más legible.

**Rich Text en TextMeshPro:**
`<color=#FFD700>x2</color>` → el texto "x2" aparece en color dorado.
TMPro interpreta estas etiquetas HTML dentro del string.

### Game Over
```csharp
bool esRecord = puntos >= record && puntos > 0;

textoRecord.text = esRecord
    ? "<color=#FFD700>★ NUEVO RECORD: 500 ★</color>"
    : $"Record: {record}";
```

---

## Preguntas de autoevaluación

1. ¿Qué es `Time.deltaTime` y por qué se multiplica en el movimiento?
2. ¿Cuándo sube la velocidad? ¿Qué función lo hace y en qué script?
3. ¿Cómo funciona el combo? ¿Qué lo incrementa y qué lo resetea?
4. ¿Qué es PlayerPrefs y para qué se usa en este juego?
5. ¿Por qué hay 3 tiles de suelo y no 2?
6. ¿Qué son los paneles de UI y cómo se muestran/ocultan?

---

> **Siguiente:** ESTUDIO_05_UNITY_EDITOR.md
