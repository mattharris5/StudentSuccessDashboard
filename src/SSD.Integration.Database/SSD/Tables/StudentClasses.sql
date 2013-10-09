


CREATE TABLE [SSD].[StudentClasses] (
    [StudentId] INT NOT NULL,
    [ClassId]   INT NOT NULL,
    CONSTRAINT [PK_SSD.StudentClasses] PRIMARY KEY CLUSTERED ([StudentId] ASC, [ClassId] ASC),
    CONSTRAINT [FK_SSD.StudentClasses_SSD.Class_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [SSD].[Class] ([ClassId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSD.StudentClasses_SSD.Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [SSD].[Student] ([StudentId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_StudentId]
    ON [SSD].[StudentClasses]([StudentId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ClassId]
    ON [SSD].[StudentClasses]([ClassId] ASC);

