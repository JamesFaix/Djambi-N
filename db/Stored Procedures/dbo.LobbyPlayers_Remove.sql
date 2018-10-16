CREATE PROCEDURE [dbo].[LobbyPlayers_Remove] 
	@LobbyPlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM LobbyPlayerId WHERE LobbyPlayerId = @LobbyPlayerId)
		THROW 50404, 'Lobby player not found.', 1
                        
    DELETE FROM LobbyPlayers
    WHERE LobbyPlayerId = @LobbyPlayerId
	
END