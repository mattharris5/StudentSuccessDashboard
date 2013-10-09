CREATE TABLE [TestData].[Teacher] (
    [id]         INT IDENTITY(1,1) NOT NULL,
    [TeacherKey] NVARCHAR (68)     NOT NULL,
    [LastName]   NVARCHAR (50)     NULL,
    [FirstName]  NVARCHAR (50)     NULL,
    [MiddleName] NVARCHAR (50)     NULL,
    [Phone]      NVARCHAR (15)     NULL,
    [Number]     NVARCHAR (36)     NULL,
    [Email]      NVARCHAR (255)    NULL,
    CONSTRAINT [PK_Teacher] PRIMARY KEY CLUSTERED ([id] ASC)
);



