# Warcraft CS 1.6 Remake (Unity)

Modern re-imagining of the classic Counter-Strike 1.6 Warcraft mod, built in Unity 2022.3 LTS. The project starts with the Iceworld arena and bot matches for rapid iteration.

## Getting Started
1. Install **Unity 2022.3.12f1 LTS** (or later in the 2022.3 LTS line).
2. Clone this repository and open the `UnityWarcraft` folder with Unity Hub.
3. Let Unity import packages (Input System, NavMesh Components, Cinemachine, TextMesh Pro).
4. Open `Assets/Scenes/Iceworld.unity` once generated (see below) or create a new scene and add the `IceworldSceneBootstrap` prefab once available.

> ⚠️ Unity will create additional `ProjectSettings` and `Packages` metadata on first open. Commit the generated files to source control after verifying changes.

## Packages
- Input System (`com.unity.inputsystem`)
- AI Navigation (`com.unity.ai.navigation`)
- Cinemachine (`com.unity.cinemachine`)
- TextMesh Pro (`com.unity.textmeshpro`)
- Unity Test Framework (`com.unity.test-framework`)
- Timeline (`com.unity.timeline`)

## Folder Layout
- `Assets/Art` – Materials, meshes, textures.
- `Assets/Audio` – Sound effects, music, dialogue.
- `Assets/Data` – ScriptableObject definitions (races, weapons, bots, match configs).
- `Assets/Prefabs` – Prefabricated environment, props, weapons, characters.
- `Assets/Scenes` – Gameplay scenes (starting with Iceworld).
- `Assets/Scripts` – All gameplay code organized by system.
- `Assets/Settings` – Input actions, match settings, global configuration.
- `Assets/Tests` – EditMode and PlayMode test assemblies.

## Source Control Tips
- Enable **Visible Meta Files** and **Force Text** in Project Settings → Editor when opening Unity for the first time.
- Commit the entire `Assets`, `Packages`, and `ProjectSettings` directories.
- Ignore generated `Library/`, `Temp/`, `Logs/`, `Obj/`, and `Build/` folders.

## Next Steps
- Run the `IceworldMapBuilder` behavior (added later) to auto-generate the layout.
- Bake a NavMesh, assign spawn points, and configure bot teams via ScriptableObjects.
- Expand the race/ability set and implement the round-based match loop.
