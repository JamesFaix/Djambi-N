CREATE PROCEDURE [Lobby].[CreateGame]
	@BoardRegionCount INT,
	@Description NVARCHAR(100),
	@CreatedByUserId INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Games (
		BoardRegionCount, 
		[Description], 
		GameStatusId, 
		CreatedOn,
		CreatedByUserid)
	VALUES (
		@BoardRegionCount, 
		@Description, 
		1, 
		GETUTCDATE(),
		@CreatedByUserId)

    SELECT SCOPE_IDENTITY()
END