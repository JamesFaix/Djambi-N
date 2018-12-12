CREATE PROCEDURE [dbo].[Games_Create]
	@RegionCount INT,
	@Description NVARCHAR(100),
	@CreatedByUserId INT,
	@AllowGuests BIT,
	@IsPublic BIT
AS
BEGIN
	SET NOCOUNT ON;

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

	SELECT SCOPE_IDENTITY()
END