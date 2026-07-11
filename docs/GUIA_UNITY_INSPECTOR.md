# Guía de Configuración en Unity — Lo que configuro Hebert

> Referencia rápida de todo lo que se toca en el EDITOR de Unity (no en código).
> Incluye cómo hacer ajustes comunes si el ing pide cambios.

---

## OBJETOS EN HIERARCHY Y SUS VALORES

---

### GameManager
- Componente: `GameManager.cs`
- No tiene valores en Inspector — todo es lógica interna

---

### LevelScroller
- Componente: `LevelScroller.cs`

| Campo Inspector | Valor actual | Para qué sirve |
|----------------|-------------|----------------|
| Velocidad Inicial | **8** | velocidad al empezar |
| Incremento Velocidad | **0.5** | cuánto sube cada 5s |
| Velocidad Maxima | **25** | tope de velocidad |
| Intervalo Incremento | **5** | segundos entre cada aumento |
| Limite Retorno | **-20** | Z donde recicla cactus al pool |

**Si el ing pide más velocidad inicial:** cambiar `Velocidad Inicial` de 8 a 10 o 12  
**Si pide que suba más rápido:** bajar `Intervalo Incremento` de 5 a 3  
**Si pide techo más alto:** subir `Velocidad Maxima` de 25 a 35  

---

### ObjectPooler
- Componente: `ObjectPooler.cs`

| Campo Inspector | Valor actual | Para qué sirve |
|----------------|-------------|----------------|
| Prefab Obstaculo | Obstaculo.prefab | el cactus que spawna |
| Tamano Pool | **10** | cuántos cactus existen |
| Distancia Spawn | **30** | Z donde aparece cada cactus |
| Intervalo Spawn | **2** | segundos entre cada cactus |

**Si pide más obstáculos seguidos:** bajar `Intervalo Spawn` de 2 a 1.5  
**Si pide menos obstáculos:** subir `Intervalo Spawn` de 2 a 3  

---

### AereoSpawner
- Componente: `AereoSpawner.cs`

| Campo Inspector | Valor actual | Para qué sirve |
|----------------|-------------|----------------|
| Prefab Aereo | ObstaculoAereo.prefab | el Pteranodon |
| Tamano Pool | **4** | cuántos Pteranodon existen |
| Distancia Spawn | **30** | Z donde aparece el ave |
| Intervalo Min | **6** | mínimo segundos entre aves |
| Intervalo Max | **12** | máximo segundos entre aves |
| Altura Vuelo | **2.5** | Y donde vuela el Pteranodon |

**Si el ave está muy alta o baja:** cambiar `Altura Vuelo`  
**Si pide que aparezca más seguido:** bajar `Intervalo Min` y `Intervalo Max`  

---

### AudioManager
- Componente: `AudioManager.cs`
- Necesita **2 AudioSources** en el mismo GameObject

| Campo Inspector | Archivo asignado |
|----------------|-----------------|
| Musica | ES_Powerwalkin - Future Joust.mp3 |
| Sonido Game Over | EMOTIONAL DAMAGE 1.mp3 |
| Sonido Salto | SonidoSalto.mp3 |
| Sonido Esquive | SonidoEsquive.mp3 |

**Cómo agregar un sonido nuevo:**
1. Arrastrá el .mp3 a Assets/Sounds/
2. En Inspector del AudioManager → arrastrá al campo correspondiente

---

### DecoracionLateral
- Componente: `DecoracionLateral.cs`

| Campo Inspector | Valor actual | Para qué sirve |
|----------------|-------------|----------------|
| Prefab Decoracion | CactusDecoracion.prefab | cactus lateral |
| Cantidad | **20** | tamaño del pool (pares) |
| Distancia Spawn | **35** | Z donde aparecen |
| Intervalo Min | **1.5** | mínimo segundos entre pares |
| Intervalo Max | **4** | máximo segundos entre pares |
| X Offset | **8** | distancia del centro (±8) |

**Si pide que los cactus laterales estén más separados:** subir `X Offset` a 10 o 12  
**Si pide que estén más juntos:** bajar `X Offset` a 5 o 6  

---

### Dino (jugador)
- Componentes: `Rigidbody` + `CapsuleCollider` + `DinoController.cs`

#### DinoController Inspector

| Campo Inspector | Valor actual | Para qué sirve |
|----------------|-------------|----------------|
| Fuerza Salto | **8** | altura del salto |
| Multiplicador Caida | **2.5** | qué tan rápido cae |
| Capa Suelo | Layer "Suelo" | qué detecta como piso |
| Punto Suelo | PuntoSuelo (Transform hijo) | desde dónde detecta el suelo |

**Si pide salto más alto:** subir `Fuerza Salto` de 8 a 10 o 12  
**Si pide salto más bajo:** bajar a 6  
**Si la caída se siente muy pesada:** bajar `Multiplicador Caida` de 2.5 a 1.8  

#### Rigidbody

| Propiedad | Valor |
|-----------|-------|
| Mass | 1 |
| Use Gravity | ✓ |
| Freeze Position | X=✓, Y=✗, Z=✓ (solo sube/baja) |
| Freeze Rotation | X=✓, Y=✓, Z=✓ (no rota) |

**Freeze Position X y Z:** el Dino no se mueve en X ni Z — solo el mundo se mueve,
no el jugador.

