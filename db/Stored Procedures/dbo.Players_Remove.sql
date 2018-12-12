CREATE PROCEDURE [dbo].[Players_Remove]
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserID INT = (SELECT TOP 1 UserId FROM Players WHERE PlayerId = @PlayerId)
	DECLARE @PlayerKindId TINYINT = (SELECT TOP 1 PlayerKindId FROM Players WHERE PlayerId = @PlayerId)

	IF @PlayerKindId IS NULL
		THROW 50404, 'Player not found.', 1

	BEGIN TRAN
		--If the removed player is a user, remove that user and all guests
		IF @PlayerKindId = 1 --USER
			DELETE FROM Players WHERE UserId = @UserId

		--Otherwise just delete that player
		ELSE
			DELETE FROM Players WHERE PlayerId = @PlayerId
	COMMIT
END