CREATE TABLE [SSD].[CustomFieldValue] (
    [CustomFieldValueId] INT            IDENTITY (1, 1) NOT NULL,
    [CustomFieldId]      INT            NOT NULL,
    [CustomDataOriginId] INT            NOT NULL,
    [Value]              NVARCHAR (MAX) NULL,
    [StudentId]          INT            NOT NULL,
    CONSTRAINT [PK_SSD.CustomFieldValue] PRIMARY KEY CLUSTERED ([CustomFieldValueId] ASC),
    CONSTRAINT [FK_SSD.CustomFieldValue_SSD.CustomDataOrigin_CustomDataOriginId] FOREIGN KEY ([CustomDataOriginId]) REFERENCES [SSD].[CustomDataOrigin] ([CustomDataOriginId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.CustomFieldValue_SSD.CustomField_CustomFieldId] FOREIGN KEY ([CustomFieldId]) REFERENCES [SSD].[CustomField] ([CustomFieldId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.CustomFieldValue_SSD.Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [SSD].[Student] ([StudentId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_StudentId]
    ON [SSD].[CustomFieldValue]([StudentId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomDataOriginId]
    ON [SSD].[CustomFieldValue]([CustomDataOriginId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomFieldId]
    ON [SSD].[CustomFieldValue]([CustomFieldId] ASC);

