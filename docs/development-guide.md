# Development Guide – Warcraft CS 1.6 Remake

This guide walks through bootstrapping the Iceworld prototype scene, wiring up runtime systems, and establishing workflows that keep the project maintainable as it grows.

## 1. Scene Composition
1. **Create the scene**: In Unity, create `Assets/Scenes/Iceworld.unity` and open it.
2. **Add the Map Builder**:
   - Create an empty GameObject named `EnvironmentRoot` and add `IceworldMapBuilder`.
   - Optionally assign materials for floor, wall, and cover.
   - Enable *Build On Awake* or trigger the `Build Iceworld` context menu.
3. **Add NavMesh infrastructure**:
   - On `EnvironmentRoot`, add `NavMeshSurface` (from the AI Navigation package) and set the agent type to *Humanoid*.
   - Attach `NavMeshRuntimeBaker` and leave *Build On Start* enabled so the navmesh generates at runtime.
4. **Set up Game Bootstrap**:
   - Create a new GameObject `GameBootstrap` and add the `GameBootstrap` component.
   - Assign `MatchSettings`, `RaceCatalog`, `WeaponCatalog`, and `BotLoadoutSet` ScriptableObjects (create placeholder assets under `Assets/Data/` if needed).
   - Reference the `MatchManager` component (see below).
5. **Assemble Match Flow**:
   - Create `MatchManager` GameObject and add the `MatchManager` component.
   - Create `BotManager` GameObject, add the component, and assign a bot prefab plus spawn points (see *Bots* section).
   - Drag the `BotManager` into the `MatchManager` field.
6. **Player Controller (optional)**:
   - Add a `Player` prefab or GameObject with `CharacterMotor`, `CharacterCombat`, `CharacterHealth`, `AbilityController`, and a `PlayerInput` component using `Assets/Settings/Input/PlayerInput.inputactions`.
   - Attach `PlayerInputHandler` and hook the component references.

## 2. Data Authoring
- **Match Settings**: Create `MatchSettings` asset under `Assets/Data/Match` to tweak round timers and bot counts.
- **Races**: Author `RaceDefinition` assets under `Assets/Data/Races/`. Use the inspector to assign abilities per level.
- **Abilities**: Create new `AbilityDefinition` assets for active/passive skills. Extend via custom MonoBehaviours when implementing ability effects.
- **Weapons**: Populate `WeaponDefinition` assets with damage curves and assign weapon prefabs under `Assets/Prefabs/Weapons`.
- **Bots**: Build `BotProfile` assets under `Assets/Data/Bots`, setting race and weapons for each archetype. Group them in a `BotLoadoutSet`.

## 3. Bot Prefab Setup
1. Create `Prefabs/Characters/Bot.prefab` with the following components:
   - `CharacterController`
   - `NavMeshAgent` (agent type Humanoid, acceleration 30, speed 5.5)
   - `CharacterMotor`, `CharacterCombat`, `CharacterHealth`, `AbilityController`
   - `BotBrain`
2. Assign a `weaponSocket` transform (e.g., the right-hand bone) on `CharacterCombat`.
3. Ensure the prefab includes colliders for raycast detection and reference materials/models for quick identification.

## 4. Testing Workflow
- **Edit Mode Tests**: Place deterministic logic tests (e.g., XP curves, ability math) inside `Assets/Tests/EditMode/`. Run them via Unity Test Runner in Edit Mode.
- **Play Mode Smoke Tests**: Add automated scene loading tests later under `Assets/Tests/PlayMode/` to validate map generation and bot spawning.
- **Continuous Integration**: When the project is linked to CI, use `-runTests` Unity CLI flags to execute both assemblies. Cache the `Library/` folder to speed up imports.

## 5. Coding Standards
- Use namespaces matching folder structure (e.g., `Warcraft.Match`, `Warcraft.Bots`).
- Prefer composition with plain C# services registered through `ServiceRegistry`.
- For ScriptableObjects, expose read-only properties so runtime systems cannot mutate authoring data.
- Gate editor/test-only helpers behind `UNITY_EDITOR` or `UNITY_INCLUDE_TESTS`.
- Keep MonoBehaviours lightweight; move heavy logic to plain C# classes or ScriptableObjects when possible.

## 6. Extending the Prototype
- **Warcraft Races**: Implement active ability behaviors by creating new MonoBehaviours that subscribe to `AbilityController` events.
- **Weapon Variety**: Support projectiles by extending `WeaponController` with spawnable projectile prefabs and physics interactions.
- **Match UI**: Create HUD scripts under `Assets/Scripts/UI` to display timers, team scores, and ability cooldowns.
- **Networking**: Integrate Netcode for GameObjects once the local loop is stable; mirror the service registration pattern for network-specific systems.

## 7. Source Control & Collaboration
- Track all assets under `Assets/`, `Packages/`, and `ProjectSettings/`.
- Enable *Enter Play Mode Options* (disable domain reload) later to speed iteration, but ensure scripts support it.
- Establish pull request checks that run Edit Mode tests and optionally static analysis with `com.unity.ide.rider` or Roslyn analyzers.

Following this flow ensures anyone can spin up the scene, run bot matches, and build on top of the Warcraft systems with confidence.
