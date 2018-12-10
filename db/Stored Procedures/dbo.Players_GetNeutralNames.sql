CREATE PROCEDURE [dbo].[Players_GetNeutralNames]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Name]
	FROM NeutralPlayerNames
END
GO
