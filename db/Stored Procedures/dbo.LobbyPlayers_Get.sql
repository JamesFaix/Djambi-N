CREATE PROCEDURE [dbo].[LobbyPlayers_Get] 
	@LobbyId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT LobbyId, 
		LobbyPlayerId,
		UserId,
		PlayerTypeId,
		[Name]
    FROM LobbyPlayers
	WHERE LobbyId = @LobbyId
END