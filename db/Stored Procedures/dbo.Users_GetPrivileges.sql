CREATE PROCEDURE [dbo].[Users_GetPrivileges]
	@UserId INT,
	@Name NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT up.PrivilegeId
    FROM UserPrivileges up
        INNER JOIN Users u
            ON up.UserId = u.UserId
	WHERE (@UserId IS NULL OR @UserId = u.UserId)
		AND (@Name IS NULL OR @Name = u.[Name])
END
GO
