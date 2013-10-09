CREATE TABLE [SSD].[LoginEvent] (
    [LoginEventId]   INT           IDENTITY (1, 1) NOT NULL,
    [CreateTime]     DATETIME2 (7) NOT NULL,
    [CreatingUserId] INT           NOT NULL,
    CONSTRAINT [PK_SSD.LoginEvent] PRIMARY KEY CLUSTERED ([LoginEventId] ASC),
    CONSTRAINT [FK_SSD.LoginEvent_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[LoginEvent]([CreatingUserId] ASC);

