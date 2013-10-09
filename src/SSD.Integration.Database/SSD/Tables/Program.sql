CREATE TABLE [SSD].[Program] (
    [ProgramId]         INT            IDENTITY (1, 1) NOT NULL,
    [IsActive]          BIT            NOT NULL,
    [Name]              NVARCHAR (255) NOT NULL,
    [StartDate]         DATETIME       NULL,
    [EndDate]           DATETIME       NULL,
    [Purpose]           NVARCHAR (MAX) NULL,
    [ContactInfo_Name]  NVARCHAR (200) NULL,
    [ContactInfo_Phone] NVARCHAR (15)  NULL,
    [ContactInfo_Email] NVARCHAR (255) NULL,
    CONSTRAINT [PK_SSD.Program] PRIMARY KEY CLUSTERED ([ProgramId] ASC)
);



