CREATE PROCEDURE [dbo].[Players_Get]
	@GameIds NVARCHAR(MAX),
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF @PlayerId IS NOT NULL
	BEGIN
		SELECT GameId,
			PlayerId,
			UserId,
			PlayerKindId,
			[Name],
			IsAlive,
			ColorId,
			StartingRegion,
			StartingTurnNumber
		FROM Players
		WHERE PlayerId = @PlayerId
	END

	ELSE IF @GameIds IS NOT NULL
	BEGIN
		SELECT value
		INTO #GameIds
		FROM STRING_SPLIT(@GameIds, ',') --TODO: Vendor dependency

		SELECT p.GameId,
			p.PlayerId,
			p.UserId,
			p.PlayerKindId,
			p.[Name],
			p.IsAlive,
			p.ColorId,
			p.StartingRegion,
			p.StartingTurnNumber
		FROM Players p
			INNER JOIN #GameIds g
				ON p.GameId = g.value
		ORDER BY p.PlayerId
	END
	ELSE
		THROW 50500, 'Invalid parameters for player query.', 1
END