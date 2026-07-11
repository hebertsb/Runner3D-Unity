# ESTUDIO 06 — Fórmulas y Cálculos del Juego

> Cómo interpretar los números detrás de cada mecánica.

---

## 1. ¿Cada cuánto aparece el siguiente cactus?

### Fórmula base
```
Tiempo entre cactus = intervaloSpawn = 2 segundos
```

### Pero hay condiciones que lo modifican:

**Caso normal (sin ave):**
```
Cactus 1 spawna → timer = 0
timer sube cada frame (+Time.deltaTime)
timer llega a 2.0 → Cactus 2 spawna
→ cada 2 segundos exactos
```

**Caso post-ave:**
```
Ave spawna → PausarPorSegundos(8)
  pausaTimer = 8.0
  timerSpawn = 0  ← resetea

Durante 8 segundos: NO spawna nada (pausaTimer > 0)
A los 8s: pausaTimer llega a 0 → timer empieza a correr
A los 8 + 2 = 10s desde que spawneó el ave → primer cactus post-ave
```

**Resumen:**
| Situación | Tiempo hasta próximo cactus |
|-----------|---------------------------|
| Normal | 2 segundos |
| Justo después de un ave | 8 + 2 = **10 segundos** |
| Si `HayAveActiva = true` | Indefinido (bloqueado) |

---

## 2. ¿Cada cuánto aparece el Pteranodon?

### Fórmula base
```
intervaloActual = Random.Range(intervaloMin, intervaloMax)
                = Random.Range(6, 12)
                = número aleatorio entre 6 y 12 segundos
```

`Random.Range(6, 12)` genera un número distinto cada vez.
Ejemplos posibles: 7.3s, 11.1s, 6.8s, 9.5s...

### Pero hay una condición extra: pista vacía

```csharp
// Si hay cactus con Z > -2 (todavía en pista):
timer = intervaloActual - 1f  // reintenta en 1 segundo

// Ejemplo:
intervaloActual = 8f
timer llega a 8 → intenta spawnar → hay cactus en Z=5 → NO spawna
timer = 8 - 1 = 7f
1 segundo después → timer llega a 8 de nuevo → chequea de nuevo
→ repite hasta que la pista esté vacía
```

**Resumen:** El ave aparece entre 6 y 12 segundos, pero puede **retrasarse**
si hay cactus en pista. Nunca es exacto — es intencional para que no sea predecible.

---

## 3. ¿Cómo el Pteranodon vuela y cuánto tiempo está en pantalla?

### Movimiento
```csharp
// Cada frame en ObstaculoAereo.Update():
transform.Translate(Vector3.back * VelocidadActual * Time.deltaTime)
```

Esto significa: **se mueve hacia el jugador** a la misma velocidad del juego.

### Cálculo del tiempo en pantalla
```
Distancia total = spawna en Z=30, muere en Z=-25
Distancia = 30 - (-25) = 55 unidades

Tiempo en pantalla = Distancia / Velocidad

Con vel=8:   55 / 8  = 6.9 segundos
Con vel=12:  55 / 12 = 4.6 segundos
Con vel=20:  55 / 20 = 2.75 segundos
```

A mayor velocidad del juego → el ave pasa más rápido → menos tiempo para reaccionar.

### ¿Por qué "vuela infinitamente"?
No vuela infinito — vuela 55 unidades y se desactiva.
Parece continuo porque:
1. Se desactiva al llegar a Z=-25
2. Vuelve al pool (SetActive(false))
3. El próximo ciclo (6-12s) spawna otro en Z=30 de nuevo

---

## 4. ¿Cuánto dura el Pteranodon en peligro para el jugador?

El jugador está en Z≈0. El ave golpea cuando está cerca de Z=0.

```
Peligro ≈ mientras ave está entre Z=3 y Z=-3 (zona del jugador)
Distancia de peligro = 6 unidades

Tiempo de peligro:
  vel=8:   6 / 8  = 0.75 segundos para reaccionar
  vel=12:  6 / 12 = 0.5 segundos
  vel=20:  6 / 20 = 0.3 segundos
```

→ A mayor velocidad, el jugador tiene MENOS tiempo para agacharse.

---

## 5. ¿Cómo se calcula la altura del salto?

### Fórmula de física (cinemática)
```
Altura máxima = v² / (2 × g)

v = fuerzaSalto = 8 m/s (velocidad inicial hacia arriba)
g = 9.81 m/s² (gravedad de Unity)

Altura = 8² / (2 × 9.81)
       = 64 / 19.62
       = 3.26 unidades de Unity
```

Pero la **caída** es más rápida porque el multiplicador la amplifica:

