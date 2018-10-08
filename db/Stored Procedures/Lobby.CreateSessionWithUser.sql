CREATE PROCEDURE [Lobby].[CreateSessionWithUser]
	@UserId INT,
	@Token NVARCHAR(50),
	@ExpiresOn DATETIME2
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION

		--Start session
		INSERT INTO Sessions (
			IsShared, 
			Token, 
			CreatedOn, 
			ExpiresOn)
		VALUES (
			0, 
			@Token, 
			GETUTCDATE(),
      	    @ExpiresOn)

		DECLARE @SessionId INT = SCOPE_IDENTITY()

		EXEC Lobby.AddUserToSession 
			@SessionId = @SessionId, 
			@UserId = @UserId

		SELECT @SessionId

	COMMIT
END