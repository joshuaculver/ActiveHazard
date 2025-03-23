# Active Hazard
#### Joshua Culver

### 7/8/2024
Moving towards a second alpha. Though, it might be more accurate to call it a demo of the first "area." I implemented a system for the slides that convey the narrative and have those for the first area. I also changed the title screen scene pretty significantly. Now there's just a light that pans around.
Before there was an actual instance of the hazard which I had to do some gnarly hacky nonsense to get to behave correctly. If I standardized the managers it'd probably be more doable. Turned them into a prefab or something. The last couple big things are just filling out the space more with clutter and furniture
as well as implementing a system to collect or pick up the slides. Some top level game state scripting will also be neseccary. Other than that it's mostly polish stuff or just making assets. I've been putting off switching the hazard from a raycast to a cone collider for detection for instance.
Also consdiering how to handle moving between areas for the player. Part of me wants to force some kind of chase sequence, but that might be more effort than nessecary. It'll probably end up being more of forcing the player to be in a particular place and making them run through the level with 
blocked off areas to turn it into a maze.


### 6/14/2024 
Since this is a private repo I might as well keep a little personal dev log for now. I've already been working on the project for several months and it's using an older prototype for the first person movement.
The "monster" of this project I've been referring to as "The Hazard." It has basic behavior to wander around, chase the player, look for them more speficially. Works basically wherever a navmesh is setup there just needs to be transforms given as "nodes" to wander between.
Enviroments are still a work in progress. Most of the layout and a majority of the furnishing/objects probably won't change, but who knows. Still have some rooms to put stuff in and a general clutter pass to do.
Right now my main goals are to polish some aspects that are a little rough still and get a game progression mechanic implemented. Opening/closing doors, getting narrative conveyed, giving the player a purpose, etc.
Probably also going to gut some quick and dirty solutions I implemented to release a little playable alpha/teaser kind of deal.    

