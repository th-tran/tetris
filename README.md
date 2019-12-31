# Tetris

This is one of my current projects - a faithful recreation of the arcade game "Tetris"!

## Dev log

### 2019-21-29

All the initial set-up of the project. I created rudimentary classes to extend later and layed out the basic UI.

*Functional:*
- Organized the project
- Created the Board class that encapsulates all the logic associated with the grid
- Created the Shape class that stores data about each block and enables movements and rotation
- Created the Spawner class that stores data about all types of shapes and spawns a random shape on request
- Created the GameManager class that handles all the game logic and acts as a mediary between other classes

*Other:* 
- Added a non-functional UI (works for 9:16 and 16:9)
- Made the UI nicer (added a board border and background)

---

### 2019-21-30

I extended the previous classes to implement all the fun Tetris logic :)

*Functional:*
- Extended GameManager and Shape to drop newly spawned blocks over time
- Extended GameManager and Spawner to keep track of the active shape and spawn a new shape when the active shape lands
- Extended Board to register landed shapes and check if a shape is in a valid position (out of bounds, not in a landed shape, etc)
- Extended Shape to allow for player input
- Extended GameManager and Board to clear lines in the grid when a shape lands
- Added a lose condition (block crosses line of death)

*Other:*
- Created a game over splash screen that appears when the player loses
- Added a functional restart button on the game over screen so the player can try again
