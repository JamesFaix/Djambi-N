CREATE PROCEDURE [dbo].[Sessions_Get]
	@SessionId INT = NULL,
	@Token NVARCHAR(50) = NULL,
	@UserId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT s.SessionId,
		u.UserId,
		u.Name AS UserName,
		u.IsAdmin,
		s.Token,
		s.CreatedOn,
		s.ExpiresOn
	FROM [Sessions] s
		INNER JOIN Users u
			ON u.UserId = s.UserId
	WHERE (@SessionId IS NULL OR s.SessionId = @SessionId)
		AND (@Token IS NULL OR s.Token = @Token)
		AND (@UserId IS NULL OR s.UserId = @UserId)
END