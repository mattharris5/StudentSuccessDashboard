CREATE TABLE [SSD].[PublicField] (
    [PublicFieldId] INT NOT NULL,
    CONSTRAINT [PK_SSD.PublicField] PRIMARY KEY CLUSTERED ([PublicFieldId] ASC),
    CONSTRAINT [FK_SSD.PublicField_SSD.CustomField_PublicFieldId] FOREIGN KEY ([PublicFieldId]) REFERENCES [SSD].[CustomField] ([CustomFieldId])
);


GO
CREATE NONCLUSTERED INDEX [IX_PublicFieldId]
    ON [SSD].[PublicField]([PublicFieldId] ASC);

