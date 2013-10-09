CREATE TABLE [SSD].[UserAccessChangeEvent] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [UserId]         INT           NOT NULL,
    [CreateTime]     DATETIME2 (7) NOT NULL,
    [CreatingUserId] INT           NOT NULL,
    [UserActive]     BIT           NOT NULL,
    [AccessData]     XML           NULL,
    CONSTRAINT [PK_SSD.UserAccessChangeEvent] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SSD.UserAccessChangeEvent_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId]),
    CONSTRAINT [FK_SSD.UserAccessChangeEvent_SSD.User_UserId] FOREIGN KEY ([UserId]) REFERENCES [SSD].[User] ([UserId])
);


GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[UserAccessChangeEvent]([CreatingUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [SSD].[UserAccessChangeEvent]([UserId] ASC);

