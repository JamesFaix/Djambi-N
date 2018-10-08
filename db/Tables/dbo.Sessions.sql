CREATE TABLE [dbo].[Sessions] (
    [SessionId] INT           IDENTITY (1, 1) NOT NULL,
    [Token]     NVARCHAR (50) NOT NULL,
    [CreatedOn] DATETIME2 (7) NOT NULL,
    [ExpiresOn] DATETIME2 (7) NOT NULL,
    [IsShared]  BIT           NOT NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([SessionId] ASC),
    CONSTRAINT [UQ_Sessions_Token] UNIQUE NONCLUSTERED ([Token] ASC)
);