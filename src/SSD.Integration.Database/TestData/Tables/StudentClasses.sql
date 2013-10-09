CREATE TABLE [TestData].[StudentClasses] (
    [id]         INT IDENTITY(1,1) NOT NULL,
    [StudentKey] NVARCHAR (68) NOT NULL,
    [ClassKey]   NVARCHAR (68) NOT NULL,
    CONSTRAINT [PK_StudentClasses] PRIMARY KEY CLUSTERED ([id] ASC)
);



