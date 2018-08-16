
CREATE PROCEDURE [dbo].[Get_User]
	@UserId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT UserId AS Id, [Name]
	FROM Users
	WHERE UserId = @UserId
END
GO
