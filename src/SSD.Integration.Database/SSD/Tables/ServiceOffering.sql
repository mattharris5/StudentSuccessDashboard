CREATE TABLE [SSD].[ServiceOffering] (
    [ServiceOfferingId] INT IDENTITY (1, 1) NOT NULL,
    [IsActive]          BIT NOT NULL,
    [ProviderId]        INT NOT NULL,
    [ServiceTypeId]     INT NOT NULL,
    [ProgramId]         INT NULL,
    CONSTRAINT [PK_SSD.ServiceOffering] PRIMARY KEY CLUSTERED ([ServiceOfferingId] ASC),
    CONSTRAINT [FK_SSD.ServiceOffering_SSD.Program_ProgramId] FOREIGN KEY ([ProgramId]) REFERENCES [SSD].[Program] ([ProgramId]),
    CONSTRAINT [FK_SSD.ServiceOffering_SSD.Provider_ProviderId] FOREIGN KEY ([ProviderId]) REFERENCES [SSD].[Provider] ([ProviderId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceOffering_SSD.ServiceType_ServiceTypeId] FOREIGN KEY ([ServiceTypeId]) REFERENCES [SSD].[ServiceType] ([ServiceTypeId]) ON DELETE CASCADE,
    CONSTRAINT [UX_ServiceOffering_ProviderId_ServiceTypeId_ProgramId] UNIQUE NONCLUSTERED ([ProviderId] ASC, [ServiceTypeId] ASC, [ProgramId] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_ProgramId]
    ON [SSD].[ServiceOffering]([ProgramId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceTypeId]
    ON [SSD].[ServiceOffering]([ServiceTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ProviderId]
    ON [SSD].[ServiceOffering]([ProviderId] ASC);

