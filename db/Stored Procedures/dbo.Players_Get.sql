CREATE PROCEDURE [dbo].[Players_Get]
	@LobbyId INT,
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT LobbyId,
		PlayerId,
		UserId,
		PlayerTypeId,
		[Name]
    FROM Players
	WHERE (LobbyId = @LobbyId OR @LobbyId IS NULL)
		AND (PlayerId = @PlayerId OR @PlayerId IS NULL)
END