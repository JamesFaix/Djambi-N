CREATE PROCEDURE [dbo].[Players_Get] 
	@LobbyId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT LobbyId, 
		PlayerId,
		UserId,
		PlayerTypeId,
		[Name]
    FROM Players
	WHERE LobbyId = @LobbyId
END