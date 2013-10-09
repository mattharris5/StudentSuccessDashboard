CREATE TABLE [SSD].[CustomFieldType] (
    [CustomFieldTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (30) NOT NULL,
    CONSTRAINT [PK_SSD.CustomFieldType] PRIMARY KEY CLUSTERED ([CustomFieldTypeId] ASC)
);



