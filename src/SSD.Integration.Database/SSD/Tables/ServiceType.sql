CREATE TABLE [SSD].[ServiceType] (
    [ServiceTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [IsActive]      BIT            NOT NULL,
    [Name]          NVARCHAR (255) NOT NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [IsPrivate]     BIT            NOT NULL,
    CONSTRAINT [PK_SSD.ServiceType] PRIMARY KEY CLUSTERED ([ServiceTypeId] ASC),
    CONSTRAINT [UX_ServiceType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



