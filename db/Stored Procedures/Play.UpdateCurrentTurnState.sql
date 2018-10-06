CREATE PROCEDURE [Play].[Update_CurrentTurnState]
	@GameId INT,
	@CurrentTurnStateJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE Games
	SET CurrentTurnStateJson = @CurrentTurnStateJson
	WHERE GameId = @GameId
END