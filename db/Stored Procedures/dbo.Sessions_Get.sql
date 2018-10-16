CREATE PROCEDURE [dbo].[Sessions_Get]
	@SessionId INT = NULL,
	@Token NVARCHAR(50) = NULL,
	@UserId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT SessionId,
		UserId,
		Token,
		CreatedOn,
		ExpiresOn
	FROM [Sessions]
	WHERE (@SessionId IS NULL OR SessionId = @SessionId)
		AND (@Token IS NULL OR Token = @Token)
		AND (@UserId IS NULL OR UserId = @UserId)
END