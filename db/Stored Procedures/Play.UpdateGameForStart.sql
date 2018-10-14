CREATE PROCEDURE [Play].[UpdateGameForStart] 
	@GameId INT,
	@StartingConditionsJson NVARCHAR(MAX),
	@CurrentGameStateJson NVARCHAR(MAX),
	@CurrentTurnStateJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @GameId)
		THROW 50404, 'Game not found.', 1

	UPDATE Games
	SET GameStatusId = 2, --Started
		StartingConditionsJson = @StartingConditionsJson,
		CurrentGameStateJson = @CurrentGameStateJson,
		CurrentTurnStateJson = @CurrentTurnStateJson
	WHERE GameId = @GameId
END