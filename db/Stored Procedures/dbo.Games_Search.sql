CREATE PROCEDURE [dbo].[Games_Search]
	@DescriptionContains NVARCHAR(100),
	@CreatedByUserName NVARCHAR(50),
	@PlayerUserName NVARCHAR(50),
	@IsPublic BIT,
	@AllowGuests BIT,
	@GameStatusId TINYINT
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

	WHERE (@DescriptionContains IS NULL OR g.[Description] LIKE '%' + @DescriptionContains + '%')
		AND (@CreatedByUserName IS NULL
			OR EXISTS(
				SELECT 1
				FROM Users u
				WHERE u.Name = @CreatedByUserName
					AND u.UserId = g.CreatedByUserId
			)
		)
		AND (@PlayerUserName IS NULL
			OR EXISTS(
				SELECT 1
				FROM Players p
					INNER JOIN Users u
						ON p.UserId = u.UserId
				WHERE p.GameId = g.GameId
					AND u.Name = @PlayerUserName
			)
		)
		AND (@IsPublic IS NULL OR @IsPublic = g.IsPublic)
		AND (@AllowGuests IS NULL OR @AllowGuests = g.AllowGuests)
		AND (@GameStatusId IS NULL OR @GameStatusId = g.GameStatusId)
END