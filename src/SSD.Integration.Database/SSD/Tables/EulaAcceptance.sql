CREATE TABLE [SSD].[EulaAcceptance] (
    [EulaAcceptanceId] INT      IDENTITY (1, 1) NOT NULL,
    [EulaAgreementId]  INT      NOT NULL,
    [CreateTime]       DATETIME NOT NULL,
    [CreatingUserId]   INT      NOT NULL,
    CONSTRAINT [PK_SSD.EulaAcceptance] PRIMARY KEY CLUSTERED ([EulaAcceptanceId] ASC),
    CONSTRAINT [FK_SSD.EulaAcceptance_SSD.EulaAgreement_EulaAgreementId] FOREIGN KEY ([EulaAgreementId]) REFERENCES [SSD].[EulaAgreement] ([EulaAgreementId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.EulaAcceptance_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId])
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[EulaAcceptance]([CreatingUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EulaAgreementId]
    ON [SSD].[EulaAcceptance]([EulaAgreementId] ASC);

