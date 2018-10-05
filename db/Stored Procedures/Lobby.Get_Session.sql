SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

CREATE PROCEDURE [Lobby].[Get_Session]
	@UserId INT = NULL,
	@Token NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT SessionId,
		UserId,
		Token,
		CreatedOn,
		ExpiresOn
	FROM [Sessions]
	WHERE (UserId = @UserId OR @UserId IS NULL)
		AND (Token = @Token OR @Token IS NULL)
END
GO
