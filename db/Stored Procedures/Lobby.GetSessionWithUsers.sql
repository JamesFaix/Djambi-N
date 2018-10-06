﻿CREATE PROCEDURE [Lobby].[GetSessionWithUsers]
	@SessionId INT = NULL,
	@Token NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@SessionId IS NOT NULL)
	AND (@Token IS NOT NULL)
		THROW 50000, 'Cannot get session using both Id and Token, must use one or the other.', 1
		
	SELECT s.SessionId,
		s.Token,
		s.CreatedOn,
		s.ExpiresOn,
		s.IsShared,
		su.UserId
	FROM [Sessions] s
		LEFT OUTER JOIN SessionUsers su
			ON s.SessionId = su.SessionId
	WHERE s.SessionId = @SessionId
		OR s.Token = @Token
END