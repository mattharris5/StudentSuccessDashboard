

CREATE TABLE [SSD].[Teacher] (
    [TeacherId]  INT            IDENTITY (1, 1) NOT NULL,
    [TeacherKey] NVARCHAR (68)  NULL,
    [LastName]   NVARCHAR (50)  NULL,
    [FirstName]  NVARCHAR (50)  NULL,
    [MiddleName] NVARCHAR (50)  NULL,
    [Number]     NVARCHAR (36)  NULL,
    [Phone]      NVARCHAR (15)  NULL,
    [Email]      NVARCHAR (255) NULL,
    CONSTRAINT [PK_SSD.Teacher] PRIMARY KEY CLUSTERED ([TeacherId] ASC)
);



 