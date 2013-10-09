CREATE TABLE [SSD].[StudentProviders] (
    [StudentId]  INT NOT NULL,
    [ProviderId] INT NOT NULL,
    CONSTRAINT [PK_SSD.StudentProviders] PRIMARY KEY CLUSTERED ([StudentId] ASC, [ProviderId] ASC),
    CONSTRAINT [FK_SSD.StudentProviders_SSD.Provider_ProviderId] FOREIGN KEY ([ProviderId]) REFERENCES [SSD].[Provider] ([ProviderId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.StudentProviders_SSD.Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [SSD].[Student] ([StudentId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ProviderId]
    ON [SSD].[StudentProviders]([ProviderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StudentId]
    ON [SSD].[StudentProviders]([StudentId] ASC);

