CREATE TABLE [SSD].[SchoolProviders] (
    [SchoolId]   INT NOT NULL,
    [ProviderId] INT NOT NULL,
    CONSTRAINT [PK_SSD.SchoolProviders] PRIMARY KEY CLUSTERED ([SchoolId] ASC, [ProviderId] ASC),
    CONSTRAINT [FK_SSD.SchoolProviders_SSD.Provider_ProviderId] FOREIGN KEY ([ProviderId]) REFERENCES [SSD].[Provider] ([ProviderId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.SchoolProviders_SSD.School_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [SSD].[School] ([SchoolId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ProviderId]
    ON [SSD].[SchoolProviders]([ProviderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SchoolId]
    ON [SSD].[SchoolProviders]([SchoolId] ASC);

