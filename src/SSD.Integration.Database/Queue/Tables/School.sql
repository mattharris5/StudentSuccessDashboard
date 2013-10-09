CREATE TABLE [Queue].[School] (
	[id]        [int] IDENTITY(1,1) NOT NULL,
	[SchoolKey] [nvarchar](68)      NOT NULL,
	[Name]      [nvarchar](50)      NULL,
    CONSTRAINT [PK_Queue.School] PRIMARY KEY CLUSTERED ([id] ASC)
);



