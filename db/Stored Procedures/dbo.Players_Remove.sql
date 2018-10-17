CREATE PROCEDURE [dbo].[Players_Remove] 
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserID INT = (SELECT TOP 1 UserId FROM Players WHERE PlayerId = @PlayerId)
	DECLARE @PlayerTypeId TINYINT = (SELECT TOP 1 PlayerTypeId FROM Players WHERE PlayerId = @PlayerId)

	IF @PlayerTypeId IS NULL
		THROW 50404, 'Player not found.', 1

	BEGIN TRAN
		DELETE FROM Players
		WHERE PlayerId = @PlayerId

		--Delete guests of user
		IF @PlayerTypeId = 1 -- User
			DELETE FROM Players 
			WHERE UserId = @UserId
	COMMIT		
END