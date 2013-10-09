CREATE TABLE [SSD].[StudentAssignedOffering] (
    [StudentAssignedOfferingId] INT            IDENTITY (1, 1) NOT NULL,
    [IsActive]                  BIT            NOT NULL,
    [ServiceOfferingId]         INT            NOT NULL,
    [StudentId]                 INT            NOT NULL,
    [CreateTime]                DATETIME2 (7)  NOT NULL,
    [StartDate]                 DATETIME2 (7)  NULL,
    [EndDate]                   DATETIME2 (7)  NULL,
    [Notes]                     NVARCHAR (MAX) NULL,
    [CreatingUserId]            INT            NOT NULL,
    [LastModifyTime]            DATETIME       NULL,
    [LastModifyingUserId]       INT            NULL,
    CONSTRAINT [PK_SSD.StudentAssignedOffering] PRIMARY KEY CLUSTERED ([StudentAssignedOfferingId] ASC),
    CONSTRAINT [FK_SSD.StudentAssignedOffering_SSD.ServiceOffering_ServiceOfferingId] FOREIGN KEY ([ServiceOfferingId]) REFERENCES [SSD].[ServiceOffering] ([ServiceOfferingId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.StudentAssignedOffering_SSD.Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [SSD].[Student] ([StudentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.StudentAssignedOffering_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.StudentAssignedOffering_SSD.User_LastModifyingUserId] FOREIGN KEY ([LastModifyingUserId]) REFERENCES [SSD].[User] ([UserId])
);




GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[StudentAssignedOffering]([CreatingUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StudentId]
    ON [SSD].[StudentAssignedOffering]([StudentId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceOfferingId]
    ON [SSD].[StudentAssignedOffering]([ServiceOfferingId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastModifyingUserId]
    ON [SSD].[StudentAssignedOffering]([LastModifyingUserId] ASC);

