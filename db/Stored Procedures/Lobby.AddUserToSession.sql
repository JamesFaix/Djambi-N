CREATE PROCEDURE Lobby.AddUserToSession
	@SessionId INT,
	@UserId INT
AS

BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION
		
		INSERT INTO SessionUsers (SessionId, UserId)
		VALUES (@SessionId, @UserId)

		--Reset failed login attempts
		UPDATE Users
		SET LastFailedLoginAttemptOn = NULL,
			FailedLoginAttempts = 0
		WHERE UserId = @UserId

	COMMIT

END