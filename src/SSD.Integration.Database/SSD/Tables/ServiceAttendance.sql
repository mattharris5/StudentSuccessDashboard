CREATE TABLE [SSD].[ServiceAttendance] (
    [ServiceAttendanceId]       INT             IDENTITY (1, 1) NOT NULL,
    [StudentAssignedOfferingId] INT             NOT NULL,
    [SubjectId]                 INT             NOT NULL,
    [DateAttended]              DATETIME        NOT NULL,
    [Duration]                  DECIMAL (18, 2) NOT NULL,
    [Notes]                     NVARCHAR (MAX)  NULL,
    [CreateTime]                DATETIME2 (7)   NOT NULL,
    [CreatingUserId]            INT             NOT NULL,
    [LastModifyTime]            DATETIME        NULL,
    [LastModifyingUserId]       INT             NULL,
    CONSTRAINT [PK_SSD.ServiceAttendance] PRIMARY KEY CLUSTERED ([ServiceAttendanceId] ASC),
    CONSTRAINT [FK_SSD.ServiceAttendance_SSD.StudentAssignedOffering_StudentAssignedOfferingId] FOREIGN KEY ([StudentAssignedOfferingId]) REFERENCES [SSD].[StudentAssignedOffering] ([StudentAssignedOfferingId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceAttendance_SSD.Subject_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [SSD].[Subject] ([SubjectId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceAttendance_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId]),
    CONSTRAINT [FK_SSD.ServiceAttendance_SSD.User_LastModifyingUserId] FOREIGN KEY ([LastModifyingUserId]) REFERENCES [SSD].[User] ([UserId])
);






GO
CREATE NONCLUSTERED INDEX [IX_SubjectId]
    ON [SSD].[ServiceAttendance]([SubjectId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StudentAssignedOfferingId]
    ON [SSD].[ServiceAttendance]([StudentAssignedOfferingId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastModifyingUserId]
    ON [SSD].[ServiceAttendance]([LastModifyingUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[ServiceAttendance]([CreatingUserId] ASC);