#### CapsuleCollider

| Propiedad | Valor |
|-----------|-------|
| Is Trigger | ✗ (es el collider físico del jugador) |
| Height | 2 |
| Center Y | 1 |

---

### Velociraptor (hijo del Dino)
- Componente: `Animator`

| Propiedad | Valor |
|-----------|-------|
| Controller | DinoAnimator.controller |
| Avatar | VelociraptorAvatar |

**El material verde (#39FF14) está en:**
Assets/Materials/TempDino.mat → Color = #39FF14

**Cómo cambiar el color del Dino:**
1. Assets/Materials/TempDino.mat → clic
2. Inspector → Base Map → clic en el cuadro de color
3. Cambiá el color

---

### PuntoSuelo (hijo del Dino)
- Es un **Transform vacío** — no tiene Mesh, no tiene Collider
- Posición relativa al Dino: X=0, Y=0, Z=0 (al ras del suelo)
- DinoController lo usa para `Physics.CheckSphere()` — detecta si hay suelo abajo

**Si el Dino flota o no salta bien:** verificar que PuntoSuelo esté en Y=0 relativo al Dino

---

### CamaraPrincipal
- Componentes: `Camera` + `CameraShake.cs`

| Propiedad | Valor |
|-----------|-------|
| Position | X=0, Y=12, Z=-15 |
| Rotation | X=15, Y=0, Z=0 |
| Background Type | Skybox |

**Si la cámara se ve muy lejos/cerca:** ajustar `Z` (más negativo = más lejos)  
**Si el ángulo se ve muy picado:** bajar `Rotation X` de 15 a 10  

---

### Suelo (tiles x3)
- Componente: `GroundScroller.cs` en uno de los tiles o en objeto separado
- Cada tile es un Plane de Unity con material `MaterialSuelo`
- Layer asignado: **Suelo** (para que DinoController lo detecte)

| Campo Inspector | Valor |
|----------------|-------|
| Tiles | [Tile1, Tile2, Tile3] |
| Longitud Tile | 50 |
| Limite Retorno | -60 |

**Si el piso se ve con gap entre tiles:** subir `Longitud Tile` a 55 o 60  
**Si quiere piso más ancho:** Scale X del Plane — cambiar de 1 a 2 en el Transform  
**Si quiere piso más largo:** Scale Z del Plane  

---

### Prefab Obstaculo (cactus principal)
- Mesh: `EA04_Env_Plants_Cactus_01a.fbx` o similar
- Tag: **Obstaculo** ← imprescindible
- BoxCollider con `Is Trigger = true`
- Material: MaterialObstaculo.mat (verde #3D7A3D)
- Generate Colliders en el FBX: **OFF** (apagado para evitar colliders extras)

---

### Prefab ObstaculoAereo (Pteranodon)
- Mesh: `Pteranodon.fbx`
- Tag: **ObstaculoAereo** ← imprescindible
- BoxCollider con `Is Trigger = true`
  - Center Y: **-1.5** (el collider baja para golpear al jugador parado, no al agachado)
- Material: MaterialAve.mat (marrón #8B4513)
- Componente: `ObstaculoAereo.cs`

**Si el ave mata al agacharse cuando no debería:** bajar más el `Center Y` del BoxCollider (ej: -2)  
**Si el ave no mata al estar parado:** subir el `Center Y` (ej: -1)  

---

### Prefab CactusDecoracion (lateral)
- Mismo mesh que el cactus principal
- **SIN BoxCollider** — no mata, solo visual
- Tag: ninguno especial
- Material: MaterialObstaculo.mat

---

## CONFIGURACIONES DE PROYECTO

### Tags (Edit → Project Settings → Tags and Layers)
```
Tags: Obstaculo, ObstaculoAereo, Scrolleable, Player
Layers: Suelo (usado para capaSuelo en DinoController)
```

### Skybox (Window → Rendering → Lighting → Environment)
```
Skybox Material: SkyboxDesert.mat (shader: Mobile/Skybox)
```

### AnimatorController — DinoAnimator
```
Parámetros:
  IsRunning  (Bool)
  isCrouching (Bool)
  IsGrounded (Bool)
  Morir      (Trigger)

Transiciones:
  idle → running: IsRunning=true, Has Exit Time=OFF, Duration=0.1
  running → idle: IsRunning=false, Has Exit Time=OFF, Duration=0.1
  running → dying: Morir trigger, Has Exit Time=OFF
```

---

## CAMBIOS RÁPIDOS MÁS PEDIDOS

| Si el ing pide... | Dónde cambiarlo | Campo |
|-------------------|----------------|-------|
| Salto más alto | Dino → DinoController | Fuerza Salto |
| Velocidad inicial más rápida | LevelScroller | Velocidad Inicial |
| Más obstáculos | ObjectPooler | Intervalo Spawn (bajar) |
| Ave más frecuente | AereoSpawner | Intervalo Min/Max (bajar) |
| Ave más alta | AereoSpawner | Altura Vuelo |
| Cactus laterales más separados | DecoracionLateral | X Offset |
| Color del Dino | TempDino.mat | Base Map color |
| Piso más ancho | Tiles Plane | Transform Scale X |
| Cámara más arriba | CamaraPrincipal | Position Y |
| Ave no mata al agacharse | ObstaculoAereo prefab BoxCollider | Center Y (más negativo) |
