CREATE PROCEDURE [dbo].[Sessions_Get]
	@SessionId INT = NULL,
	@Token NVARCHAR(50) = NULL,
	@UserId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT s.SessionId,
		s.UserId,
		s.Token,
		s.CreatedOn,
		s.ExpiresOn,
		u.IsAdmin
	FROM [Sessions]
		INNER JOIN Users u
			ON u.UserId = s.UserId
	WHERE (@SessionId IS NULL OR s.SessionId = @SessionId)
		AND (@Token IS NULL OR s.Token = @Token)
		AND (@UserId IS NULL OR s.UserId = @UserId)
END