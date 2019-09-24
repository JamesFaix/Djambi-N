CREATE PROCEDURE [dbo].[Games_Get]
	@GameId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT g.GameId,
		g.GameStatusId,
		g.[Description],
		g.RegionCount,
		g.CreatedByUserId,
		u.Name as CreatedByUserName,
		g.CreatedOn,
		g.AllowGuests,
		g.IsPublic,
		g.TurnCycleJson,
		g.PiecesJson,
		g.CurrentTurnJson
    FROM Games g
		INNER JOIN Users u
			ON g.CreatedByUserId = u.UserId
	WHERE @GameId = g.GameId
END