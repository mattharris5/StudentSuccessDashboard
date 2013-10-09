CREATE TABLE [SSD].[CustomFieldCategoryMapping] (
    [CustomFieldId]         INT NOT NULL,
    [CustomFieldCategoryId] INT NOT NULL,
    CONSTRAINT [PK_SSD.CustomFieldCategoryMapping] PRIMARY KEY CLUSTERED ([CustomFieldId] ASC, [CustomFieldCategoryId] ASC),
    CONSTRAINT [FK_SSD.CustomFieldCategoryMapping_SSD.CustomField_CustomFieldId] FOREIGN KEY ([CustomFieldId]) REFERENCES [SSD].[CustomField] ([CustomFieldId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.CustomFieldCategoryMapping_SSD.CustomFieldCategory_CustomFieldCategoryId] FOREIGN KEY ([CustomFieldCategoryId]) REFERENCES [SSD].[CustomFieldCategory] ([CustomFieldCategoryId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomFieldCategoryId]
    ON [SSD].[CustomFieldCategoryMapping]([CustomFieldCategoryId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomFieldId]
    ON [SSD].[CustomFieldCategoryMapping]([CustomFieldId] ASC);

