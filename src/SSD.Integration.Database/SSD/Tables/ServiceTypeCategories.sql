CREATE TABLE [SSD].[ServiceTypeCategories] (
    [ServiceTypeId] INT NOT NULL,
    [CategoryId]    INT NOT NULL,
    CONSTRAINT [PK_SSD.ServiceTypeCategories] PRIMARY KEY CLUSTERED ([ServiceTypeId] ASC, [CategoryId] ASC),
    CONSTRAINT [FK_SSD.ServiceTypeCategories_SSD.Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [SSD].[Category] ([CategoryId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceTypeCategories_SSD.ServiceType_ServiceTypeId] FOREIGN KEY ([ServiceTypeId]) REFERENCES [SSD].[ServiceType] ([ServiceTypeId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CategoryId]
    ON [SSD].[ServiceTypeCategories]([CategoryId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceTypeId]
    ON [SSD].[ServiceTypeCategories]([ServiceTypeId] ASC);

