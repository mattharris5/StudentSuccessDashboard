CREATE TABLE [SSD].[ServiceRequest] (
    [ServiceRequestId]    INT             IDENTITY (1, 1) NOT NULL,
    [Notes]               NVARCHAR (1000) NULL,
    [ServiceTypeId]       INT             NOT NULL,
    [StudentId]           INT             NOT NULL,
    [PriorityId]          INT             NOT NULL,
    [SubjectId]           INT             NOT NULL,
    [CreateTime]          DATETIME2 (7)   NOT NULL,
    [CreatingUserId]      INT             NOT NULL,
    [LastModifyTime]      DATETIME        NULL,
    [LastModifyingUserId] INT             NULL,
    CONSTRAINT [PK_SSD.ServiceRequest] PRIMARY KEY CLUSTERED ([ServiceRequestId] ASC),
    CONSTRAINT [FK_SSD.ServiceRequest_SSD.Priority_PriorityId] FOREIGN KEY ([PriorityId]) REFERENCES [SSD].[Priority] ([PriorityId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceRequest_SSD.ServiceType_ServiceTypeId] FOREIGN KEY ([ServiceTypeId]) REFERENCES [SSD].[ServiceType] ([ServiceTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceRequest_SSD.Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [SSD].[Student] ([StudentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceRequest_SSD.Subject_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [SSD].[Subject] ([SubjectId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceRequest_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceRequest_SSD.User_LastModifyingUserId] FOREIGN KEY ([LastModifyingUserId]) REFERENCES [SSD].[User] ([UserId])
);




GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[ServiceRequest]([CreatingUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SubjectId]
    ON [SSD].[ServiceRequest]([SubjectId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PriorityId]
    ON [SSD].[ServiceRequest]([PriorityId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StudentId]
    ON [SSD].[ServiceRequest]([StudentId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceTypeId]
    ON [SSD].[ServiceRequest]([ServiceTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastModifyingUserId]
    ON [SSD].[ServiceRequest]([LastModifyingUserId] ASC);

