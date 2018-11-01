CREATE PROCEDURE [dbo].[Lobbies_Create]
	@RegionCount INT,
	@Description NVARCHAR(100),
	@CreatedByUserId INT,
	@AllowGuests BIT,
	@IsPublic BIT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Ignore TABLE (Id INT)

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

		INSERT INTO @Ignore -- This is required to prevent this from selection an additional result set
		EXEC Players_Add
			@LobbyId = @LobbyId,
			@UserId = @CreatedByUserId,
			@PlayerTypeId = 1, --User (only users can create a lobby)
			@Name = NULL --Looked up from user's name

		SELECT @LobbyId
	COMMIT
END