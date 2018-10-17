CREATE PROCEDURE [dbo].[Lobbies_Get] 
	@LobbyId INT,
	@DescriptionContains NVARCHAR(100),
	@CreatedByUserId INT,
	@PlayerUserId INT,
	@IsPublic BIT,
	@AllowGuests BIT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT l.LobbyId, 
		l.[Description],
		l.RegionCount,
		l.CreatedByUserId,
		l.CreatedOn,
		l.AllowGuests,
		l.IsPublic
		--TODO: Add player count
    FROM Lobbies l
		LEFT OUTER JOIN Games g
			ON g.LobbyId = l.LobbyId
	WHERE (@LobbyId IS NULL OR @LobbyId = l.LobbyId)
		AND (@DescriptionContains IS NULL OR l.[Description] LIKE '%' + @DescriptionContains + '%')
		AND (@CreatedByUserId IS NULL OR @CreatedByUserId = l.CreatedByUserId)
		AND (@PlayerUserId IS NULL OR EXISTS(
			SELECT 1 FROM Players p WHERE p.LobbyId = l.LobbyId AND p.UserId = @PlayerUserId))
		AND (@IsPublic IS NULL OR @IsPublic = l.IsPublic)
		AND (@AllowGuests IS NULL OR @AllowGuests = l.AllowGuests)
		AND g.GameId IS NULL
END