CREATE PROCEDURE [dbo].[Players_Get]
	@LobbyId INT,
	@GameId INT,
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF @PlayerId IS NOT NULL
	BEGIN
		SELECT LobbyId,
			PlayerId,
			UserId,
			PlayerTypeId,
			[Name]
		FROM Players
		WHERE PlayerId = @PlayerId
	END

	ELSE IF @LobbyId IS NOT NULL
	BEGIN
		IF NOT EXISTS (SELECT 1 FROM Lobbies WHERE LobbyId = @LobbyId)
			THROW 50404, 'Lobby not found.', 1

		SELECT LobbyId,
			PlayerId,
			UserId,
			PlayerTypeId,
			[Name]
		FROM Players
		WHERE LobbyId = @LobbyId
	END

	ELSE IF @GameId IS NOT NULL
	BEGIN
		IF NOT EXISTS (SELECT 1 FROM Games WHERE GameId = @GameId)
			THROW 50404, 'Game not found.', 1

		SELECT p.LobbyId,
			p.PlayerId,
			p.UserId,
			p.PlayerTypeId,
			p.[Name]
		FROM Players p
			INNER JOIN Games g
				ON p.LobbyId = g.LobbyId
		WHERE g.GameId = @GameId
	END
	ELSE
		THROW 50500, 'Invalid parameters for player query.', 1
END