CREATE TABLE [SSD].[CustomDataOrigin] (
    [CustomDataOriginId] INT            IDENTITY (1, 1) NOT NULL,
    [FileName]           NVARCHAR (MAX) NULL,
    [WasManualEntry]     BIT            NOT NULL,
    [Source]             NVARCHAR (50)  NULL,
    [CreateTime]         DATETIME2 (7)  NOT NULL,
    [CreatingUserId]     INT            NOT NULL,
    [AzureBlobKey]       NVARCHAR (255) NULL,
    CONSTRAINT [PK_SSD.CustomDataOrigin] PRIMARY KEY CLUSTERED ([CustomDataOriginId] ASC),
    CONSTRAINT [FK_SSD.CustomDataOrigin_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId])
);




GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[CustomDataOrigin]([CreatingUserId] ASC);

