--TODO: Deprecate this and use Games_Update
CREATE PROCEDURE [dbo].[Games_UpdateParameters]
	@GameId INT,
    @RegionCount INT,
	@Description NVARCHAR(100),
	@AllowGuests BIT,
	@IsPublic BIT
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @GameId)
        THROW 50404, 'Game not found.', 1

	BEGIN TRAN

        --Remove guests if disabling guests
        IF @AllowGuests = 0
            DELETE FROM Players
            WHERE GameId = @GameId
                AND PlayerKindId = 2 --Guest

        --Remove extra players if reducing region count
        DELETE p1
        FROM Players p1
            INNER JOIN (
                SELECT PlayerId, ROW_NUMBER() OVER(ORDER BY PlayerId) AS [Row]
                FROM Players
                WHERE GameId = @GameId
            ) p2
            ON p1.PlayerId = p2.PlayerId
        WHERE p2.[Row] > @RegionCount

		UPDATE Games
        SET RegionCount = @RegionCount,
            [Description] = @Description,
            AllowGuests = @AllowGuests,
            IsPublic = @IsPublic
        WHERE GameId = @GameId
	COMMIT
END