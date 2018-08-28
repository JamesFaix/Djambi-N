SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO


CREATE PROCEDURE [Lobby].[Get_Users]
	@UserId INT,
	@Name NVARCHAR(50),
	@ActiveSessionToken NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT UserId AS Id, 
		[Name], 
		RoleId, 
		[Password], 
		FailedLoginAttempts, 
		LastFailedLoginAttemptOn, 
		ActiveSessionToken
	FROM Users
	WHERE (@UserId IS NULL OR @UserId = UserId)
		AND (@Name IS NULL OR @Name = [Name])
		AND (@ActiveSessionToken IS NULL OR @ActiveSessionToken = ActiveSessionToken)
END
GO
