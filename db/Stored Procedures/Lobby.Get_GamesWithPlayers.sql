SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO




CREATE PROCEDURE [Lobby].[Get_GamesWithPlayers] 
	@GameId INT,
	@UserId INT,
	@StatusId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT g.GameId, 
		g.GameStatusId,
		g.[Description] AS GameDescription, 
		g.BoardRegionCount,
		p.PlayerId, 
		p.UserId, 
		p.[Name] as PlayerName
    FROM Games g
        LEFT OUTER JOIN Players p ON g.GameId = p.GameId
	WHERE @GameId IS NULL OR @GameId = g.GameId
		AND @StatusId IS NULL OR @StatusId = g.GameStatusId
		AND @UserId IS NULL OR EXISTS(SELECT 1 FROM Players WHERE GameId = g.GameId AND UserId = @UserId)
END
GO
