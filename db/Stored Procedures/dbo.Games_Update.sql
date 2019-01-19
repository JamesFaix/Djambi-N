CREATE PROCEDURE [dbo].[Games_Update]
	@GameId INT,
    @RegionCount TINYINT,
    @Description NVARCHAR(100),
    @AllowGuests BIT,
    @IsPublic BIT,
    @GameStatusId TINYINT,
    @TurnCycleJson NVARCHAR(MAX),
    @PiecesJson NVARCHAR(MAX),
    @CurrentTurnJson NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @GameId)
        THROW 50404, 'Game not found.', 1

    UPDATE Games
    SET RegionCount = @RegionCount,
        [Description] = @Description,
        AllowGuests = @AllowGuests,
        IsPublic = @IsPublic,
        GameStatusId = @GameStatusId,
        TurnCycleJson = @TurnCycleJson,
        PiecesJson = @PiecesJson,
        CurrentTurnJson = @CurrentTurnJson
    WHERE GameId = @GameId
END