SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[VUserViewableGames]
AS

-- Users can see games if:
-- 1. Game.IsPublic = 1
-- 2. The user is a player in that game (if you create a game, you automatically become the 1st player)
-- 3. The user has the ViewGames privilege, which allows viewing any game

WITH UserGamePrivileges AS (
	SELECT u.UserId,
		IIF(up.PrivilegeId IS NULL, 0, 1) AS HasViewGamesPrivilege
	FROM Users u
		LEFT JOIN UserPrivileges up
			ON u.UserId = up.UserId
	WHERE up.PrivilegeId IS NULL
		OR up.PrivilegeId = 4 --ViewGames
),

PublicOrPrivilegeGames AS (
	SELECT g.GameId, u.UserId
	FROM Games g
		CROSS JOIN UserGamePrivileges u
	WHERE g.IsPublic = 1
		OR u.HasViewGamesPrivilege = 1
        OR g.CreatedByUserId = u.UserId
)

SELECT GameId, UserId FROM PublicOrPrivilegeGames
UNION (
	SELECT GameId, UserId FROM VGameUsers
)

GO