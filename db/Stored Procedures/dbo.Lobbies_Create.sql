CREATE PROCEDURE [dbo].[Lobbies_Create]
	@RegionCount INT,
	@Description NVARCHAR(100),
	@CreatedByUserId INT,
	@AllowGuestPlayers BIT,
	@IsPublic BIT
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRAN
		INSERT INTO Games (
			BoardRegionCount, 
			[Description], 
			GameStatusId, 
			CreatedOn,
			CreatedByUserId,
			AllowGuestPlayers,
			IsPublic)
		VALUES (
			@BoardRegionCount, 
			@Description, 
			1, 
			GETUTCDATE(),
			@CreatedByUserId,
			@AllowGuestPlayers,
			@IsPublic)

		DECLARE @LobbyId INT = SCOPE_IDENTITY()

		EXEC Lobbies_AddPlayer
			@LobbyId = @LobbyId,
			@UserId = @CreatedByUserId

		SELECT @LobbyId
	COMMIT
END