CREATE PROCEDURE [dbo].[Games_UpdateGameState]
	@GameId INT,
	@GameStateJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE Games
	SET GameStateJson = @GameStateJson
	WHERE GameId = @GameId
END