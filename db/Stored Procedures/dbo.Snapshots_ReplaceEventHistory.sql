SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[Snapshots_ReplaceEventHistory]
	@GameId INT,
    @History EventList READONLY
AS
BEGIN
	SET NOCOUNT ON;

    IF NOT EXISTS(SELECT 1 FROM Games WHERE GameId = @GameId)
        THROW 50404, 'Game not found.', 1

    BEGIN TRAN
        DELETE FROM Events
        WHERE GameId = @GameId

        INSERT INTO Events
        SELECT @GameId,
            CreatedByUserId,
            ActingPlayerId,
            CreatedOn,
            EventKindId,
            EffectsJson
        FROM @History
    COMMIT
END
GO
