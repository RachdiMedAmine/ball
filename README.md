# 🎮 Roll-a-Ball Unity Game

A 3D Unity game where players control a rolling ball to collect coins while avoiding intelligent AI enemies across three progressively challenging levels.

## 🎯 Game Overview

Roll-a-Ball features physics-based ball movement, smart AI enemies, and dynamic coin spawning across three levels of increasing difficulty. Navigate the arena, collect coins, and survive enemy encounters to win!

### Gameplay Levels

**Level 1: Tutorial** 🟢
- Collect 4 coins to progress
- No enemies - learn the controls

**Level 2: The Hunt Begins** 🟡  
- Collect 10 total coins (6 more after Level 1)
- 1 AI enemy spawns and hunts you

**Level 3: Survival Mode** 🔴
- Survive for 60 seconds
- 2 AI enemies chase you simultaneously

## 🕹️ Controls

- `W` `A` `S` `D` or `Arrow Keys` - Move the ball
- Camera automatically follows the player

## ✨ Features

- 🤖 **Intelligent Enemy AI** - Enemies navigate obstacles while pursuing the player
- 📈 **Progressive Difficulty** - Three distinct levels with escalating challenge
- 🎨 **Modern Graphics** - Built with Universal Render Pipeline (URP)
- 🎯 **Dynamic Spawning** - Coins spawn at intervals throughout gameplay
- ⚡ **Physics-Based Movement** - Realistic ball rolling with Rigidbody physics
- 🔄 **Quick Restart** - Restart button appears on win/lose

## 🚀 Getting Started

### Prerequisites
- Unity 2020.3 or later
- Universal Render Pipeline (URP) package
- TextMeshPro (optional - supports legacy UI Text)

### Installation

1. Clone this repository
```bash
   git clone https://github.com/yourusername/roll-a-ball.git
```

2. Open the project in Unity Hub

3. Open `Assets/Roll-a-ball.unity` scene

4. Press Play to start the game

5. **Configure GameManager** (if needed):
   - Select GameManager object in hierarchy
   - Assign Enemy Prefab, UI elements, and Restart Button in Inspector

## 📁 Project Structure
```
Assets/
├── Materials/          # Visual materials for objects
├── Prefabs/           # Reusable game objects
│   ├── Player.prefab
│   ├── Enemy.prefab
│   ├── Pick Up.prefab
│   └── GameManager.prefab
├── Scripts/           # C# game logic
│   ├── PlayerController.cs
│   ├── EnemyAI.cs
│   ├── GameManager.cs
│   ├── CameraController.cs
│   ├── PickUpSpawner.cs
│   └── Rotator.cs
└── URP/              # Render pipeline settings
```

## 🛠️ Technical Details

### Core Scripts

| Script | Purpose |
|--------|---------|
| **PlayerController.cs** | Player movement, input, collision detection, fall detection |
| **EnemyAI.cs** | AI pathfinding, obstacle avoidance, player pursuit |
| **GameManager.cs** | Game state, level progression, UI management, enemy spawning |
| **CameraController.cs** | Camera follows player with fixed offset |
| **PickUpSpawner.cs** | Spawns coins at timed intervals |
| **Rotator.cs** | Animates coin rotation |

### Key Systems

**Physics System**
- Rigidbody-based movement for realistic physics
- Speed limiting and drag for responsive control
- Collision and trigger detection

**AI System**  
- Multi-directional raycasting for obstacle detection
- Intelligent pathfinding with avoidance vectors
- Dynamic pursuit behavior

**Spawn System**
- Randomized positions with player distance checks
- Prevents spawn overlap
- Timed intervals for progressive challenge

## 🎯 Win/Lose Conditions

### 🏆 Win
Complete Level 3 by surviving for 60 seconds

### ❌ Lose
- Enemy catches you (within 1.5 units)
- Fall off the map edge

## ⚙️ Configuration

Adjust these public variables in the Unity Inspector:

**EnemyAI.cs**
```csharp
moveSpeed = 8f              // Movement speed
maxSpeed = 12f              // Speed cap
catchDistance = 1.5f        // Catch radius
detectionDistance = 5f      // Obstacle detection range
```

**PlayerController.cs**
```csharp
speed = 10f                 // Movement force
maxSpeed = 15f              // Maximum speed
fallThreshold = -5f         // Fall detection Y position
```

**PickUpSpawner.cs**
```csharp
numberOfPickUps = 12        // Total coins
spawnInterval = 5f          // Seconds between spawns
spawnAreaWidth = 60f        // Arena width
spawnAreaDepth = 60f        // Arena depth
```

## 🤝 Contributing

Contributions are welcome! Feel free to:
- Report bugs
- Suggest features
- Submit pull requests

## 📝 Future Ideas

- Power-ups (speed boost, shield, slow-motion)
- Multiple arena designs
- Sound effects and music
- Leaderboard system
- Additional enemy types
- More levels
- Mobile controls

## 📄 License

This project is available under the MIT License.

## 🙏 Acknowledgments

Based on Unity's Roll-a-Ball tutorial, enhanced with:
- Multi-level progression system
- Intelligent AI enemies
- Dynamic spawning mechanics
- Modern URP rendering

---

⭐ Star this repo if you found it helpful!
