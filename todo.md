## Architecture
 - Use DI container for services
 - Be more consistent about Option vs Nullable
 - Review if Result needs to be used so much

## Enhancements
 - Implement more advanced stalemate checking. This could include checking if there is a wall of corpses separating each player and no Undertakers left.

## Bugs
 - Kill chief if player has 0 moves and abandon remaining pieces
 - Make error message returned by Controller more user friendly