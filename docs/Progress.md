# Progress

## Current Version
The current implementation is being developed as a web application to support online play. It will support real-time play (all players online at the same time, game lasting 20 minutes) and play by correspondence (each player makes a move once a day, game lasting as long as you want).

### Tech stack
- Web client written in TypeScript/HTML/CSS. Currently no rendering framework, but React will probably be added as the UI gets more sophisticated.
- RESTful API written in F# with the Giraffe framework. 
- SQL Server database (Will probably change to a free SQL vendor for production)
- No mobile clients yet, but Android is far ahead on the roadmap.

### Complete
- Core game logic in API
- Board rendering in web client
- Board/piece interactions in web client

### In progress
- User security

### Not started
- Making the UI look nice
- Push notification to clients for when another player moves
- Sounds
- Animations 
- Menus/lobby
- Player messaging

### Development setup
- .NET Core 2.1 SDK
- SQL Server

## Archive/V3
A working desktop version of Djambi, implemented in Windows Presentation Foundation can be found in `/_archive/v3/client/wpf/`. This version is stable and can be played with multiple players sitting around the same Windows computer. 

There are a few bugs that allow moves that are against the rules of Djambi. For example, it will allow you to use a _Diplomat_ move a non-_Chief_ piece to _The Seat_. I have played through many games and have not found anything that will cause it to crash.

There is also a `v3/client/unity` folder where I was starting to rewrite the UI in Unity3D with the same library for the rules, but that was abandoned.

[1]: https://en.wikipedia.org/wiki/Djambi
[2]: rules.md