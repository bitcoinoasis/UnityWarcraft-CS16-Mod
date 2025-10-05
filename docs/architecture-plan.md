# CS 1.6 Warcraft Unity Remake – Architecture Plan

## Vision
Recreate the classic CS 1.6 Warcraft mod experience inside a modern Unity project, starting with the Iceworld map and AI-driven bots for prototyping. The project must be modular, data-driven, and ready for incremental content drops (new races, abilities, weapons, and maps).

## Initial Scope (Milestone 0.1)
- **Environment**: Procedurally composed Iceworld-inspired layout built from modular prefabs.
- **Characters**: First-person controller supporting weapon handling, health, shields, and Warcraft XP/level progression.
- **Bots**: NavMesh-based agents with basic combat and ability usage for demo matches.
- **Game Loop**: Round-based deathmatch with XP rewards, respawn system, and match state management.
- **Content Authoring**: ScriptableObject-driven definitions for races, abilities, weapons, and spawn configurations.

## Core Subsystems
1. **Bootstrap & Composition Root**
   - `GameBootstrap` sets up dependency container, loads settings, initializes services.
   - Service locator or lightweight dependency container to decouple systems.

2. **Match Flow**
   - `MatchManager` handles round states (Warmup → Active → RoundEnd → Intermission).
   - `TeamManager` tracks team rosters, XP, and win conditions.

3. **Player & Bot Characters**
   - Shared `CharacterMotor` for movement.
   - `CharacterCombat` for weapon firing, damage application.
   - `AbilityController` for Warcraft-specific passives/actives.
   - `BotBrain` layered AI (perception → decision → action).

4. **Progression System**
   - `RaceDefinition` (ScriptableObject) describing abilities unlocked per level.
   - `XPService` tracks experience, handles level ups and ability unlock events.

5. **Input & Camera**
   - Modular `InputActionAsset` mapping for FPS controls.
   - `CameraRig` handling head bob, recoil, FOV changes.

6. **Environment & Navigation**
   - Modular prefab library (`Assets/Prefabs/Environment/Iceworld`).
   - Runtime geometry assembly via `IceworldMapBuilder`.
   - NavMesh surface baking + links for vertical movement.

7. **Combat & Weapons**
   - Weapon scriptable objects describing fire rate, damage, spread.
   - `WeaponController` attaches to character and instantiates projectiles.

8. **FX & Audio**
   - Placeholder audio/particle systems with clear extension hooks.

## Folder Layout (Unity `Assets/`)
```
Assets/
  Art/
    Materials/
    Textures/
    Models/
  Audio/
  Data/
    Races/
    Weapons/
    Bots/
  Prefabs/
    Characters/
    Environment/
    Props/
    Weapons/
  Scenes/
    Iceworld.unity
  Scripts/
    Core/
    Match/
    Characters/
    Bots/
    Abilities/
    Weapons/
    Environment/
  Settings/
    Input/
    Match/
  Tests/
    EditMode/
    PlayMode/
```

## Development Guidelines
- Favor composition over inheritance. Use interfaces and ScriptableObjects for configuration.
- Keep runtime data (`MonoBehaviour`) and immutable definitions (ScriptableObjects) separate.
- Ensure all MonoBehaviours guard against missing dependencies and produce clear warnings.
- Build editor tooling for repetitive tasks (e.g., race/ability editors) once core loop is stable.
- Maintain unit tests for deterministic logic (progression, damage calculations) and play mode tests for integration.

## Next Milestones
1. **Milestone 0.2**: Flesh out first race (Human) with 4 abilities, basic weapon set, and improved UI.
2. **Milestone 0.3**: Add multiplayer scaffolding (Netcode for GameObjects) and network-ready state replication.
3. **Milestone 0.4**: Visual polish pass (materials, lighting, audio cues) and performance profiling.
