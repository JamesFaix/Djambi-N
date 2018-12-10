CREATE PROCEDURE [dbo].[Games_Delete]
	@GameId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @GameId)
		THROW 50404, 'Game not found.', 1

	BEGIN TRAN
		DELETE FROM Players WHERE GameId = @GameId

		DELETE FROM Games WHERE GameId = @GameId
	COMMIT
END