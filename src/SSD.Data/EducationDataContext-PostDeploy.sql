
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Student_StudentSISId')
	CREATE INDEX [IX_Student_StudentSISId] ON [SSD].[Student] (StudentSISId)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Student_Name')
	CREATE INDEX [IX_Student_Name] ON [SSD].[Student] (LastName, FirstName, MiddleName)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Student_Grade')
	CREATE INDEX [IX_Student_Grade] ON [SSD].[Student] (Grade)
GO