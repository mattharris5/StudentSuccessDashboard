CREATE TABLE [SSD].[CustomFieldCategory] (
    [CustomFieldCategoryId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]                  NVARCHAR (30) NOT NULL,
    CONSTRAINT [PK_SSD.CustomFieldCategory] PRIMARY KEY CLUSTERED ([CustomFieldCategoryId] ASC)
);



