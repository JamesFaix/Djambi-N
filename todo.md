## Bugs 

Confirm and Cancel buttons
 - Enable cancel button if any selections have been made
 - Disable cancel button if no selections have been made
 - Don't allow confirm until turn status is awaiting confirmation

Don't allow dropping corpse at move destination
 - When previewing a game state, since Subject and Target pieces are determined by location, they can be wrong
 - Maybe split Selection type into two types, one for a selection that the user has made and one for a potential selection.
   The potential selection may only need a location, so that cells can be highlighted
   The chosen selection may need a pieceId to prevent ambiguity over Subject and Target
		
## Debug aides

Logging
 - Implement log in engine
 - Write log to log display
	
Add labels to edge of board for row/column numbers

Add pieceId to pieces in UI

## Architecture

Use DI container for services

Be more consistent about Option vs Nullable

Review if Result needs to be used so much