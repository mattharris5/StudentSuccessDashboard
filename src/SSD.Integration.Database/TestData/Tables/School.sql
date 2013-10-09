CREATE TABLE [TestData].[School] (
    [id]        INT IDENTITY(1,1) NOT NULL,
    [SchoolKey] NVARCHAR (68)     NOT NULL,
    [Name]      NVARCHAR (50)     NULL,
    CONSTRAINT [PK_School] PRIMARY KEY CLUSTERED ([id] ASC)
);



