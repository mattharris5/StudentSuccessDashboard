

CREATE TABLE [SSD].[Student] (
    [StudentId]         INT            IDENTITY (1, 1) NOT NULL,
    [StudentKey]        NVARCHAR (68)  NULL,
    [StudentSISId]      NVARCHAR (36)  NULL,
    [LastName]          NVARCHAR (50)  NULL,
    [MiddleName]        NVARCHAR (50)  NULL,
    [FirstName]         NVARCHAR (50)  NULL,
    [Grade]             INT            NOT NULL,
    [DateOfBirth]       DATETIME       NULL,
    [Parents]           NVARCHAR (200) NULL,
    [HasParentalOptOut] BIT            NOT NULL,
    [SchoolId]          INT            NOT NULL,
    CONSTRAINT [PK_SSD.Student] PRIMARY KEY CLUSTERED ([StudentId] ASC),
    CONSTRAINT [FK_SSD.Student_SSD.School_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [SSD].[School] ([SchoolId]) ON DELETE CASCADE
);

 
GO
CREATE NONCLUSTERED INDEX [IX_Student_StudentSISId]
    ON [SSD].[Student]([StudentSISId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Student_Name]
    ON [SSD].[Student]([LastName] ASC, [FirstName] ASC, [MiddleName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Student_Grade]
    ON [SSD].[Student]([Grade] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SchoolId]
    ON [SSD].[Student]([SchoolId] ASC);

