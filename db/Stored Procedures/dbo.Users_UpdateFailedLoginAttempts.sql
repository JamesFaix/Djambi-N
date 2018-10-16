CREATE PROCEDURE [dbo].[Users_UpdateFailedLoginAttempts]
	@UserId INT,
	@FailedLoginAttempts INT,
	@LastFailedLoginAttemptOn DATETIME2
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Users WHERE UserId = @UserId)
		THROW 50404, 'User not found.', 1

	UPDATE Users
	SET FailedLoginAttempts = @FailedLoginAttempts,
		LastFailedLoginAttemptOn = @LastFailedLoginAttemptOn
	WHERE UserId = @UserId
END
GO
