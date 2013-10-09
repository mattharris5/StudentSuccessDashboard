CREATE TABLE [SSD].[ServiceRequestFulfillment] (
    [ServiceRequestFulfillmentId] INT             IDENTITY (1, 1) NOT NULL,
    [ServiceRequestId]            INT             NOT NULL,
    [FulfillmentStatusId]         INT             NOT NULL,
    [FulfilledById]               INT             NULL,
    [Notes]                       NVARCHAR (1000) NULL,
    [CreateTime]                  DATETIME2 (7)   NOT NULL,
    [CreatingUserId]              INT             NOT NULL,
    CONSTRAINT [PK_SSD.ServiceRequestFulfillment] PRIMARY KEY CLUSTERED ([ServiceRequestFulfillmentId] ASC),
    CONSTRAINT [FK_SSD.ServiceRequestFulfillment_SSD.FulfillmentStatus_FulfillmentStatusId] FOREIGN KEY ([FulfillmentStatusId]) REFERENCES [SSD].[FulfillmentStatus] ([FulfillmentStatusId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceRequestFulfillment_SSD.ServiceRequest_ServiceRequestId] FOREIGN KEY ([ServiceRequestId]) REFERENCES [SSD].[ServiceRequest] ([ServiceRequestId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.ServiceRequestFulfillment_SSD.StudentAssignedOffering_FulfilledById] FOREIGN KEY ([FulfilledById]) REFERENCES [SSD].[StudentAssignedOffering] ([StudentAssignedOfferingId]),
    CONSTRAINT [FK_SSD.ServiceRequestFulfillment_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId])
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_FulfilledById]
    ON [SSD].[ServiceRequestFulfillment]([FulfilledById] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FulfillmentStatusId]
    ON [SSD].[ServiceRequestFulfillment]([FulfillmentStatusId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceRequestId]
    ON [SSD].[ServiceRequestFulfillment]([ServiceRequestId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[ServiceRequestFulfillment]([CreatingUserId] ASC);

