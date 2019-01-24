CREATE PROCEDURE [dbo].[Players_Update]
    @PlayerId INT,
    @PlayerStatusId TINYINT,
    @ColorId TINYINT,
    @StartingRegion TINYINT,
    @StartingTurnNumber TINYINT
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT EXISTS(SELECT 1 FROM Players WHERE PlayerId = @PlayerId)
        THROW 50404, 'Player not found.', 1

    UPDATE Players
    SET ColorId = @ColorId,
        StartingRegion = @StartingRegion,
        StartingTurnNumber = @StartingTurnNumber,
        PlayerStatusId = @PlayerStatusId
    WHERE PlayerId = @PlayerId
END