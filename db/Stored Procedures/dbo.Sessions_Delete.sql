CREATE PROCEDURE [dbo].[Sessions_Delete]
	@SessionId INT = NULL,
	@Token NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@SessionId IS NOT NULL)
	AND (@Token IS NOT NULL)
		THROW 50500, 'Cannot delete session using both Id and Token, must use one or the other.', 1

	DELETE 
	FROM Sessions
	WHERE SessionId = @SessionId
		OR Token = @Token
END