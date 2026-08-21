# HW_2026_Test — Doofus Adventure Game
The Basic version of the game I implemented

https://github.com/user-attachments/assets/5c374bad-d714-45b1-8c44-7f0f632b9675

My version after changing the basic version



https://github.com/user-attachments/assets/435cba9c-2aec-4b33-bde0-0184285e511a



# Pulpit Hopper & The Ring of Power

A dynamic, grid-based survival platformer built with Unity and C#. Control Doofus across randomly generating floating pulpits, collect bonus rings, and survive on a collapsing path before time runs out.

---
## Scripts present and their functions:
* **`CameraFollow.cs`**
Smoothly follows the player character (Doofus) from a fixed offset distance using interpolation to keep the view centered on the action without jarring movements.
* **`DoofusController.cs`**
Handles player movement (WASD / Arrow Keys), physics, and rotation. It controls the character's Animator (`Idle` $\leftrightarrow$ `Run` transitions) and detects when the player falls off a platform to trigger Game Over.
* **`GameConfig.cs`**
Reads and deserializes the external `Doofus Diary` JSON file. It broadcasts game balance parameters (player speed, pulpit spawn time, min/max destruction timers) to other systems using events.
* **`GameManager.cs`**
The core state manager that tracks the game loop (`Playing`, `GameOver`), updates live score and high scores (`PlayerPrefs`), controls UI panels, and handles restart/scene routing.
* **`MainMenuController.cs`**
Manages the Start Menu UI interactions, specifically handling scene loading when pressing **Play** and exiting the application when clicking **Quit**.
* **`Pulpit.cs`**
Attached to each individual platform. It manages the pulpit's self-destruction countdown timer, signals the manager when it's time to spawn the next platform, and detects when Doofus steps on it.
* **`PulpitManager.cs`**
Coordinates platform generation on the grid in random adjacent directions ($+X, -X, +Z, -Z$). It enforces the 2-platform maximum rule, cleans up expired pulpits, and handles bonus collectible spawning.
* **`SimpleCollectibleScript.cs`**
Controls the bonus star/ring collectible. It handles continuous rotation, trigger collision with the player, spawning sound/particle VFX, and awarding $+5$ bonus points to the score via `GameManager`.

## Gameplay & Features

* **Dynamic Pulpit Generation:** Floating platforms spawn randomly in adjacent positions (Forward, Back, Left, Right) while enforcing active platform limits and self-destruction timers.
* **Responsive Character Controller:** Physics-driven player movement with velocity clamping, directional rotation smoothing, and fall-detection triggers.
* **Animated Character Rig:** Integrated 3D model with smooth state transitions between Idle and Run animations based on real-time movement velocity.
* **Collectibles & Scoring:**
* **Platform Stepping:** Earn points by reaching the safe zone of new pulpits.
* **Bonus Rings/Stars:** Rare collectibles spawn on platforms to award $+5$ score boosts with sound and particle effects.
* **Single-Instance Enforcement:** Ensures only one collectible item exists in the world at a time.


* **Configuration-Driven Design:** Supports external data models for dynamic player speed, pulpit spawn intervals, and destroy countdowns.
* **Complete UI/UX Loop:** Main menu scene, pause/restart systems, live score tracking, high scores, and game over screens.
* **Persistent Audio:** Seamless loop playback of background music across scenes via a singleton audio manager.


## 🏆 Challenge Levels Implemented

### ✅ Level 1: Character Movement & JSON-Driven Platform Spawning

* **Dynamic Configuration (`Doofus Diary / JSON`):** Implemented `GameConfig.cs` to asynchronously/locally parse the JSON configuration parameters:
* `player_data.speed` $\rightarrow$ Controls Doofus's walking/sliding speed.
* `pulpit_data.pulpit_spawn_time` ($x$) $\rightarrow$ Interval after which the next pulpit spawns.
* `pulpit_data.min_pulpit_destroy_time` ($y$) & `pulpit_data.max_pulpit_destroy_time` ($z$) $\rightarrow$ Random destruction countdown range ($y$ to $z$ seconds).


* **Physics-Based Character Controller (`DoofusController.cs`):**
* Full WASD and Arrow-key input support.
* Linear velocity clamping with rotation constraints to ensure clean, responsive traversal without tipping over.
* Custom 3D rigged character integration with an Animator Controller (smooth Idle $\leftrightarrow$ Run state transitions and root motion isolation).


