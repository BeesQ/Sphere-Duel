# Sphere Duel Game by Jakub "BeesQ" Łotysz

![Unity](https://img.shields.io/badge/Unity-6000.3.10f1-black?logo=unity) ![NGO](https://img.shields.io/badge/Netcode_for_GameObjects-2.9.2-blue) ![License](https://img.shields.io/badge/License-MIT-green)

![Sphere Duel Gameplay](https://github.com/user-attachments/assets/04f1985b-9007-42e5-9c23-d8fcd259ac63)

## About

**Sphere Duel** is a 2D Online Multiplayer (Peer-To-Peer*) Top-Down Shooter, made as a Portfolio Project

> *\*In the Gaming Industry, "Peer-To-Peer" typically refers to a Host-Client (Listen Server) model — one player acts as both Server and Client. This differs from true P2P, where all peers are equal with no central authority.*

## Features

- Online multiplayer for 2 players (Host-Client)
- Server-authoritative scoring and hit detection
- Health, death, and respawn system with visual feedback
- Round-based matches with win/lose screen
- Play Again and Main Menu options after match ends
- Disconnect handling with automatic return to menu
- Dynamic arena bounds based on camera size

## Technologies

- **Unity** 6000.3.10f1
- **Netcode for GameObjects** 2.9.2
- **Unity Transport** 2.6.0
- **Input System** 1.18.0
- **Multiplayer Play Mode** 2.0.1
- **Multiplayer Tools** 2.2.8

## Architecture

- **Event-Driven Architecture** — decoupled communication between systems via static `GameEvents` class (e.g. `OnPlayerDied`, `OnScoreChanged`, `OnMatchWon`)
- **Server-Authoritative Design** — health, scoring, projectile spawning and hit detection are all validated on the server to prevent cheating
- **NetworkVariables** — real-time state synchronization for health and scores across all clients
- **RPCs** — client-server communication for gameplay actions
  - `ServerRpc` — client-to-server requests (e.g. shooting)
  - `ClientRpc` — server-to-client broadcasts (e.g. death, respawn, match results)
- **Singleton Pattern** — centralized access points for `GameManager`, `ScoreManager`, and `DisconnectHandler`
- **Centralized Constants** — all magic numbers stored in `Const.cs`, with `GameManager` exposing Inspector-configurable overrides
- **Separation of Concerns** — utility classes like `SpawnHelper` keep logic clean and reusable

## How to Play

| Action | Control |
|--------|---------|
| Move | `WASD` |
| Aim | Mouse |
| Shoot | Left Click |

First to **3 kills** wins the match.

## How to Run

1. Clone the repository
2. Open in **Unity 6000.3.10f1**
3. It is recommended to open `MenuScene` as the starting scene (an Editor script auto-redirects to `MenuScene` when entering Play Mode from other scenes)

**Testing Multiplayer:**

- **Multiplayer Play Mode (MPPM)** — enable via *Window > Multiplayer Play Mode*, add a Virtual Player, then enter Play Mode. One instance hosts, the other joins on `127.0.0.1` (localhost)
- **Two Builds** — build the project, run two instances. Host on one, join on the other using the host's local IP
- Default port: `7777`

## Project Structure (Key Folders)

```
Assets/
├── Editor/            ForceMenuSceneEditor
├── Prefabs/           Player, Projectile
│   └── UI/
├── Scenes/            GameScene, MenuScene
└── Scripts/
    ├── Core/          Const, GameEvents, GameManager, ScoreManager, SpawnHelper
    ├── Network/       ClientNetworkTransform, DisconnectHandler, NetworkManagerUI
    ├── Player/        PlayerController, PlayerHealth, PlayerShooting
    ├── Projectile/    Projectile
    └── UI/            HealthBarUI, WinScreenUI
```

## Roadmap

- **Unity Relay + WebGL** — integrate Unity Relay to enable browser-playable builds hosted on itch.io or GitHub Pages, removing the need for port forwarding

## Assets

- Graphics -> [Kenney Game Assets (All-in-1)](https://kenney.itch.io/kenney-game-assets)
