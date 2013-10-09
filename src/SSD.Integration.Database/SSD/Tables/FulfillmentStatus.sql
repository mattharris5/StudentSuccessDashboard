CREATE TABLE [SSD].[FulfillmentStatus] (
    [FulfillmentStatusId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SSD.FulfillmentStatus] PRIMARY KEY CLUSTERED ([FulfillmentStatusId] ASC),
    CONSTRAINT [UX_FulfillmentStatus_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

