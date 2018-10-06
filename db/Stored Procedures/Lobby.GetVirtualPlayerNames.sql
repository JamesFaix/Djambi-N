CREATE PROCEDURE [Lobby].[GetVirtualPlayerNames] 
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Name] 
	FROM VirtualPlayerNames	
END
GO
