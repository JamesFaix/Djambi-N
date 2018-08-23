SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO


CREATE PROCEDURE [Play].[Update_CurrentGameState]
	@GameId INT,
	@CurrentGameStateJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE Games
	SET CurrentGameStateJson = @CurrentGameStateJson
	WHERE GameId = @GameId
END
GO
