CREATE PROCEDURE [dbo].[Sessions_Renew]
	@SessionId INT,
	@ExpiresOn DATETIME2
AS
BEGIN		
	IF NOT EXISTS(SELECT 1 FROM Sessions WHERE SessionId = @SessionId)
		THROW 50404, 'Session not found.', 1

	UPDATE [Sessions]
	SET ExpiresOn = @ExpiresOn
	WHERE SessionId = @SessionId
END