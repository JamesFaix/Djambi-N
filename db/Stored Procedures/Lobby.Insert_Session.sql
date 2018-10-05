SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO



CREATE PROCEDURE [Lobby].[Insert_Session]
	@UserId INT,
	@Token NVARCHAR(50),
	@ExpiresOn DATETIME2	
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION
		--Start session
		INSERT INTO Sessions (UserId, Token, CreatedOn, ExpiresOn)
		VALUES (@UserId, @Token, GETUTCDATE(), @ExpiresOn)

		--Reset failed login attempts
		UPDATE Users
		SET LastFailedLoginAttemptOn = NULL,
			FailedLoginAttempts = 0
		WHERE UserId = @UserId

	COMMIT
END
GO
