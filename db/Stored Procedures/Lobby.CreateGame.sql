CREATE PROCEDURE [Lobby].[CreateGame]
	@BoardRegionCount INT,
	@Description NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Games (
		BoardRegionCount, 
		[Description], 
		GameStatusId, 
		CreatedOn)
	VALUES (
		@BoardRegionCount, 
		@Description, 
		1, 
		GETUTCDATE())

    SELECT SCOPE_IDENTITY()
END