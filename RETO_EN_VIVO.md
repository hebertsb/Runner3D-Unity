# RETO EN VIVO — Cambios Rápidos en Unity

> Abrí este archivo durante el reto. Buscá lo que pide el ing y hacelo.

---

## VELOCIDAD

**"Hacé que el juego empiece más rápido"**
```
Hierarchy → LevelScroller → Inspector
Campo: Velocidad Inicial → subir de 8 a 12
```

**"Que la velocidad suba más rápido"**
```
Hierarchy → LevelScroller → Inspector
Campo: Intervalo Incremento → bajar de 5 a 3
```

**"Que llegue a más velocidad máxima"**
```
Hierarchy → LevelScroller → Inspector
Campo: Velocidad Maxima → subir de 25 a 40
```

---

## SALTO

**"Hacé que el dinosaurio salte más alto"**
```
Hierarchy → Dino → Inspector → DinoController
Campo: Fuerza Salto → subir de 8 a 12
```

**"Que caiga más lento"**
```
Hierarchy → Dino → Inspector → DinoController
Campo: Multiplicador Caida → bajar de 2.5 a 1.5
```

---

## OBSTÁCULOS CACTUS

**"Que aparezcan más seguido"**
```
Hierarchy → ObjectPooler → Inspector
Campo: Intervalo Spawn → bajar de 2 a 1
```

**"Que aparezcan más lejos/cerca"**
```
Hierarchy → ObjectPooler → Inspector
Campo: Distancia Spawn → subir/bajar (actual: 30)
```

---

## AVE PTERANODON

**"Que el ave aparezca más seguido"**
```
Hierarchy → AereoSpawner → Inspector
Campos: Intervalo Min → 3 | Intervalo Max → 6
```

**"Que el ave vuele más alto"**
```
Hierarchy → AereoSpawner → Inspector
Campo: Altura Vuelo → subir de 2.5 a 4
```

**"Que el ave mate al agacharse también"**
```
Project → Prefabs → ObstaculoAereo → Inspector → BoxCollider
Campo: Center Y → subir de -1.5 a -0.5
```

**"Que el ave NO mate al estar parado"**
```
Project → Prefabs → ObstaculoAereo → Inspector → BoxCollider
Campo: Center Y → bajar de -1.5 a -3
```

---

## CACTUS LATERALES

**"Que los cactus del costado estén más separados"**
```
Hierarchy → DecoracionLateral → Inspector
Campo: X Offset → subir de 8 a 12
```

**"Que aparezcan más seguido"**
```
Hierarchy → DecoracionLateral → Inspector
Campos: Intervalo Min → 0.5 | Intervalo Max → 1.5
```

---

## COLOR / VISUAL

**"Cambiá el color del dinosaurio"**
```
Project → Assets/Materials → TempDino.mat → clic
Inspector → Base Map → clic en el cuadro de color → cambiá
```

**"Cambiá el color del suelo"**
```
Project → Assets/Materials → MaterialSuelo.mat → clic
Inspector → Base Map → clic en el cuadro de color → cambiá
```

---

## CÁMARA

**"Cambiá el ángulo de la cámara"**
```
Hierarchy → CamaraPrincipal → Inspector → Transform
Position Y → subir/bajar (actual: 12)
Rotation X → subir/bajar (actual: 15)
```

---

## PUNTAJE / COMBO

**"Que el combo empiece antes (con menos obstáculos)"**
```
Abrir: Assets/Scripts/GameManager.cs
Línea: const int COMBO_NIVEL2 = 5  → cambiar a 3
Línea: const int COMBO_NIVEL3 = 10 → cambiar a 6
Guardá → Unity recompila solo
```

---

## PASOS GENERALES PARA CUALQUIER CAMBIO

```
1. Identificar el objeto en Hierarchy (o script en Project/Scripts)
2. Clic en el objeto
3. Inspector → encontrar el campo
4. Cambiar el valor
5. Presionar Play para probar
6. Si está bien → guardá la escena (Ctrl+S)
```
