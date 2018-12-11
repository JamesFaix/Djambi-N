CREATE PROCEDURE [dbo].[Players_Add]
	@GameId INT,
	@UserId INT,
	@PlayerKindId TINYINT,
	@Name NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF (SELECT COUNT(1) FROM Players WHERE GameId = @GameId)
	 = (SELECT RegionCount FROM Games WHERE GameId = @GameId)
		THROW 50400, 'Max player count reached.', 1

	IF @PlayerKindId = 1 --User
	BEGIN
		IF @Name IS NOT NULL
			THROW 50400, 'Cannot set player name if player kind is User.', 1

		SET @Name = (SELECT [Name] FROM Users WHERE UserId = @UserId)
	END

	INSERT INTO Players (
		GameId,
		UserId,
		PlayerKindId,
		[Name],
		IsAlive,
		ColorId,
		StartingRegion,
		StartingTurnNumber)
	VALUES(
		@GameId,
		@UserId,
		@PlayerKindId,
		@Name,
		NULL,
		NULL,
		NULL,
		NULL)

	SELECT SCOPE_IDENTITY()
END