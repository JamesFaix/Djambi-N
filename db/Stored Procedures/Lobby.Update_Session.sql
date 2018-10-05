SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO


CREATE PROCEDURE [Lobby].[Update_Session]
	@UserId INT,
	@ExpiresOn DATETIME2
AS
BEGIN	
	DECLARE @SessionId INT = (SELECT SessionId FROM [Sessions] WHERE UserId = @UserId)

	IF @SessionId IS NULL
		THROW 50000, 'Session not found', 1

	UPDATE [Sessions]
	SET ExpiresOn = @ExpiresOn
	WHERE SessionId = @SessionId
END
GO
