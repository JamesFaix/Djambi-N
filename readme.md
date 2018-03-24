# Djambi
This is an implementation of the chess-inspired board game, [Djambi][1], by Jean Anesto.

__Note:__ This readme is currently incomplete.  Please see the link to the Wikipedia page above for complete rules.

## Rules

### The board
- The game is played on a 9x9 board with alternating black and white squares, like a chess board.
- The center square, called _The Seat_ is a different color and has some special properties. (See **The Seat** section below.)

### The players
- The game is designed for 4 players, but can be played with 2 or 3 players (See **Less than 4 players** section below).
- Alliances, or any form of diplomacy or treachery, is allowed.
- The objective is to be the last player alive, so all alliances must be broken eventually.

### The pieces
- Each player has 9 pieces: 1 _Chief_, 1 _Assassin_, 1 _Journalist_, 1 _Diplomat_, 1 _Undertaker_, and 4 _Thugs_. 
- All pieces can move in all 8 directions (horizontal, vertical, and diagonal).
- No pieces can "jump" over other pieces, like a _Knight_ in Chess.
- Most pieces can move any number of cells in a single direction, except the _Thug_.
- You cannot use one piece to kill another piece you control.

#### <img src="https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/corpse.png" width="30"> Corpse
- When a piece is killed, it becomes a _Corpse_. (Unlike Chess, where pieces are removed from the board.)
- Players cannot control _Corpses_; they act as obstacles.

#### <img src="https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/thug.png" width="30"> Thug
- Can only move up to 2 cells.
- Plays a similar role to a _Pawn_ in Chess; numerous and expendable.
- Can move to any cell except _The Seat_.
- If the destination cell contains an enemy piece, the target piece is killed and the _Thug_'s controller places the _Corpse_ in any empty cell on the board, except _The Seat_.
	
#### <img src="https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/chief.png" width="30"> Chief
- Plays a similar role to a _King_ in Chess; if you lose it, you lose. Unlike a _King_ in Chess, there is nothing like _being in check_. Players are free to put their _Chief_ in dangerous situations.
- Can move to any cell.
- If the destination cell contains an enemy piece, the target piece is killed and the _Chief_'s controller places the _Corpse_ in any empty cell on the board, except _The Seat_.
- If a player kills another player's _Chief_, the killing player takes control of any other remaining pieces controlled by the dead _Chief_'s controller.

#### <img src="https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/assassin.png" width="30"> Assassin
- Can move to any cell except _The Seat_, unless an enemy _Chief_ is there. 
- If the destination cell contains an enemy piece, the target piece is killed and its _Corpse_ is moved to the cell the _Assassin_ started its turn in.
- If an enemy _Chief_ is in _The Seat_, the _Assassin_ may move there to kill it, and then its controller gets another move to move the _Assassin_ out of _The Seat_. This secondary move must be to an empty cell; no double kills.

#### <img src="https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/journalist.png" width="30"> Journalist
- Can move to any **empty** cell except _The Seat_.
- After the _Journalist_ moves, if any enemy pieces are horizontally or vertically adjacent to its destination cell, the _Journalist_'s controller kills one of the adjacent enemies. Diagonally adjacent pieces cannot be killed. The targeted piece's _Corpse_ is left in place.
- A _Journalist_ may kill only one adjacent enemy. If there are any adjacent enemies, it's controller must kill one, they cannot opt out.
- If a _Journalist_ is moved by a _Diplomat_, they do not kill any enemies adjacent to where it is moved to.
- If a _Journalist_ kills a _Chief_ in _The Seat_, the _Chief_'s _Corpse_ stays in _The Seat_. This is the only way a piece other than a _Chief_ can stay in _The Seat_.

#### <img src="https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/diplomat.png" width="30"> Diplomat
- Can move to any cell except _The Seat_, unless an enemy _Chief_ is there.
- If the destination cell contains an enemy piece, the _Diplomat_'s controller places the target piece in any empty cell on the board, **without** killing the target piece. 
- The _Diplomat_ cannot move any pieces except _Chiefs_ to _The Seat_.
- If an enemy _Chief_ is in _The Seat_, the _Diplomat_ may move there to move it, and then its controller gets another move to move the _Diplomat_ out of _The Seat_. This secondary move must be to an empty cell.

#### <img src="https://github.com/JamesFaix/Djambi3/blob/master/client/wpf/Djambi3.UI/Images/undertaker.png" width="30"> Undertaker
- Can move to any cell except _The Seat_, unless there is a _Corpse_ there.
- If the destination cell contains a _Corpse_, the _Undertaker_'s controller places the _Corpse_ in any empty cell on the board, except _The Seat_.
- If the _Undertaker_ moves to _The Seat_ to move a _Corpse_ out, its controller gets another move to move the _Undertaker_ out of _The Seat_. This secondary move must be to an empty cell.

### The Seat
- With some minor exceptions, only a _Chief_ may move to the _The Seat_.
- An _Assassin_ or _Diplomat_ may move there to get a _Chief_ out, but then they must make a secondary move to vacate _The Seat_. 
- If a _Journalist_ kills a _Chief_ in _The Seat_, the _Chief_'s _Corpse_ is left there.
- An _Undertaker_ can move to _The Seat_ to remove the _Corpse_ left by a _Journalist_, but must make a secondary move to vacate.
- These secondary moves must be to an empty cell. (So, for example, an _Assassin_ cannot use this to make two kills in one turn.)
- Any piece may move through _The Seat_ (if it is empty) to get to another cell.

### Surrounding
- If a _Chief_ that is not in _The Seat_ is surrounded by _Corpses_, it is killed and its _Corpse_ stays in the same cell.
- Any other pieces controlled by the same player become _Neutral Pieces_.

### Neutral Pieces
- _Neutral Pieces_ are pieces on the board that are alive, but not controlled by any player. 
- They can be killed or moved by a _Diplomat_ like any other living piece.

### Rising to Power
- When a _Chief_ moves to _The Seat_ if there are more than two players left, its controller _Rises to Power_. 
- A player in power gets to take a turn between each of the other players' turns. (If there are only two players left, this would not do anything.)
- When a _Chief_ leaves _The Seat_ (alive), its controller _Falls from Power_, and any extra pending turns its controller had are removed from the turn list, except the last one.
- When a player _Rises to Power_, they take control of any _Neutral_ pieces left over from _Surrounded Chiefs_, and keep control of them even if they _Fall from Power_.
- **Tip:** When another player rises to power, it is a good time to form temporary alliances against them.

### Less than 4 players
- When playing with only 2 or 3 players, _Neutral Players_ are created to fill the remaining player slots.
- _Neutral Players_ do not take turns.
- If a player kills a _Neutral Player_'s _Chief_, they gain control of any other pieces controlled by that _Neutral Player_.
- When a player _Rises to Power_, they do **not** gain control of any pieces controlled by a _Neutral Player_.

### Stalemate
- It is possible to end up with multiple _Chiefs_ still alive, but separated by walls of _Corpses_, with no _Undertakers_ still alive to move them out of the way. In this case, there is no winner.

 [1]: https://en.wikipedia.org/wiki/Djambi
