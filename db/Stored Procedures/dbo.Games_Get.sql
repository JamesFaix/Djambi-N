CREATE PROCEDURE [Play].[GetGame]
	@GameId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT g.GameId,
		l.RegionCount,
		g.CurrentGameStateJson,
		g.CurrentTurnStateJson
	FROM Games g
		INNER JOIN Lobbies l
			ON g.LobbyId = l.LobbyId
	WHERE g.GameId = @GameId
END