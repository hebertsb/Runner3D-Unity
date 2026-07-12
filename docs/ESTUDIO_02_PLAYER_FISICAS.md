# ESTUDIO 02 — Jugador y Físicas (DinoController)

> Este es el script más técnico y el más preguntado en un examen.

---

## Componentes que necesita el Dino en Unity

En la **Hierarchy**, el Dino tiene esta estructura:

```
Dino (GameObject vacío)
  ├─ Rigidbody          → física: gravedad, velocidad
  ├─ CapsuleCollider    → hitbox del jugador (se achica al agacharse)
  ├─ DinoController.cs  → script de control
  ├─ PuntoSuelo         → Transform hijo invisible, posicionado al ras del suelo
  └─ Velociraptor (hijo)
       ├─ Mesh del modelo 3D
       └─ Animator       → animaciones (corriendo, muriendo, etc.)
```

**Por qué el Animator está en el hijo y no en el padre:**
`GetComponentInChildren<Animator>()` lo busca en los hijos. Si estuviera en el padre
usaríamos `GetComponent<Animator>()`. El modelo 3D importado tiene su propio objeto.

---

## Sistema de Salto — paso a paso

### Paso 1: Detectar si está en el suelo
```csharp
bool enSueloFisico = Physics.CheckSphere(puntoSuelo.position, 0.2f, capaSuelo);
```
- `puntoSuelo` → Transform hijo ubicado a los pies del Dino
- `0.2f` → radio de la esfera de detección
- `capaSuelo` → LayerMask — solo detecta objetos en la capa "Suelo"

**Por qué LayerMask:** Sin ella, la esfera detectaría al propio Dino u otros objetos,
dando falsos positivos.

### Paso 2: Coyote Time (0.15 segundos)
```csharp
if (enSueloFisico)
    timerCoyote = 0.15f;   // mientras toca suelo, timer en máximo
else
    timerCoyote -= Time.deltaTime;  // en el aire, cuenta hacia 0

estaEnSuelo = timerCoyote > 0f;
```

**¿Qué es coyote time?**
Si el jugador presiona saltar justo cuando sale del borde de una plataforma,
la física puede no detectar el suelo en ese frame exacto y el salto se ignora.
Con 0.15s de gracia, el jugador puede saltar hasta 0.15 segundos DESPUÉS de salir del suelo.

**Resultado:** salto más responsivo, no se "pega".

### Paso 3: Aplicar el salto
```csharp
bool salto = Keyboard.current.spaceKey.wasPressedThisFrame || ...;

if (salto && estaEnSuelo)
{
    timerCoyote = 0f;   // cancela la gracia para no saltar doble
    rb.linearVelocity = new Vector3(vel.x, fuerzaSalto, vel.z);
    // fuerzaSalto = 8f → impulso hacia arriba
}
```

**`wasPressedThisFrame` vs `isPressed`:**
- `wasPressedThisFrame` → true solo el PRIMER frame que se presiona (un disparo)
- `isPressed` → true mientras el botón esté apretado (continuo)

El salto usa `wasPressedThisFrame` porque no queremos salto continuo.

### Paso 4: Gravedad extra en caída
```csharp
// En FixedUpdate (física):
if (rb.linearVelocity.y < 0)   // si está cayendo
    rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
    // multiplicadorCaida = 2.5 → caída 1.5x más rápida que la subida
```

**Por qué:** Sin esto, el salto se siente "flotante". La caída más rápida
da sensación de peso real.

**Update vs FixedUpdate:**
- `Update()` → cada frame (input, animaciones, lógica)
- `FixedUpdate()` → tasa fija (50 veces/seg) → siempre para física con Rigidbody

---

## Sistema de Agacharse

### Teclas detectadas
```csharp
bool agachado = jugando && !salto && (
    Keyboard.current.sKey.isPressed ||        // S
    Keyboard.current.downArrowKey.isPressed || // ↓
    Keyboard.current.leftCtrlKey.isPressed);   // LCtrl
```

**`!salto` es clave:** Si no estuviera, al presionar Space mientras se agacha,
`salto` sería true pero `agachado` también sería true en el mismo frame,
y el capsule no volvería al tamaño normal para que el salto funcione.

### Cambio de hitbox
```csharp
// En Start(), guarda los valores originales del CapsuleCollider:
alturaColNormal   = capsule.height;      // ej: 2.0
centroNormal      = capsule.center;      // ej: Y=1.0
alturaColAgachado = capsule.height * 0.5f; // ej: 1.0
centroAgachado    = center con Y * 0.5f;   // ej: Y=0.5

// En Update(), cambia según estado:
capsule.height = agachado ? alturaColAgachado : alturaColNormal;
capsule.center = agachado ? centroAgachado    : centroNormal;
```

**Por qué se lee en Start() y no se hardcodea:**
Si cambiás el CapsuleCollider en el Inspector más tarde,
el código se adapta solo sin tener que cambiar números en el script.

**Por qué cambia el centro además de la altura:**
Si solo achicás la altura, el collider se encoge hacia abajo y el Dino
se hunde en el suelo. Al bajar el centro, el collider encoge hacia arriba
manteniendo la base en el mismo lugar.

---

## Detección de colisión con obstáculos

```csharp
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Obstaculo") || other.CompareTag("ObstaculoAereo"))
    {
        CameraShake.Instance?.Shake(0.3f, 0.4f);
        animator.SetTrigger("Morir");
        GameManager.Instance.TriggerGameOver();
    }
}
```

**OnTriggerEnter vs OnCollisionEnter:**
- `OnCollisionEnter` → colisión física real (rebote, fricción)
- `OnTriggerEnter` → detección sin física (pasa a través pero dispara el evento)

Los obstáculos tienen su collider como **Trigger** = true, por eso
el Dino no rebota con ellos sino que los "atraviesa" y muere.

**`?.` (operador null-condicional):**
`CameraShake.Instance?.Shake(...)` → si Instance es null, no ejecuta nada
(evita NullReferenceException si el componente no está en la escena).

---

## Animaciones — parámetros del Animator

| Parámetro | Tipo | Cuándo es true |
|-----------|------|----------------|
| `IsRunning` | Bool | Estado == Jugando |
| `isCrouching` | Bool | S/↓/LCtrl presionado |
| `IsGrounded` | Bool | estaEnSuelo == true |
| `Morir` | Trigger | Al colisionar con obstáculo |

**Bool vs Trigger:**
- Bool → estado continuo (corriendo o no corriendo)
- Trigger → disparo único (se activa una vez y se resetea solo)

---

## Preguntas de autoevaluación

1. ¿Por qué el Animator está en el hijo y no en el Dino directamente?
2. ¿Qué es coyote time y por qué mejora la jugabilidad?
3. ¿Qué diferencia hay entre `Update()` y `FixedUpdate()`? ¿Por qué la gravedad va en FixedUpdate?
4. ¿Por qué se usa `wasPressedThisFrame` para el salto y `isPressed` para agacharse?
5. ¿Qué pasa en el CapsuleCollider cuando el jugador se agacha? ¿Por qué cambia el centro también?
6. ¿Qué diferencia hay entre OnTriggerEnter y OnCollisionEnter?

---

> **Siguiente:** ESTUDIO_03_OBSTACULOS_POOL.md
