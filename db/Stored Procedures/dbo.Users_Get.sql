CREATE PROCEDURE [dbo].[Users_Get]
	@UserId INT,
	@Name NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT UserId,
		[Name],
		IsAdmin,
		[Password],
		FailedLoginAttempts,
		LastFailedLoginAttemptOn
	FROM Users
	WHERE (@UserId IS NULL OR @UserId = UserId)
		AND (@Name IS NULL OR @Name = [Name])
END
GO
