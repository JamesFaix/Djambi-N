CREATE PROCEDURE [Lobby].[CreateGame]
	@BoardRegionCount INT,
	@Description NVARCHAR(100),
	@CreatedByUserId INT
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRAN
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

		DECLARE @GameId INT = SCOPE_IDENTITY()

		EXEC Lobby.AddPlayerToGame 
			@GameId = @GameId,
			@UserId = @CreatedByUserId

		SELECT @GameId
	COMMIT
END