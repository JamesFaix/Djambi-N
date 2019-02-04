CREATE PROCEDURE [dbo].[Players_Remove]
	@GameId INT,
	@PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(
		SELECT 1
		FROM Players
		WHERE GameId = @GameId
		AND PlayerId = @PlayerId
	)
		THROW 50404, 'Player not found.', 1

	DELETE FROM Players
	WHERE PlayerId = @PlayerId
		AND GameId = @GameId
END