CREATE TABLE [SSD].[FavoriteServiceOffering] (
    [ServiceOfferingId] INT NOT NULL,
    [UserId]            INT NOT NULL,
    CONSTRAINT [PK_SSD.FavoriteServiceOffering] PRIMARY KEY CLUSTERED ([ServiceOfferingId] ASC, [UserId] ASC),
    CONSTRAINT [FK_SSD.FavoriteServiceOffering_SSD.ServiceOffering_ServiceOfferingId] FOREIGN KEY ([ServiceOfferingId]) REFERENCES [SSD].[ServiceOffering] ([ServiceOfferingId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.FavoriteServiceOffering_SSD.User_UserId] FOREIGN KEY ([UserId]) REFERENCES [SSD].[User] ([UserId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [SSD].[FavoriteServiceOffering]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceOfferingId]
    ON [SSD].[FavoriteServiceOffering]([ServiceOfferingId] ASC);

