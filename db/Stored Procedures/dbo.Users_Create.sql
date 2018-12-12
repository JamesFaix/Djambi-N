CREATE PROCEDURE [dbo].[Users_Create]
	@Name NVARCHAR(50),
	@Password NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Users (
		[Name],
		CreatedOn,
		IsAdmin,
		[Password],
		FailedLoginAttempts,
		LastFailedLoginAttemptOn)
	VALUES (
		@Name,
		GETUTCDATE(),
		0,
		@Password,
		0,
		NULL)

	SELECT SCOPE_IDENTITY()
END
GO
