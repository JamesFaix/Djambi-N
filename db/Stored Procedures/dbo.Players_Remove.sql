CREATE PROCEDURE [dbo].[Players_Remove]
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserID INT = (SELECT TOP 1 UserId FROM Players WHERE PlayerId = @PlayerId)
	DECLARE @PlayerTypeId TINYINT = (SELECT TOP 1 PlayerTypeId FROM Players WHERE PlayerId = @PlayerId)
	DECLARE @LobbyId INT = (SELECT TOP 1 LobbyId FROM Players WHERE PlayerId = @PlayerId)
	DECLARE @LobbyCreatorUserId INT = (SELECT TOP 1 CreatedByUserId FROM Lobbies WHERE LobbyId = @LobbyId)

	IF @PlayerTypeId IS NULL
		THROW 50404, 'Player not found.', 1

	BEGIN TRAN
		--If the removed player is the user who created the lobby, close the lobby
		IF (@UserId IS NOT NULL)
		AND (@PlayerTypeId = 1) --USER
		AND (@UserId = @LobbyCreatorUserId)
		BEGIN
			DELETE FROM Players WHERE LobbyId = @LobbyId
			DELETE FROM Lobbies WHERE LobbyId = @LobbyId
		END

		ELSE
		BEGIN
			--If the removed player is a user, remove that user and all guests
		    IF @PlayerTypeId = 1 --USER
				DELETE FROM Players WHERE UserId = @UserId

            --Otherwise just delete that player
			ELSE
				DELETE FROM Players WHERE PlayerId = @PlayerId
		END
	COMMIT
END