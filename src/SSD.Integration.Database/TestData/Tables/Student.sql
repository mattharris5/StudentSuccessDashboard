CREATE TABLE [TestData].[Student] (
    [id]           INT            IDENTITY (1, 1) NOT NULL,
    [StudentKey]   NVARCHAR (68)  NOT NULL,
    [SchoolKey]    NVARCHAR (68)  NOT NULL,
    [StudentSISId] NVARCHAR (36)  NULL,
    [LastName]     NVARCHAR (50)  NULL,
    [MiddleName]   NVARCHAR (50)  NULL,
    [FirstName]    NVARCHAR (50)  NULL,
    [Grade]        INT            NOT NULL,
    [DateOfBirth]  DATETIME       NULL,
    [Parents]      NVARCHAR (200) NULL,
    CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED ([id] ASC)
);



