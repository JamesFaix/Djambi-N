CREATE PROCEDURE [Lobby].[DeleteGame]
	@GameId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @GameId)
		THROW 50404, 'Game not found.', 1

	DELETE FROM [Messages] WHERE GameId = @GameId
	DELETE FROM Turns WHERE GameId = @GameId
	DELETE FROM Players WHERE GameId = @GameId
    DELETE FROM Games WHERE GameId = @GameId
END