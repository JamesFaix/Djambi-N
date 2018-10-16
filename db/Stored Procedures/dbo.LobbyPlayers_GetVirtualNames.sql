CREATE PROCEDURE [dbo].[LobbyPlayers_GetVirtualNames] 
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Name] 
	FROM VirtualPlayerNames	
END
GO
