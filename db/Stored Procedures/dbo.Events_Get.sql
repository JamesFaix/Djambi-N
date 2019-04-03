CREATE PROCEDURE [dbo].[Events_Get]
    @EventId INT,
    @GameId INT,
    @Ascending BIT = 1,
    @MaxResults INT = NULL,
    @ThresholdTime DATETIME = NULL,
    @ThresholdEventId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

    -- Stash all the events for the game in a temp table

    SELECT
        e.EventId,
        e.GameId,
        e.CreatedByUserId,
        u.Name as CreatedByUserName,
        e.ActingPlayerId,
        e.CreatedOn,
        e.EventKindId,
        e.EffectsJson
    INTO #events
    FROM Events e
        INNER JOIN Users u
            ON e.CreatedByUserId = u.UserId
    WHERE GameId = @GameId
        AND (@EventId IS NULL OR EventId = @EventId)

    -- Filter temp table

    IF @ThresholdTime IS NOT NULL
        BEGIN
            IF @Ascending = 1
                -- Only return events after the threshold time
                DELETE FROM #events
                WHERE CreatedOn <= @ThresholdTime
            ELSE
                -- Only return events before the threshold time
                DELETE FROM #events
                WHERE CreatedOn >= @ThresholdTime
        END

    IF @ThresholdEventId IS NOT NULL
        BEGIN
            IF @Ascending = 1
                -- Only return events after the threshold event
                DELETE FROM #events
                WHERE EventId <= @ThresholdEventId
            ELSE
                -- Only return events before the threshold event
                DELETE FROM #events
                WHERE EventId >= @ThresholdEventId
        END

    -- Limit result count

    DECLARE @MaxSupportedResults INT = 100000 --It seems very unlikely that a game would ever have this many events
    SET @MaxResults = ISNULL(@MaxResults, @MaxSupportedResults)
    IF @MaxResults > @MaxSupportedResults
        SET @MaxResults = @MaxSupportedResults

    IF @Ascending = 1
        SELECT TOP (@MaxResults) * FROM #events ORDER BY EventID ASC
    ELSE
        SELECT TOP (@MaxResults) * FROM #events ORDER BY EventID DESC
END