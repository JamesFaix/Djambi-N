CREATE PROCEDURE [Play].[GetGame]
	@GameId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT GameId,
		BoardRegionCount,
		CurrentGameStateJson,
		CurrentTurnStateJson
	FROM Games 
	WHERE GameId = @GameId
END