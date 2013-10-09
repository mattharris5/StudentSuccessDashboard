CREATE TABLE [SSD].[UserRoleProviders] (
    [UserRoleId] INT NOT NULL,
    [ProviderId] INT NOT NULL,
    CONSTRAINT [PK_SSD.UserRoleProviders] PRIMARY KEY CLUSTERED ([UserRoleId] ASC, [ProviderId] ASC),
    CONSTRAINT [FK_SSD.UserRoleProviders_SSD.Provider_ProviderId] FOREIGN KEY ([ProviderId]) REFERENCES [SSD].[Provider] ([ProviderId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.UserRoleProviders_SSD.UserRoles_UserRoleId] FOREIGN KEY ([UserRoleId]) REFERENCES [SSD].[UserRoles] ([UserRoleId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ProviderId]
    ON [SSD].[UserRoleProviders]([ProviderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserRoleId]
    ON [SSD].[UserRoleProviders]([UserRoleId] ASC);

