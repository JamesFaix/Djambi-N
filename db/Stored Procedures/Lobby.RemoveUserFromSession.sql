CREATE PROCEDURE Lobby.RemoveUserFromSession
	@SessionId INT,
	@UserId INT
AS

BEGIN
	SET NOCOUNT ON;
		
	DECLARE @PrimaryUserId INT =
		(SELECT PrimaryUserId 
		FROM Sessions 
		WHERE SessionId = @SessionId)

	IF @UserId = @PrimaryUserId
	BEGIN
		BEGIN TRAN
			DELETE FROM SessionUsers
			WHERE SessionId = @SessionId

			DELETE FROM Sessions
			WHERE SessionId = @SessionId
		COMMIT
	END
	ELSE	
		DELETE FROM SessionUsers 
		WHERE SessionId = @SessionId 
			AND UserId = @UserId

END