# 🧭 CoursesGn — Générateur de Cartes de Navigation

Application console .NET 9.0 qui génère des **cartes de navigation fictives** au format A4 (JPG ou PDF).  
Chaque carte contient un parcours de flèches colorées connectées bout-à-bout dans les 8 directions cardinales, avec une boussole et des indications de couleurs.

Idéal pour des **jeux de piste**, des **exercices d'orientation** ou des **activités pédagogiques**.

---

## 🎯 Fonctionnalités

- **8 directions cardinales** : N, NE, E, SE, S, SO, O, NO
- **6 couleurs de flèches** : Bleu, Rouge, Vert, Orange, Violet, Jaune
- **3 styles de flèches** : Clean, Dessiné à la main, Pointillé
- **5 niveaux de difficulté** avec des rendus progressivement plus complexes
- **Formats de sortie** : JPG (150 DPI) et PDF vectoriel
- **Boussole embarquée** (avec et sans points cardinaux)
- **Police custom** : support de polices `.ttf` / `.otf` embarquées
- **Anti-boucle** : règles de proximité et de virage pour éviter les recoupements

## 🎮 Niveaux de difficulté

| Niveau | Nom | Description |
|:------:|-----|-------------|
| 1 | **Facile** | Liste textuelle centrée : `Direction → Couleur` |
| 2 | **Normal** | Flèches colorées + noms de couleurs écrits le long des flèches + boussole (nord fixe) |
| 3 | **Moyen** | Flèches colorées, pas de noms, boussole **tournée aléatoirement** (sans points cardinaux) |
| 4 | **Difficile** | Flèches **noires**, boussole tournée (sans points cardinaux), couleurs dans un **encadré séparé** |
| 5 | **Cauchemar** | Flèches **noires**, **pas de boussole**, encadré couleurs avec **effet Stroop** (couleurs trompeuses) |

## 🏗️ Architecture

Architecture en 4 couches, principes SOLID et Clean Code :

```
src/
├── CoursesGn.Domain/            # Modèles, enums, interfaces
│   ├── Enums/                   # Direction, ArrowColor, ArrowStyle, OutputFormat, Difficulty, StartPosition
│   ├── Models/                  # Point2D, Arrow, NavigationCourse, GenerationParameters
│   ├── Helpers/                 # DirectionHelper, GeometryHelper
│   └── Interfaces/              # ICourseGenerator, ICardRenderer, IFileExporter
│
├── CoursesGn.Application/       # Logique métier
│   └── Services/                # CourseGeneratorService, NavigationCardOrchestrator
│
├── CoursesGn.Infrastructure/    # Rendu & export
│   ├── Rendering/               # SkiaCardRenderer, ArrowDrawer, CompassRenderer,
│   │                            # FontProvider, SkiaColorMapper, RenderingConstants
│   ├── Export/                  # FileExporter
│   └── Resources/               # boussole.png, boussole_sans_points_cardinaux.png, Fonts/
│
└── CoursesGn.Console/           # Point d'entrée
    ├── Input/                   # UserInputHandler
    └── Program.cs
```

## 🚀 Démarrage rapide

### Prérequis

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Lancer l'application

```bash
dotnet run --project src/CoursesGn.Console
```

L'application vous guide interactivement :

```
Nombre de flèches par carte (3-50) [défaut: 8] :
Nombre de cartes à générer (1-100) :
Format de sortie (pdf / jpg) [défaut: jpg] :
Style des flèches (1: Clean, 2: Dessiné à la main, 3: Pointillé) [défaut: 2] :
Difficulté (1-5) [défaut: 2] :
```

> **Appuyez sur Entrée** pour accepter les valeurs par défaut (8 flèches, JPG, dessiné à la main, niveau 2).

### Sortie

Les cartes sont générées dans un dossier horodaté :

```
cartes_hand_drawn_normal_20260217_210146/
├── carte_nav_normal_001.jpg
├── carte_nav_normal_002.jpg
└── ...
```

## 🎨 Personnalisation

### Police custom

1. Placez votre fichier `.ttf` ou `.otf` dans `src/CoursesGn.Infrastructure/Resources/Fonts/`
2. Dans `Program.cs`, configurez :
   ```csharp
   FontProvider.CustomFontFileName = "VoxPopuli.ttf";
   ```
3. Commentez cette ligne pour revenir à la police par défaut (`Calibri`)

### Constantes de rendu

Toutes les constantes visuelles sont centralisées dans `RenderingConstants.cs` :

| Constante | Description | Défaut |
|-----------|-------------|--------|
| `ArrowStrokeWidth` | Épaisseur du trait des flèches | `6.0` |
| `CompassSize` | Taille de la boussole (px) | `260` |
| `StepNumberFontSize` | Taille des numéros d'étape | `14` |
| `ColorLabelFontSize` | Taille des noms de couleurs (niv. 2) | `20` |
| `ColorBoxFontSize` | Taille du texte dans l'encadré (niv. 4-5) | `26` |
| `TextLevelFontSize` | Taille du texte (niv. 1) | `50` |

## 🔧 Règles de génération

Les parcours respectent 4 règles pour garantir des tracés cohérents :

1. **Pas de demi-tour** — interdit de faire un 180° (ex : Nord puis Sud)
2. **Pas de ligne droite** — la direction change à chaque flèche
3. **Pas d'intersection** — les segments ne se croisent jamais
4. **Proximité minimale** — distance min. de 0.9 entre les points
5. **Limite de virages** — max. 2 virages consécutifs dans la même direction

## 📦 Dépendances

- [SkiaSharp 2.88.9](https://github.com/mono/SkiaSharp) — rendu 2D (JPG + PDF)

## 📄 Licence

Projet personnel.
