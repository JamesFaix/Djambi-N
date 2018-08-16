CREATE PROCEDURE [dbo].[Delete_User]
	@UserId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Users WHERE UserId = @UserId)
		THROW 50000, 'User not found', 1
        
    DELETE FROM Users
    WHERE UserId = @UserId
END
GO


