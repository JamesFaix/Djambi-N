CREATE PROCEDURE [dbo].[Games_Search]
	@CurrentUserId INT,
	@DescriptionContains NVARCHAR(100),
	@CreatedByUserName NVARCHAR(50),
	@PlayerUserName NVARCHAR(50),
	@ContainsMe BIT,
	@IsPublic BIT,
	@AllowGuests BIT,
	@GameStatusIds Int32List READONLY,
	@CreatedBefore DATETIME2,
	@CreatedAfter DATETIME2,
	@LastEventBefore DATETIME2,
	@LastEventAfter DATETIME2
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @AnyStatusFilter BIT = IIF(EXISTS(SELECT 1 FROM @GameStatusIds), 1, 0)

	SELECT g.GameId,
		g.GameStatusId,
		g.[Description],
		g.RegionCount,
		g.CreatedByUserId,
		u.Name as CreatedByUserName,
		g.CreatedOn,
		g.AllowGuests,
		g.IsPublic,
		ISNULL(e.CreatedOn, g.CreatedOn) as LastEventOn,
		gpc.PlayerCount,
		IIF(gu.UserId IS NOT NULL, 1, 0) as ContainsMe
    FROM Games g
		INNER JOIN Users u
			ON g.CreatedByUserId = u.UserId
		LEFT JOIN VLatestEvents e
			ON g.GameId = e.GameId
		INNER JOIN VGamePlayerCounts gpc
			ON g.GameId = gpc.GameId
		INNER JOIN VUserViewableGames uvg
			ON g.GameId = uvg.GameId
		LEFT JOIN @GameStatusIds gsids
			ON gsids.N = g.GameStatusId
		LEFT JOIN VGameUsers gu
			ON g.GameId = gu.GameId
			AND gu.UserId = @CurrentUserId

	WHERE uvg.UserId = @CurrentUserId
		AND (@DescriptionContains IS NULL OR g.[Description] LIKE '%' + @DescriptionContains + '%')
		AND (@CreatedByUserName IS NULL OR u.Name LIKE '%' + @CreatedByUserName + '%')
		AND (@PlayerUserName IS NULL
			OR EXISTS(
				SELECT 1
				FROM Players p
					INNER JOIN Users u
						ON p.UserId = u.UserId
				WHERE p.GameId = g.GameId
					AND u.Name LIKE '%' + @PlayerUserName + '%'
			)
		)
		AND (@ContainsMe IS NULL OR @ContainsMe = IIF(gu.UserId IS NOT NULL, 1, 0))
		AND (@IsPublic IS NULL OR @IsPublic = g.IsPublic)
		AND (@AllowGuests IS NULL OR @AllowGuests = g.AllowGuests)
		AND (@AnyStatusFilter = 0 OR gsids.N IS NOT NULL)
		AND (@CreatedBefore IS NULL OR @CreatedBefore > g.CreatedOn)
		AND (@CreatedAfter IS NULL OR @CreatedAfter < g.CreatedOn)
		AND (@LastEventBefore IS NULL OR @LastEventBefore > e.CreatedOn)
		AND (@LastEventAfter IS NULL OR @LastEventAfter < e.CreatedOn)
END