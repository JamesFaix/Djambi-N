SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Play].[Update_GameState]
	@GameId INT,
	@CurrentStateJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE Games
	SET CurrentStateJson = @CurrentStateJson
	WHERE GameId = @GameId
END
GO
