CREATE PROCEDURE [dbo].[Players_Get]
	@LobbyId INT,
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@LobbyId IS NOT NULL)
    AND (NOT EXISTS (SELECT 1 FROM Lobbies WHERE LobbyId = @LobbyId))
		THROW 50404, 'Lobby not found.', 1

	SELECT LobbyId,
		PlayerId,
		UserId,
		PlayerTypeId,
		[Name]
    FROM Players
	WHERE (LobbyId = @LobbyId OR @LobbyId IS NULL)
		AND (PlayerId = @PlayerId OR @PlayerId IS NULL)
END