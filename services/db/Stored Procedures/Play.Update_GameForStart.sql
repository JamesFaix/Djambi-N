SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO


CREATE PROCEDURE [Play].[Update_GameForStart] 
	@GameId INT,
	@StartingConditionsJson NVARCHAR(MAX),
	@CurrentGameStateJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @GameId)
		THROW 50000, 'Game not found', 1

	UPDATE Games
	SET GameStatusId = 2, --Started
		StartingConditionsJson = @StartingConditionsJson,
		CurrentGameStateJson = @CurrentGameStateJson
	WHERE GameId = @GameId
END
GO
