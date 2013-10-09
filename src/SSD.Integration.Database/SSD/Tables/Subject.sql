CREATE TABLE [SSD].[Subject] (
    [SubjectId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_SSD.Subject] PRIMARY KEY CLUSTERED ([SubjectId] ASC),
    CONSTRAINT [UX_Subject_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

