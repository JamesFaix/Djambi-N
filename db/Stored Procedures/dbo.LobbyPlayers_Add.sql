CREATE PROCEDURE [dbo].[LobbyPlayers_AddUser] 
	@LobbyId INT,
	@UserId INT,
	@PlayerTypeId TINYINT,
	@Name NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF (SELECT COUNT(1) FROM Players WHERE LobbyId = @LobbyId) 
	 = (SELECT RegionCount FROM Lobbies WHERE LobbyId = @LobbyId)
		THROW 50400, 'Max player count reached.', 1
                      
	IF @PlayerTypeId = 1 --User
	BEGIN
		IF @Name IS NOT NULL
			THROW 50400, 'Cannot set player name if player type is User.', 1

		SET @Name = (SELECT [Name] FROM Users WHERE UserId = @UserId)
	END

	INSERT INTO LobbyPlayers (
		LobbyId, 
		UserId, 
		PlayerTypeId,
		[Name])
	VALUES( 
		@LobbyId, 
		UserId, 
		@PlayerTypeId,
		@Name)
		
	SELECT SCOPE_IDENTITY()
END