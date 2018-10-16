CREATE PROCEDURE [dbo].[Games_Start] 
	@LobbyId INT,
	@StartingConditionsJson NVARCHAR(MAX),
	@GameStateJson NVARCHAR(MAX),
	@TurnStateJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Lobbies WHERE LobbyId = @LobbyId)
		THROW 50404, 'Lobby not found.', 1

	INSERT INTO Games (
		LobbyId,
		StartedOn,
		GameStatusId,
		StartingConditionsJson,
		GameStateJson,
		TurnStateJson)
	VALUES (
		@LobbyId,
		GETUTCDATE(),
		2, --Started
		@StartingConditionsJson,
		@GameStateJson,
		@TurnStateJson
	)
	
	SELECT SCOPE_IDENTITY()
END