CREATE TABLE [SSD].[User] (
    [UserId]           INT              IDENTITY (1, 1) NOT NULL,
    [UserKey]          NVARCHAR (255)   NOT NULL,
    [DisplayName]      NVARCHAR (50)    NOT NULL,
    [FirstName]        NVARCHAR (50)    NOT NULL,
    [LastName]         NVARCHAR (50)    NOT NULL,
    [EmailAddress]     NVARCHAR (255)   NOT NULL,
    [PendingEmail]     NVARCHAR (255)   NULL,
    [ConfirmationGuid] UNIQUEIDENTIFIER NOT NULL,
    [CreateTime]       DATETIME2 (7)    NOT NULL,
    [Comments]         NVARCHAR (MAX)   NULL,
    [Active]           BIT              NOT NULL,
    CONSTRAINT [PK_SSD.User] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [UX_User_UserKey] UNIQUE NONCLUSTERED ([UserKey] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_User_EmailAddress]
    ON [SSD].[User]([EmailAddress] ASC) WHERE ([EmailAddress]<>'Anonymous@sample.com');

