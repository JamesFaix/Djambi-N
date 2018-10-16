CREATE PROCEDURE [dbo].[Lobbies_Delete]
	@LobbyId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Lobbies WHERE LobbyId = @LobbyId)
		THROW 50404, 'Lobby not found.', 1

	DELETE FROM LobbyPlayers WHERE LobbyId = @LobbyId
END