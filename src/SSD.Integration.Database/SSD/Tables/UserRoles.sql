CREATE TABLE [SSD].[UserRoles] (
    [UserRoleId]          INT           IDENTITY (1, 1) NOT NULL,
    [UserId]              INT           NOT NULL,
    [RoleId]              INT           NOT NULL,
    [CreateTime]          DATETIME2 (7) NOT NULL,
    [CreatingUserId]      INT           NOT NULL,
    [LastModifyTime]      DATETIME      NULL,
    [LastModifyingUserId] INT           NULL,
    CONSTRAINT [PK_SSD.UserRoles] PRIMARY KEY CLUSTERED ([UserRoleId] ASC),
    CONSTRAINT [FK_SSD.UserRoles_SSD.Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [SSD].[Role] ([RoleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.UserRoles_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId]),
    CONSTRAINT [FK_SSD.UserRoles_SSD.User_LastModifyingUserId] FOREIGN KEY ([LastModifyingUserId]) REFERENCES [SSD].[User] ([UserId]),
    CONSTRAINT [FK_SSD.UserRoles_SSD.User_UserId] FOREIGN KEY ([UserId]) REFERENCES [SSD].[User] ([UserId]),
    CONSTRAINT [UX_UserRoles_UserId_RoleId] UNIQUE NONCLUSTERED ([UserId] ASC, [RoleId] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_RoleId]
    ON [SSD].[UserRoles]([RoleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [SSD].[UserRoles]([UserId] ASC);


GO



GO
CREATE NONCLUSTERED INDEX [IX_LastModifyingUserId]
    ON [SSD].[UserRoles]([LastModifyingUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[UserRoles]([CreatingUserId] ASC);

