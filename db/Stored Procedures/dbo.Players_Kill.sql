CREATE PROCEDURE [dbo].[Players_Kill]
    @PlayerId INT
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT EXISTS(SELECT 1 FROM Players WHERE PlayerId = @PlayerId)
        THROW 50404, 'Player not found.', 1

    UPDATE Players
    SET IsAlive = 0
    WHERE PlayerId = @PlayerId
END