SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Lobby].[Delete_Player] 
	@GameId INT,
	@UserId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Players WHERE GameId = @GameId AND UserId = @UserId)
		THROW 50000, 'Player not found', 1
                        
    IF (SELECT GameStatusId FROM Games WHERE GameId = @GameId) <> 1
	    THROW 50000, 'Game no longer open', 1

    DELETE FROM Players
    WHERE GameId = @GameId AND UserId = @UserId
	
END
GO
