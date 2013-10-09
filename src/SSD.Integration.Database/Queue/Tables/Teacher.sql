CREATE TABLE [Queue].[Teacher] (
	[id]         [int] IDENTITY(1,1) NOT NULL,
	[TeacherKey] [nvarchar](68)      NOT NULL,
	[LastName]   [nvarchar](50)      NULL,
	[FirstName]  [nvarchar](50)      NULL,
	[MiddleName] [nvarchar](50)      NULL,
	[Phone]      [nvarchar](15)      NULL,
	[Number]     [nvarchar](36)      NULL,
	[Email]      [nvarchar](255)     NULL,
    CONSTRAINT [PK_Queue.Teacher] PRIMARY KEY CLUSTERED ([id] ASC),
);
