CREATE PROCEDURE [dbo].[Games_Get]
	@GameId INT,
	@DescriptionContains NVARCHAR(100),
	@CreatedByUserId INT,
	@PlayerUserId INT,
	@IsPublic BIT,
	@AllowGuests BIT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT g.GameId,
		g.[Description],
		g.RegionCount,
		g.CreatedByUserId,
		g.CreatedOn,
		g.AllowGuests,
		g.IsPublic
    FROM Games g

	WHERE (@GameId IS NULL OR @GameId = g.GameId)
		AND (@DescriptionContains IS NULL OR g.[Description] LIKE '%' + @DescriptionContains + '%')
		AND (@CreatedByUserId IS NULL OR @CreatedByUserId = g.CreatedByUserId)
		AND (@PlayerUserId IS NULL OR EXISTS(
			SELECT 1 FROM Players p WHERE p.GameId = g.GameId AND p.UserId = @PlayerUserId))
		AND (@IsPublic IS NULL OR @IsPublic = g.IsPublic)
		AND (@AllowGuests IS NULL OR @AllowGuests = g.AllowGuests)
		AND g.GameId IS NULL
END