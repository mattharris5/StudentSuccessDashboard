CREATE TABLE [Queue].[StudentClasses](
	[id] [int]   IDENTITY(1,1) NOT NULL,
    [StudentKey] NVARCHAR (68) NOT NULL,
    [ClassKey]   NVARCHAR (68) NOT NULL,
 CONSTRAINT [PK_Queue.StudentSection] PRIMARY KEY CLUSTERED 
(
	[id] ASC
))
;
