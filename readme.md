# Djambi
This is an implementation of the chess-inspired board game, [Djambi][1], by Jean Anesto.

__Note:__ This readme is currently incomplete.  Please see the link to the Wikipedia page above for complete rules.

## Rules

### The board
- The game is played on a 9x9 board with alternating black and white squares, like a chess board.
- The center square, called _The Seat_ is a different color and has some special properties.

### The players
- The game is designed for 4 players, but can be played with 2 or 3 players with some special considerations.
- Alliances or any form of diplomacy is allowed.
- The objective is to be the last player alive, so all alliances must be broken eventually.

### The pieces
- Each player has 9 pieces: 1 _Chief_, 1 _Assassin_, 1 _Journalist_, 1 _Diplomat_, 1 _Undertaker_, and 4 _Thugs_. 
- All pieces can move in all 8 directions (horizontal, vertical, and diagonal).

#### <img src="https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/thug.png" width="48"> Thug
- Can move up to 2 cells
- Destination can be any cell except the Seat or a cell occupied by an ally piece
- If destination contains enemy piece, it is killed
- Must place Corpse in any unoccupied cell, except the Seat
	
#### ![alt text](https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/chief.png) Chief
- Can move up to 8 cells
- Destination can be any cell not occupied by an ally piece
- If destination contains enemy piece, it is killed
- Must place Corpse in any unoccupied cell, except the Seat

#### ![alt text](https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/assassin.png) Assassin
- Can move up to 8 cells
- Destination can be any cell except the Seat or a cell occupied by an ally piece
- Destination can be the Seat if an enemy Chief is there, if so the Assassin must move to another unoccupied cell afterward
- If destination contains enemy piece, it is killed
- Corpse is placed in the cell the Assassin started in

#### ![alt text](https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/journalist.png) Journalist
- Can move up to 8 cells
- Destination can be any unoccupied cell or the Seat
- Can target a piece horizontally or vertically adjacent to destination
- Corpse stays in its cell (this is the only way to have a corpse in the Seat)

#### ![alt text](https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/diplomat.png) Diplomat
- Can move up to 8 cells
- Destination can be any cell except the Seat or a cell occupied by an ally piece
- Destination can be the Seat if an enemy Chief is there, if so the Diplomat must move to another unoccupied cell afterward
- Does not kill target, but moves to any unoccupied cell
- Cannot move a non-Chief piece to the Seat

#### ![alt text](https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/undertaker.png) Undertaker
- Can move up to 8 cells
- Destination can be any unoccupied cell except the Seat or any cell occupied by a Corpse
- Destination can be the Seat if a Corpse is there, if so the Undertaker must move to another unoccupied cell afterward	
- Move Corpse to any unoccupied cell, except the Seat

#### ![alt text](https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/corpse.png) Corpse
- When a piece is killed, it becomes a Corpse. (Unlike Chess, where pieces are removed from the board.)
- Players cannot control Corpses, although an Undertaker can be used to move them and create barricades.

 [1]: https://en.wikipedia.org/wiki/Djambi
