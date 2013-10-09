CREATE TABLE [SSD].[Property] (
    [PropertyId]  INT            IDENTITY (1, 1) NOT NULL,
    [EntityName]  NVARCHAR (255) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [IsProtected] BIT            NOT NULL,
    CONSTRAINT [PK_SSD.Property] PRIMARY KEY CLUSTERED ([PropertyId] ASC),
    CONSTRAINT [UX_Property_EntityName_Name] UNIQUE NONCLUSTERED ([EntityName] ASC, [Name] ASC)
);