* **Adjacency & Platform Lifecycle Management (`PulpitManager.cs` & `Pulpit.cs`):**
* Spawns $9 \times 9$ platforms randomly along adjacent cardinal vectors (`Vector3.forward`, `back`, `left`, `right` $\times 9$).
* **Max 2 Pulpits Rule:** Enforces that no more than two platforms exist simultaneously in the world.
* Independent timer countdowns per pulpit that trigger destruction and prevent duplicate coordinate overlaps using a `HashSet<Vector3>` registry.



---

### ✅ Level 2: Robust Scoring System & Edge-Case Handling

* **Pulpit Stepping Evaluation:** Tracks stepped platforms via unique instances in `GameManager.cs` to guarantee score is incremented **only once per distinct pulpit**.
* **Inner-Boundary Trigger Detection:** Utilizes trigger volumes with 2D distance thresholding to award points only when Doofus genuinely lands and walks on the platform surface.
* **Fall Detection & Instant Freeze:** Monitors falling thresholds ($Y < -3$). Freezes velocity, locks physics, and halts platform spawn loops immediately upon death to prevent infinite falling or orphan coroutine execution.

---

### ✅ Level 3: Complete UI/UX State Loop

* **Start Screen (`StartScene`):**
* Custom themed main menu with **Start Game** and **Quit** functionality.
* Integrated controls and instructions overlay.


* **In-Game HUD & Game Over Screen (`MainScene`):**
* Real-time score display and persistent high-score tracking using `PlayerPrefs`.
* Game Over modal with final score readout and an instant **Restart / Return to Menu** flow.


* **Audio Architecture (`MusicManager.cs`):**
* Persistent background music loop utilizing a singleton `DontDestroyOnLoad` pattern to maintain uninterrupted audio across scene transitions.



---

### ✨ Additional Creative Enhancements ("Out of the Box")

* **Collectible Ring/Star Mechanic (`SimpleCollectibleScript.cs`):**
* Dynamically spawns a bonus collectible star on newly generated pulpits with a **single-active-instance guarantee** (only 1 star exists in the game at any time).
* Collecting a star grants a **$+5$ Score Bonus**, plays spatial audio, and instantiates an auto-destroying VFX particle burst.


* **Smooth Directional Rotation:** Slerp-based yaw rotation that smoothly pivots the character model toward the current direction of movement.

## Controls

| Key / Input | Action |
| --- | --- |
| **W** | Move Forward ($+Z$) |
| **S**| Move Backward ($-Z$) |
| **A**  | Move Left ($-X$) |
| **D** | Move Right ($+X$) |

---

## Project Structure

```text
Assets/
├── Animations/           # Animation clips & Animator Controllers (PigController)
├── Audio/                # Background music and collectible sound FX
├── Materials/            # Platform, character, and UI materials
├── Models/               # 3D character and environment FBX assets
├── Prefabs/              # Pulpit, Player (Doofus), Collectibles, UI panels
├── Scenes/
│   ├── StartScene.unity  # Main Menu (Scene Index: 0 or 1)
│   └── MainScene.unity   # Primary Gameplay (Scene Index: 0 or 1)
└── Scripts/
    ├── DoofusController.cs        # Movement, physics, animation, and fall detection
    ├── Pulpit.cs                  # Platform lifecycle, timers, and trigger evaluation
    ├── PulpitManager.cs           # Grid layout calculation, spawning, and cleanup
    ├── GameManager.cs             # State machine, score tracking, and UI routing
    ├── GameConfig.cs              # Dynamic data & balance configuration
    ├── SimpleCollectibleScript.cs # Collectible pickups, audio/VFX, and bonus scoring
    ├── MainMenuController.cs      # Menu navigation and application quit logic
    └── MusicManager.cs            # Persistent background audio singleton

```

---

## Setup & Installation

1. **Clone the Repository:**
2. **Open in Unity:**
* Recommended Unity Version: **Unity 2022.3 LTS** or **Unity 6**.
3. **Configure Build Settings:**
* Navigate to `File` $\rightarrow$ `Build Settings...`.
* Add the scenes in order:
* `Index 1`: `StartScene`
* `Index 0`: `MainScene`




4. **Run the Game:**
* Open `StartScene` in the editor and press **Play**.
