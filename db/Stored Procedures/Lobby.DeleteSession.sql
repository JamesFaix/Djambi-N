CREATE PROCEDURE [Lobby].[DeleteSession]
	@SessionId INT = NULL,
	@Token NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@SessionId IS NOT NULL)
	AND (@Token IS NOT NULL)
		THROW 50500, 'Cannot delete session using both Id and Token, must use one or the other.', 1

	BEGIN TRAN

		DELETE su
		FROM SessionUsers su
			INNER JOIN Sessions s
				ON su.SessionId = s.SessionId
		WHERE s.SessionId = @SessionId
			OR s.Token = @Token

		DELETE 
		FROM Sessions
		WHERE SessionId = @SessionId
			OR Token = @Token

	COMMIT
END