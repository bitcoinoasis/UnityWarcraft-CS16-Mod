# Roadmap – Warcraft CS 1.6 Remake

## Milestone 0.1 – Prototype (Current)
- Procedural Iceworld layout with runtime NavMesh baking.
- Bot-vs-bot rounds using configurable loadouts.
- ScriptableObject-driven races, abilities, and weapons.
- XP service with unit test coverage.

## Milestone 0.2 – Core Gameplay
- Finalize Human and Orc race ability sets with active ability behaviours.
- Implement hitscan vs projectile weapon variants plus recoil patterns.
- Add HUD for health, shield, XP progress, and round timers.
- Expand tests: ability scaling, weapon damage math, match state transitions.

## Milestone 0.3 – Multiplayer Foundations
- Integrate Netcode for GameObjects (host / client authority split).
- Synchronize match phase, player spawns, and ability triggers over the network.
- Add lobby flow with team assignment and ready checks.
- Introduce persistence hooks for XP progression across sessions.

## Milestone 0.4 – Content & Polish
- Replace primitive geometry with authored modular environment assets.
- Add soundscape (footsteps, weapon SFX, ability cues) and particle FX.
- Implement announcer VO, scoreboard UI, and match summary screen.
- Performance pass (GPU instancing for props, navmesh optimization).

## Milestone 0.5 – Warcraft Expansion
- Additional races (Night Elf, Undead) with unique ability mechanics.
- Skill tree UI for mid-match ability selection and upgrades.
- Item shop system allowing XP spending on temporary boosts.
- Boss or event-based PvE rounds for cooperative variants.

## Engineering Wishlist
- Build automation (Unity Build Pipeline + CLI tests) integrated with CI.
- Roslyn analyzers or Rider inspections for code quality enforcement.
- Editor tooling for batch authoring (race/ability editors, bot wave generator).
- Telemetry abstraction for match analytics (team balance, ability usage).
