CREATE TABLE [SSD].[CustomField] (
    [CustomFieldId]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [CreateTime]          DATETIME2 (7)  NOT NULL,
    [CreatingUserId]      INT            NOT NULL,
    [LastModifyTime]      DATETIME       NULL,
    [LastModifyingUserId] INT            NULL,
    [CustomFieldTypeId]   INT            NOT NULL,
    CONSTRAINT [PK_SSD.CustomField] PRIMARY KEY CLUSTERED ([CustomFieldId] ASC),
    CONSTRAINT [FK_SSD.CustomField_SSD.CustomFieldType_CustomFieldTypeId] FOREIGN KEY ([CustomFieldTypeId]) REFERENCES [SSD].[CustomFieldType] ([CustomFieldTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.CustomField_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId]),
    CONSTRAINT [FK_SSD.CustomField_SSD.User_LastModifyingUserId] FOREIGN KEY ([LastModifyingUserId]) REFERENCES [SSD].[User] ([UserId]),
    CONSTRAINT [UX_CustomField_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_CustomFieldTypeId]
    ON [SSD].[CustomField]([CustomFieldTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[CustomField]([CreatingUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastModifyingUserId]
    ON [SSD].[CustomField]([LastModifyingUserId] ASC);

