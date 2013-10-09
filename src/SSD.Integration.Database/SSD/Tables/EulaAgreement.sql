CREATE TABLE [SSD].[EulaAgreement] (
    [EulaAgreementId] INT            IDENTITY (1, 1) NOT NULL,
    [EulaText]        NVARCHAR (MAX) NOT NULL,
    [CreateTime]      DATETIME       NOT NULL,
    [CreatingUserId]  INT            NOT NULL,
    CONSTRAINT [PK_SSD.EulaAgreement] PRIMARY KEY CLUSTERED ([EulaAgreementId] ASC),
    CONSTRAINT [FK_SSD.EulaAgreement_SSD.User_CreatingUserId] FOREIGN KEY ([CreatingUserId]) REFERENCES [SSD].[User] ([UserId])
);


GO
CREATE NONCLUSTERED INDEX [IX_CreatingUserId]
    ON [SSD].[EulaAgreement]([CreatingUserId] ASC);

