CREATE TABLE [SSD].[Provider] (
    [ProviderId]     INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (255) NOT NULL,
    [Address_City]   NVARCHAR (50)  NULL,
    [Address_State]  NVARCHAR (50)  NULL,
    [Address_Street] NVARCHAR (50)  NULL,
    [Address_Zip]    NVARCHAR (10)  NULL,
    [Website]        NVARCHAR (MAX) NULL,
    [Contact_Name]   NVARCHAR (200) NULL,
    [Contact_Phone]  NVARCHAR (15)  NULL,
    [Contact_Email]  NVARCHAR (255) NULL,
    [IsActive]       BIT            NOT NULL,
    CONSTRAINT [PK_SSD.Provider] PRIMARY KEY CLUSTERED ([ProviderId] ASC),
    CONSTRAINT [UX_Provider_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



