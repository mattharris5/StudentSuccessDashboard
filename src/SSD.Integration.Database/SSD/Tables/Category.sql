CREATE TABLE [SSD].[Category] (
    [CategoryId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SSD.Category] PRIMARY KEY CLUSTERED ([CategoryId] ASC),
    CONSTRAINT [UX_Category_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

