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

	SELECT LobbyId, 
		[Description],
		RegionCount,
		CreatedByUserId,
		CreatedOn,
		AllowGuests,
		IsPublic
		--TODO: Add player count
    FROM Lobbies
	WHERE (@LobbyId IS NULL OR @LobbyId = LobbyId)
		AND (@DescriptionContains IS NULL OR [Description] LIKE '%' + @DescriptionContains + '%')
		AND (@CreatedByUserId IS NULL OR @CreatedByUserId = CreatedByUserId)
		AND (@PlayerUserId IS NULL OR EXISTS(
			SELECT 1 FROM LobbyPlayers lp WHERE lp.LobbyId = LobbyId AND lp.UserId = @PlayerUserId))
		AND (@IsPublic IS NULL OR @IsPublic = IsPublic)
		AND (@AllowGuests IS NULL OR @AllowGuests = AllowGuests)
END