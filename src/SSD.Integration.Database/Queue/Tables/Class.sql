CREATE TABLE [Queue].[Class] (
    [id]         INT IDENTITY (1, 1) NOT NULL,
    [ClassKey]   NVARCHAR (68)       NULL,
    [Name]       NVARCHAR (60)       NULL,
    [Number]     NVARCHAR (20)       NULL,
    [TeacherKey] NVARCHAR (68)       NULL,
    CONSTRAINT [PK_Queue.Class] PRIMARY KEY CLUSTERED ([id] ASC)
);



go
