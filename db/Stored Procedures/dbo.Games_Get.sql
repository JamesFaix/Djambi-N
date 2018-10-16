CREATE PROCEDURE [dbo].[Games_Get]
	@GameId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT g.GameId,
		l.RegionCount,
		g.GameStateJson,
		g.TurnStateJson
	FROM Games g
		INNER JOIN Lobbies l
			ON g.LobbyId = l.LobbyId
	WHERE g.GameId = @GameId
END