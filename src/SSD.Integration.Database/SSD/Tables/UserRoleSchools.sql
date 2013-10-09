CREATE TABLE [SSD].[UserRoleSchools] (
    [UserRoleId] INT NOT NULL,
    [SchoolId]   INT NOT NULL,
    CONSTRAINT [PK_SSD.UserRoleSchools] PRIMARY KEY CLUSTERED ([UserRoleId] ASC, [SchoolId] ASC),
    CONSTRAINT [FK_SSD.UserRoleSchools_SSD.School_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [SSD].[School] ([SchoolId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.UserRoleSchools_SSD.UserRoles_UserRoleId] FOREIGN KEY ([UserRoleId]) REFERENCES [SSD].[UserRoles] ([UserRoleId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_SchoolId]
    ON [SSD].[UserRoleSchools]([SchoolId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserRoleId]
    ON [SSD].[UserRoleSchools]([UserRoleId] ASC);

