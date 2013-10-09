CREATE TABLE [SSD].[PrivateHealthInfoViewEvent] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [CreateTime]     DATETIME2 (7) NOT NULL,
    [CreatingUserId] INT           NOT NULL,
    CONSTRAINT [PK_SSD.PrivateHealthInfoViewEvent] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SSD.PrivateHealthInfoViewEvent_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId])
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[PrivateHealthInfoViewEvent]([CreatingUserId] ASC);

