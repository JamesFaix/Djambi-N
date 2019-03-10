CREATE TABLE [dbo].[UserPrivileges]
(
    [UserId]      INT NOT NULL,
    [PrivilegeId] TINYINT NOT NULL,
    CONSTRAINT [PK_UserPrivileges] PRIMARY KEY CLUSTERED  ([UserId], [PrivilegeId]),
    CONSTRAINT [FK_UserPrivileges_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_UserPrivileges_PrivilegeId] FOREIGN KEY ([PrivilegeId]) REFERENCES [dbo].[Privileges] ([PrivilegeId])
)