```
Gravedad en caída = Physics.gravity.y × (multiplicadorCaida - 1)
                  = -9.81 × (2.5 - 1)
                  = -9.81 × 1.5
                  = -14.7 m/s² extra (se suma a la gravedad normal)

Gravedad efectiva en caída = -9.81 + (-14.7) = -24.5 m/s²
```

→ Sube a 3.26u con gravedad normal, cae con 2.5x la gravedad normal.
Resultado: subida suave, caída rápida. Se siente "arcade".

---

## 6. ¿Cómo funciona el Coyote Time?

```
Situación: jugador corre y llega al borde de algo

Frame N:   está en suelo → enSueloFisico=true → timerCoyote = 0.15
Frame N+1: sale del borde → enSueloFisico=false → timerCoyote = 0.15 - deltaTime
Frame N+2: sigue en aire → timerCoyote = 0.15 - 2×deltaTime
...
Frame N+9: timerCoyote ≈ 0.0 → estaEnSuelo = false

Si el jugador presiona Space en Frame N+3 (todavía coyote time activo):
  estaEnSuelo = true (timer > 0) → SALTO PERMITIDO ✓
```

**En números:**
```
deltaTime a 60fps = 0.0167s
0.15 / 0.0167 = ~9 frames de gracia

El jugador tiene 9 frames para saltar después de salir del borde.
= 150 milisegundos
```

---

## 7. ¿Cómo se calculan los puntos?

### Fórmula exacta (GameManager.Update cada frame)
```
Puntos += VelocidadActual × ComboMultiplicador × Time.deltaTime
```

### Ejemplo concreto — 1 segundo de juego
```
vel=8,  combo x1: 8  × 1 × 1s = 8  puntos
vel=8,  combo x2: 8  × 2 × 1s = 16 puntos
vel=12, combo x2: 12 × 2 × 1s = 24 puntos
vel=20, combo x3: 20 × 3 × 1s = 60 puntos
```

### Por qué se multiplica por Time.deltaTime
```
Sin deltaTime:
  60fps → += 12 × 60 veces/seg = 720 puntos/seg
  30fps → += 12 × 30 veces/seg = 360 puntos/seg
  → INJUSTO: a más fps, más puntos

Con deltaTime:
  60fps → += 12 × (1/60) × 60 = 12 puntos/seg
  30fps → += 12 × (1/30) × 30 = 12 puntos/seg
  → IGUAL sin importar los fps
```

---

## 8. ¿Cómo se calcula el Nivel?

```
NivelActual = floor((VelocidadActual - 6) / 2) + 1

Desglose:
  VelocidadActual - 6  → resta la velocidad base
  / 2                  → cada 2 unidades = 1 nivel
  floor(...)           → redondea hacia abajo (sin decimales)
  + 1                  → el nivel mínimo es 1 (no 0)

Ejemplos:
  vel=8:  floor((8-6)/2)  + 1 = floor(1)   + 1 = 2
  vel=9:  floor((9-6)/2)  + 1 = floor(1.5) + 1 = 2
  vel=10: floor((10-6)/2) + 1 = floor(2)   + 1 = 3
  vel=25: floor((25-6)/2) + 1 = floor(9.5) + 1 = 10
```

---

## 9. ¿Cómo se recicla el suelo infinitamente?

```
3 tiles, cada uno de longitud 50 unidades.
Se mueven hacia Z negativo.

Cuando un tile llega a Z < -60:
  maxZ = posición del tile más adelante (ej: Z=40)
  tile.position.z = maxZ + 50 = 90

Efecto: el tile "salta" al frente del más adelantado.
El jugador ve el suelo continuo porque nunca hay gap.
```

**Condición para que no haya gap:**
```
Necesitamos que siempre haya tile cubriendo Z=0 (posición del jugador).
Con 3 tiles de 50u: cobertura total = 150u
Tile retorno en -60 → el tile más adelante siempre está en al menos Z=40
→ siempre hay suelo entre Z=-60 y Z=90 → el jugador en Z=0 siempre tiene suelo ✓
```

---

## Preguntas de autoevaluación

1. Si vel=15 y combo x2, ¿cuántos puntos gana el jugador en 3 segundos?
2. ¿Cuánto tiempo está el Pteranodon en pantalla si la velocidad es 10?
3. ¿Cuántos frames de gracia tiene el jugador para saltar con coyote time a 60fps?
4. Si el ing pide que el ave aparezca EXACTAMENTE cada 5 segundos, ¿qué cambiás?
5. ¿Por qué a mayor velocidad el juego se vuelve más difícil? (pensá en los tiempos de reacción)
6. ¿Por qué se necesita multiplicar por `Time.deltaTime` en el movimiento?
