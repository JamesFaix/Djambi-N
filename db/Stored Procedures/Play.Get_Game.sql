SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO



CREATE PROCEDURE [Play].[Get_Game]
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
GO
