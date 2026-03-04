# Active Hazard
First Person Horror Exploration Game Project

Currently a work in progress. Code can be found in Assets/Scripts

## Features
### - Minimilistic Exploration
The player moves through areas while collecting narrative elements. The game focusing on exploration and primarily avoiding the a single NPC as an obstacle. The player has no means to engage in combat. 
### - Dynamic NPC
The main antagonist NPC takes advantage of multiple behavior states and Unity's NavMeshes. The NPC uses a linked list of transforms in order to make consistent patrols traversing the list in either direction. Said nodes also track which the player has moved past most recently in order to allow the NPC to follow the player with some delay. Using collision and line of sight the NPC can "see" the player and transition into a chasing state. When losing line of sight the NPC will move to the last position it was able to see the player. Other behaviors include searching for, but not chasing, the player. Except for the chasing behavior the NPC escalates and de-escalates through it's behavior states sequentially. Background systems which track player actions and player/NPC interactions allow the NPC to behave more or less aggresively to ensure the NPC presents a challenge without becoming an oppressive obstacle.
### - Dynamic Music
Music is determined by game state and tracks are crossfaded into one another to prevent jarring or strange sounding transitions. States which determine music including ones based on NPC behavior as well as more scripted cutscene states. The music featured in the game are original compositions. 

## Planned Features
- Additional NPCs
- Completed 2nd and 3rd area
- Complete narrative

