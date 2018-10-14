CREATE PROCEDURE [Lobby].[GetSessionWithUsers]
	@SessionId INT = NULL,
	@Token NVARCHAR(50) = NULL,
	@UserId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@SessionId IS NOT NULL)
	AND (@Token IS NOT NULL)
		THROW 50500, 'Cannot get session using both Id and Token, must use one or the other.', 1
		
	SELECT s.SessionId,
		s.Token,
		s.PrimaryUserId,
		s.CreatedOn,
		s.ExpiresOn,
		s.IsShared,
		su.UserId
	FROM [Sessions] s
		LEFT OUTER JOIN SessionUsers su
			ON s.SessionId = su.SessionId
	WHERE (@SessionId IS NULL OR s.SessionId = @SessionId)
		AND (@Token IS NULL OR s.Token = @Token)
		AND (@UserId IS NULL OR su.UserId = @UserId)
END