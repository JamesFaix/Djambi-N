SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Lobby].[Update_UserLoginData]
	@UserId INT,
	@FailedLoginAttempts TINYINT,
	@LastFailedLoginAttemptOn DATETIME2,
	@ActiveSessionToken NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Users WHERE UserId = @UserId)
		THROW 50000, 'User not found', 1

	UPDATE Users
	SET FailedLoginAttempts = @FailedLoginAttempts,
		LastFailedLoginAttemptOn = @LastFailedLoginAttemptOn,
		ActiveSessionToken = @ActiveSessionToken
	WHERE UserId = @UserId
END
GO
