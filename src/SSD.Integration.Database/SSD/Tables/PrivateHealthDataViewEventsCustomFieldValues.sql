CREATE TABLE [SSD].[PrivateHealthDataViewEventsCustomFieldValues] (
    [PrivateHealthDataViewEventId] INT NOT NULL,
    [CustomFieldValueId]           INT NOT NULL,
    CONSTRAINT [PK_SSD.PrivateHealthDataViewEventsCustomFieldValues] PRIMARY KEY CLUSTERED ([PrivateHealthDataViewEventId] ASC, [CustomFieldValueId] ASC),
    CONSTRAINT [FK_SSD.PrivateHealthDataViewEventsCustomFieldValues_SSD.CustomFieldValue_CustomFieldValueId] FOREIGN KEY ([CustomFieldValueId]) REFERENCES [SSD].[CustomFieldValue] ([CustomFieldValueId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.PrivateHealthDataViewEventsCustomFieldValues_SSD.PrivateHealthInfoViewEvent_PrivateHealthDataViewEventId] FOREIGN KEY ([PrivateHealthDataViewEventId]) REFERENCES [SSD].[PrivateHealthInfoViewEvent] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomFieldValueId]
    ON [SSD].[PrivateHealthDataViewEventsCustomFieldValues]([CustomFieldValueId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PrivateHealthDataViewEventId]
    ON [SSD].[PrivateHealthDataViewEventsCustomFieldValues]([PrivateHealthDataViewEventId] ASC);

