CREATE PROCEDURE Lobby.RemoveUserFromSession
	@SessionId INT,
	@UserId INT
AS

BEGIN
	SET NOCOUNT ON;
		
	BEGIN TRAN
		DELETE FROM SessionUsers 
		WHERE SessionId = @SessionId 
			AND UserId = @UserId

		IF NOT EXISTS (SELECT 1 FROM SessionUsers WHERE SessionId = @SessionId)
			DELETE FROM Sessions
			WHERE SessionId = @SessionId
	COMMIT
END