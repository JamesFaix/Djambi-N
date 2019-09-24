SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[VGamePlayerCounts]
AS

WITH PlayerCounts AS (
	SELECT GameID, COUNT(PlayerID) AS PlayerCount
	FROM Players
	GROUP BY GameID
)

SELECT g.GameId,
	ISNULL(pc.PlayerCount, 0) AS PlayerCount
FROM Games g
	LEFT JOIN PlayerCounts pc
		ON g.GameId = pc.GameId
GO