SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[Players_Get]
	@GameIds dbo.Int32List READONLY,
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
			PlayerStatusId,
			ColorId,
			StartingRegion,
			StartingTurnNumber
		FROM Players
		WHERE PlayerId = @PlayerId
	END

	ELSE IF EXISTS(SELECT 1 FROM @GameIds)
	BEGIN
		SELECT p.GameId,
			p.PlayerId,
			p.UserId,
			p.PlayerKindId,
			p.[Name],
			p.PlayerStatusId,
			p.ColorId,
			p.StartingRegion,
			p.StartingTurnNumber
		FROM Players p
			INNER JOIN @GameIds g
				ON p.GameId = g.N
		ORDER BY p.PlayerId
	END
	ELSE
		THROW 50500, 'Invalid parameters for player query.', 1
END
GO
