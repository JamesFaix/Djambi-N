CREATE PROCEDURE [dbo].[Games_Create]
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
		INSERT INTO Games (
			GameStatusId,
			CreatedOn,
			CreatedByUserId,
			RegionCount,
			[Description],
			AllowGuests,
			IsPublic,
			TurnCycleJson,
			PiecesJson,
			CurrentTurnJson)
		VALUES (
			1, --Pending
			GETUTCDATE(),
			@CreatedByUserId,
			@RegionCount,
			@Description,
			@AllowGuests,
			@IsPublic,
			'',
			'',
			'')

		DECLARE @GameId INT = SCOPE_IDENTITY()

		INSERT INTO @Ignore -- This is required to prevent this from selection an additional result set
		EXEC Players_Add
			@GameId = @GameId,
			@UserId = @CreatedByUserId,
			@PlayerKindId = 1, --User (only users can create a lobby)
			@Name = NULL --Looked up from user's name

		SELECT @GameId
	COMMIT
END