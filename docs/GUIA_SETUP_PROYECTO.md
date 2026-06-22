# Guía de Setup — Runner3D en Unity 6
### Para continuar el proyecto desde cero o retomarlo en otro PC

---

## ÍNDICE
1. [Requisitos e Instalación](#1-requisitos-e-instalación)
2. [Crear el Proyecto en Unity Hub](#2-crear-el-proyecto-en-unity-hub)
3. [Configuración Inicial del Proyecto](#3-configuración-inicial-del-proyecto)
4. [Estructura de Carpetas](#4-estructura-de-carpetas)
5. [Configurar la Escena Base](#5-configurar-la-escena-base)
6. [Configurar el Personaje (Dino)](#6-configurar-el-personaje-dino)
7. [Crear el Script DinoController](#7-crear-el-script-dinocontroller)
8. [Subir el Proyecto a GitHub](#8-subir-el-proyecto-a-github)
9. [Clonar y Abrir el Proyecto en Otro PC](#9-clonar-y-abrir-el-proyecto-en-otro-pc)
10. [Próximos Scripts a Crear](#10-próximos-scripts-a-crear)

---

## 1. Requisitos e Instalación

### Instalar Unity Hub
1. Ir a **unity.com/download**
2. Descargar Unity Hub para **Windows x64**
3. Ejecutar el instalador `.exe`
4. Abrir Unity Hub → crear cuenta Unity gratis → iniciar sesión

### Instalar Unity 6.3 LTS (o 6.5)
1. En Unity Hub → menú izquierdo → **Installs**
2. Clic **Install Editor**
3. Seleccionar **Unity 6.3 LTS** → clic **Install**
4. Marcar estos módulos:
   - ✅ Microsoft Visual Studio Community 2022 (o VS Code)
   - ✅ Windows Build Support (IL2CPP)
5. Esperar descarga (~4-6 GB)

> **Nota:** Si aparece error de permisos al instalar, cerrar Unity Hub y abrirlo como **Administrador** (clic derecho → Ejecutar como administrador).

---

## 2. Crear el Proyecto en Unity Hub

1. Unity Hub → menú izquierdo → **Projects**
2. Clic **New project** (botón azul arriba derecha)
3. Configurar:
   - **Template:** `Universal 3D` ← el que tiene el ícono azul con "SRP" (NO el 3D básico, NO el High Definition)
   - **Project name:** `Runner3D`
   - **Location:** `C:\Users\USUARIO\Documents\UNIVERSIDAD\Programacion_Grafica\Feria_Expo\`
   - **Source control provider:** dejar vacío
4. Clic **+ Create project**
5. Esperar ~2-3 minutos mientras Unity configura

### Errores comunes al abrir por primera vez

| Error | Solución |
|---|---|
| "Unity Package Manager Error — Access denied" | Clic **Continue**, luego cerrar y abrir Unity Hub como administrador |
| "Enter Safe Mode?" | Clic **Ignore** |
| "Unity is running as administrator" | Clic **I wish to continue at my own risk** |
| "Auto Graphics API Notice" | Clic **Confirm** |
| "URP Material upgrade" | Clic **OK** |

---

## 3. Configuración Inicial del Proyecto

### Verificar que URP está activo
`Edit → Project Settings → Graphics` → debe mostrar un asset URP asignado. Si está vacío, el proyecto falló — recrear con template "Universal 3D".

### Configurar Active Input Handling (IMPORTANTE)
Unity 6 usa New Input System por defecto. Los scripts usan ese sistema. Si hay errores de Input:
1. `Edit → Project Settings → Player`
2. Expandir **Other Settings**
3. Buscar **Active Input Handling**
4. Cambiar a **"Both"** (o dejar en "Input System Package (New)")
5. Unity pedirá reiniciar → aceptar

### Configurar Tags necesarios
1. `Edit → Project Settings → Tags and Layers`
2. Expandir **Tags** → clic **+**
3. Agregar: `Obstaculo`
4. `Ctrl+S`

### Configurar Layer "Ground"
1. En misma ventana Tags and Layers
2. En **Layers** → buscar primer slot vacío (ej: User Layer 6)
3. Escribir: `Ground`
4. `Ctrl+S`

---

## 4. Estructura de Carpetas

En el panel **Project** (abajo), dentro de `Assets`, crear estas carpetas:

Clic derecho en área vacía → **Create → Folder**

```
Assets/
├── Animations/       ← animaciones del personaje
├── Audio/            ← música y efectos de sonido
├── Materials/        ← materiales y colores
├── Models/           ← modelos 3D importados
├── Prefabs/          ← objetos prefabricados (obstáculos, suelo)
├── Resources/        ← ya viene creada, no tocar
├── Scenes/           ← ya viene creada
├── Scripts/          ← todos los scripts C#
└── Settings/         ← ya viene creada, no tocar
```

---

## 5. Configurar la Escena Base

### Crear la escena del juego
1. En panel Project → carpeta `Scenes`
2. Clic derecho → **Create → Scene** → nombre `GameScene`
3. Doble clic en `GameScene` para abrirla

### Agregar el Suelo
1. Menú superior → **GameObject → 3D Object → Plane**
2. En Inspector → **Transform**:
   - Position: X=`0`, Y=`0`, Z=`0`
   - Scale: X=`5`, Y=`1`, Z=`50`
3. Renombrar: clic en "Plane" en Hierarchy → **F2** → escribir `Suelo` → Enter
4. Asignar Layer: en Inspector arriba → `Layer: Default` → dropdown → seleccionar `Ground`
5. Si pregunta "Change children?" → **No, this object only**

### Agregar el Personaje (Dino)
1. **GameObject → 3D Object → Capsule**
2. **Transform**:
   - Position: X=`0`, Y=`1`, Z=`0`
   - Scale: X=`1`, Y=`1`, Z=`1`
3. Renombrar a `Dino` (F2)

### Renombrar objetos existentes de la escena
Seleccionar cada objeto en Hierarchy → F2 → escribir nuevo nombre:

| Nombre original | Nuevo nombre |
|---|---|
| Main Camera | CamaraPrincipal |
| Directional Light | LuzDireccional |

### Resultado final en Hierarchy
```
GameScene
├── CamaraPrincipal
├── LuzDireccional
├── Suelo              (Layer: Ground, Scale 5,1,50)
└── Dino              (Position 0,1,0 / Scale 1,1,1)
    └── DetectorSuelo  (Position 0,-1,0)
```

---

## 6. Configurar el Personaje (Dino)

### Agregar Rigidbody al Dino
1. Clic en `Dino` en Hierarchy
2. Inspector → **Add Component** → buscar `Rigidbody` → seleccionar
3. En el Rigidbody → expandir **Constraints**:
   - ✅ Freeze Rotation X
   - ✅ Freeze Rotation Y
   - ✅ Freeze Rotation Z

### Crear DetectorSuelo (hijo del Dino)
1. Clic derecho en `Dino` en Hierarchy → **Create Empty**
2. Renombrar a `DetectorSuelo` (F2)
3. Transform del DetectorSuelo:
   - Position: X=`0`, Y=`-1`, Z=`0`

### Posicionar la Cámara
1. Clic en `CamaraPrincipal` en Hierarchy
2. Inspector → Transform:
   - Position: X=`0`, Y=`6`, Z=`-3`
   - Rotation: X=`45`, Y=`0`, Z=`0`

### Crear material de color para el Dino
1. Clic en carpeta `Materials` en Project
2. Clic derecho → **Create → Material** → nombre `MaterialDino`
3. Inspector → clic en rectángulo blanco junto a **Base Map**
4. Elegir color (verde, rojo, lo que prefieras)
5. Arrastrar `MaterialDino` desde Project al `Dino` en Hierarchy

---

## 7. Crear el Script DinoController

### Crear el archivo
1. Clic en carpeta `Scripts` en Project
2. Clic derecho → **Create → MonoBehaviour Script**
3. Nombre: `DinoController` (exacto, sin espacios)
4. Doble clic → abre VS Code

### Código completo — DinoController.cs
Borrar todo el contenido y pegar esto:

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DinoController : MonoBehaviour
{
    [Header("Fisicas de Salto")]
    [SerializeField] private float fuerzaSalto = 8f;
    [SerializeField] private float multiplicadorCaida = 2.5f;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform puntoSuelo;

    private Rigidbody rb;
    private bool estaEnSuelo;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        estaEnSuelo = Physics.CheckSphere(puntoSuelo.position, 0.2f, capaSuelo);

        bool salto = Keyboard.current.spaceKey.wasPressedThisFrame ||
                     Keyboard.current.wKey.wasPressedThisFrame ||
                     Keyboard.current.upArrowKey.wasPressedThisFrame;

        if (salto && estaEnSuelo)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerzaSalto, rb.linearVelocity.z);
        }
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            Debug.Log("GAME OVER");
        }
    }
}
```

`Ctrl+S` en VS Code → volver a Unity → esperar compilación.

### Asignar el script al Dino
1. Clic en `Dino` en Hierarchy
2. Inspector → **Add Component** → buscar `DinoController` → seleccionar
3. Configurar los campos que aparecen:
   - **Capa Suelo:** clic dropdown → seleccionar `Ground`
   - **Punto Suelo:** arrastrar `DetectorSuelo` desde Hierarchy al campo

### Verificar
- Console (abajo) → 0 errores rojos ✓
- ▶ Play → presionar **Espacio** → Dino salta ✓

---

## 8. Subir el Proyecto a GitHub

### Instalar Git (si no lo tienes)
Descargar desde **git-scm.com** → instalar con opciones por defecto.

### Crear .gitignore para Unity
Antes de subir, crear archivo `.gitignore` en la raíz del proyecto (`Runner3D/`) para no subir archivos innecesarios (Library, Temp, etc. pesan varios GB).

En VS Code, crear archivo `.gitignore` en la carpeta `Runner3D/` con este contenido:

```
# Carpetas generadas automáticamente (se recrean solas)
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/

# Archivos de Visual Studio
.vs/
*.csproj
*.unityproj
*.sln
*.suo
*.tmp
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db

# Archivos de sistema
.DS_Store
Thumbs.db
```

### Crear repositorio en GitHub
1. Ir a **github.com** → iniciar sesión → **New repository**
2. Nombre: `Runner3D-Unity`
3. Privado o público (privado recomendado para trabajo de equipo)
4. **NO** marcar "Add README" ni ".gitignore" (ya los tenemos)
5. Clic **Create repository**
6. Copiar la URL del repo (ej: `https://github.com/tuusuario/Runner3D-Unity.git`)

### Subir desde terminal
Abrir terminal en la carpeta del proyecto `Runner3D/`:

```bash
git init
git add .
git commit -m "Setup inicial: escena base, DinoController con salto"
git branch -M main
git remote add origin https://github.com/TUUSUARIO/Runner3D-Unity.git
git push -u origin main
```

> **La primera vez** pedirá login de GitHub — ingresar usuario y contraseña (o token).

---

## 9. Clonar y Abrir el Proyecto en Otro PC

### El compañero necesita
- Git instalado
- Unity Hub instalado
- Unity 6.5 instalado (misma versión)

### Pasos para clonar
1. Abrir terminal en la carpeta donde quiere guardar el proyecto
2. Ejecutar:
```bash
git clone https://github.com/TUUSUARIO/Runner3D-Unity.git
```
3. Abrir Unity Hub → **Projects → Add → Add project from disk**
4. Navegar hasta la carpeta clonada `Runner3D-Unity/` → seleccionar
5. Unity abrirá el proyecto y reimportará activos automáticamente (~2-3 min)

> **Importante:** La carpeta `Library/` se regenera sola. Si aparece error de paquetes al abrir, clic **Continue** y esperar que Unity resuelva las dependencias.

### Trabajar en paralelo (flujo de trabajo)
Cada persona trabaja en su script propio y hace push/pull:

```bash
# Antes de empezar a trabajar cada día
git pull origin main

# Después de terminar cambios
git add Assets/Scripts/MiScript.cs
git commit -m "feat: agregar sistema de puntuacion"
git push origin main
```

---

## 10. Próximos Scripts a Crear

Estado actual del proyecto:
- ✅ Escena base configurada
- ✅ Suelo con Layer Ground
- ✅ Dino con Rigidbody + DinoController
- ✅ Salto funcional con Espacio/W/↑

Scripts pendientes por persona:

### P2 — LevelScroller.cs (mundo infinito)
```csharp
using UnityEngine;

public class LevelScroller : MonoBehaviour
{
    [SerializeField] private float velocidad = 8f;

    void Update()
    {
        transform.Translate(Vector3.back * velocidad * Time.deltaTime);
    }
}
```
Asignar este script a cada obstáculo (cubo). El obstáculo se mueve solo hacia el Dino.

### P2 — ObjectPooler.cs (reciclaje de obstáculos)
Ver código completo en `Runner3D_Documentacion_Final.md` → Anexo C.

### P3 — GameManager.cs (control de estados)
- Singleton accesible globalmente
- Estados: Menu, Playing, GameOver
- Puntuación incremental por tiempo
- Llamar `GameManager.Instance.TriggerGameOver()` desde DinoController

### P3 — UIManager.cs (interfaz)
- Texto de puntuación en pantalla
- Panel de Game Over con botón Reiniciar

### P5 — CinemachineShake.cs (cámara)
- Instalar Cinemachine desde Package Manager
- Sacudida de cámara al morir

---

## Notas Importantes

**Errores frecuentes en Unity 6:**

| Error | Causa | Solución |
|---|---|---|
| `InvalidOperationException: UnityEngine.Input` | Script usa Input clásico, Unity 6 usa New Input System | Usar `Keyboard.current.spaceKey.wasPressedThisFrame` |
| `Tag is not defined` | Tag no creado en Project Settings | Edit → Project Settings → Tags → agregar tag |
| `NullReferenceException: puntoSuelo` | Campo no asignado en Inspector | Arrastrar DetectorSuelo al campo Punto Suelo del DinoController |
| Dino invisible | Scale en 0 | Inspector → Transform → Scale = 1,1,1 |
| Cambios no persisten | Se hicieron en Play mode | Hacer cambios SIEMPRE con Play detenido (■) |

**Regla de oro:** Siempre `Ctrl+S` después de cualquier cambio en la escena o en scripts.
