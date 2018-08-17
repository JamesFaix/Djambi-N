SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Lobby].[Get_GameWithPlayers] 
	@GameId INT
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
    WHERE g.GameId = @GameId
					
END
GO
