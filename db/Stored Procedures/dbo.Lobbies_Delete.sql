CREATE PROCEDURE [dbo].[Lobbies_Delete]
	@LobbyId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Lobbies WHERE LobbyId = @LobbyId)
		THROW 50404, 'Lobby not found.', 1

	BEGIN TRAN
		DELETE FROM Players WHERE LobbyId = @LobbyId

		DELETE FROM Lobbies WHERE LobbyId = @LobbyId
	COMMIT
END