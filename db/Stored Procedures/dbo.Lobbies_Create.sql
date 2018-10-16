CREATE PROCEDURE [dbo].[Lobbies_Create]
	@RegionCount INT,
	@Description NVARCHAR(100),
	@CreatedByUserId INT,
	@AllowGuests BIT,
	@IsPublic BIT
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRAN
		INSERT INTO Lobbies (
			RegionCount, 
			[Description], 
		--	GameStatusId, 
			CreatedOn,
			CreatedByUserId,
			AllowGuests,
			IsPublic)
		VALUES (
			@RegionCount, 
			@Description, 
	--		1, 
			GETUTCDATE(),
			@CreatedByUserId,
			@AllowGuests,
			@IsPublic)

		DECLARE @LobbyId INT = SCOPE_IDENTITY()

		EXEC Lobbies_AddPlayer
			@LobbyId = @LobbyId,
			@UserId = @CreatedByUserId

		SELECT @LobbyId
	COMMIT
END