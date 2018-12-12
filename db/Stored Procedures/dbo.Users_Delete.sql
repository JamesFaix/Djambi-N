CREATE PROCEDURE [dbo].[Users_Delete]
	@UserId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Users WHERE UserId = @UserId)
		THROW 50404, 'User not found.', 1

	BEGIN TRAN

		--TODO: Delete Players
		--TODO: Delete Session

		DELETE FROM Users
		WHERE UserId = @UserId

	COMMIT
END
GO
