CREATE PROCEDURE [Lobby].[CreateGame]
	@BoardRegionCount INT,
	@Description NVARCHAR(100),
	@GameId INT OUTPUT
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

    SET @GameId = SCOPE_IDENTITY()
END