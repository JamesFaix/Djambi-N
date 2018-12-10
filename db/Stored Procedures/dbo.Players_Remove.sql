CREATE PROCEDURE [dbo].[Players_Remove]
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserID INT = (SELECT TOP 1 UserId FROM Players WHERE PlayerId = @PlayerId)
	DECLARE @PlayerKindId TINYINT = (SELECT TOP 1 PlayerKindId FROM Players WHERE PlayerId = @PlayerId)
	DECLARE @GameId INT = (SELECT TOP 1 GameId FROM Players WHERE PlayerId = @PlayerId)
	DECLARE @GameCreatorUserId INT = (SELECT TOP 1 CreatedByUserId FROM Games WHERE GameId = @GameId)

	IF @PlayerKindId IS NULL
		THROW 50404, 'Player not found.', 1

	BEGIN TRAN
		--If the removed player is the user who created the game, close the game
		IF (@UserId IS NOT NULL)
		AND (@PlayerKindId = 1) --USER
		AND (@UserId = @GameCreatorUserId)
		BEGIN
			DELETE FROM Players WHERE GameId = @GameId
			DELETE FROM Games WHERE GameId = @GameId
		END

		ELSE
		BEGIN
			--If the removed player is a user, remove that user and all guests
		    IF @PlayerKindId = 1 --USER
				DELETE FROM Players WHERE UserId = @UserId

            --Otherwise just delete that player
			ELSE
				DELETE FROM Players WHERE PlayerId = @PlayerId
		END
	COMMIT
END