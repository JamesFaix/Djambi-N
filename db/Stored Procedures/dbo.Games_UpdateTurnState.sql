CREATE PROCEDURE [dbo].[Games_UpdateTurnState]
	@GameId INT,
	@TurnStateJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE Games
	SET TurnStateJson = @TurnStateJson
	WHERE GameId = @GameId
END