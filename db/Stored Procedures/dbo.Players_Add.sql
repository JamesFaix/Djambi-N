CREATE PROCEDURE [dbo].[Players_Add]
	@GameId INT,
	@UserId INT,
	@PlayerKindId TINYINT,
	@Name NVARCHAR(50),

	--Default values all assume a player is being added in lobby
	--Custom values must be set when adding a full neutral player during game start
	@PlayerStatusId TINYINT = 1, --Pending
	@ColorId TINYINT = NULL,
	@StartingRegion TINYINT = NULL,
	@StartingTurnNumber TINYINT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (SELECT COUNT(1) FROM Players WHERE GameId = @GameId)
	 = (SELECT RegionCount FROM Games WHERE GameId = @GameId)
		THROW 50400, 'Max player count reached.', 1


	IF @PlayerKindId = 1 --User
	BEGIN
		DECLARE @UserName NVARCHAR(50) = (SELECT [Name] FROM Users WHERE UserId = @UserId)
		IF @Name IS NOT NULL AND @Name <> @UserName
			THROW 50400, 'Cannot set player name if player kind is User.', 1
		SET @Name = @UserName
	END

	INSERT INTO Players (
		GameId,
		UserId,
		PlayerKindId,
		[Name],
		PlayerStatusId,
		ColorId,
		StartingRegion,
		StartingTurnNumber)
	VALUES(
		@GameId,
		@UserId,
		@PlayerKindId,
		@Name,
		@PlayerStatusId,
		@ColorId,
		@StartingRegion,
		@StartingTurnNumber)

	SELECT SCOPE_IDENTITY()
END