CREATE TABLE [dbo].[SessionUsers]
(
    [SessionId] INT NOT NULL, 
    [UserId]    INT NOT NULL,
    CONSTRAINT [PK_SessionUsers]           PRIMARY KEY CLUSTERED ([SessionId] ASC, [UserId] ASC),	
    CONSTRAINT [FK_SessionUsers_UserId]    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_SessionUsers_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Sessions] ([SessionId]),
    CONSTRAINT [UQ_SessionUsers_UserId]    UNIQUE NONCLUSTERED  ([UserId])
)