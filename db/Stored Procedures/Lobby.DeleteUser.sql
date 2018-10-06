CREATE PROCEDURE [Lobby].[DeleteUser]
	@UserId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Users WHERE UserId = @UserId)
		THROW 50000, 'User not found', 1
        
	BEGIN TRAN

		--Delete Players
		--Delete Session

		DELETE FROM Users
		WHERE UserId = @UserId

	COMMIT
END
GO
