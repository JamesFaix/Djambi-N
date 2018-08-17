SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Lobby].[Update_Game] 
	@GameId INT,
	@StatusId INT,
	@Description NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @GameId)
		THROW 50000, 'Game not found', 1

	UPDATE Games
	SET GameStatusId = @StatusId,
		Description = @Description
	WHERE GameId = @GameId
END
GO
