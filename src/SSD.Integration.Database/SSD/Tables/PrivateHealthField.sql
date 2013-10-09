CREATE TABLE [SSD].[PrivateHealthField] (
    [PrivateHealthFieldId] INT NOT NULL,
    [ProviderId]           INT NULL,
    CONSTRAINT [PK_SSD.PrivateHealthField] PRIMARY KEY CLUSTERED ([PrivateHealthFieldId] ASC),
    CONSTRAINT [FK_SSD.PrivateHealthField_SSD.CustomField_PrivateHealthFieldId] FOREIGN KEY ([PrivateHealthFieldId]) REFERENCES [SSD].[CustomField] ([CustomFieldId]),
    CONSTRAINT [FK_SSD.PrivateHealthField_SSD.Provider_ProviderId] FOREIGN KEY ([ProviderId]) REFERENCES [SSD].[Provider] ([ProviderId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ProviderId]
    ON [SSD].[PrivateHealthField]([ProviderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PrivateHealthFieldId]
    ON [SSD].[PrivateHealthField]([PrivateHealthFieldId] ASC);

