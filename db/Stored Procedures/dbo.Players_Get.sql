SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[Players_Get]
	@GameId INT,
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT GameId,
		PlayerId,
		UserId,
		PlayerKindId,
		[Name],
		PlayerStatusId,
		ColorId,
		StartingRegion,
		StartingTurnNumber
	FROM Players
	WHERE GameId = @GameId
		AND (@PlayerId IS NULL OR PlayerId = @PlayerId)
END
GO
