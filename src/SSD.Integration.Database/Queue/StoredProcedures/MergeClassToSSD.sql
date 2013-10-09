CREATE PROCEDURE [Queue].[MergeClassToSSD]
	--ALTER PROCEDURE [Queue].[MergeClassToSSD]

AS
BEGIN

	DECLARE @OutputTable TABLE (ActionPerformed nvarchar(10), EntityID int, EntityKey nvarchar(50), EntityName nvarchar(50))

	MERGE [SSD].[Class]  as T
		USING [Queue].[Class] as S
		ON (T.ClassKey = S.ClassKey)

	WHEN NOT MATCHED THEN
		INSERT (ClassKey, Name, Number, TeacherId)
		VALUES (S.ClassKey, S.Name, S.Number, (Select TeacherId FROM [SSD].Teacher Where TeacherKey = S.TeacherKey))

	WHEN MATCHED THEN
		UPDATE SET 
			T.ClassKey = S.ClassKey,
			T.Name = S.Name,
			T.Number = S.Number,
 			T.TeacherId = (Select TeacherId FROM [SSD].Teacher Where TeacherKey = S.TeacherKey)

	OUTPUT $action, Inserted.ClassId, Inserted.ClassKey, Inserted.Name INTO @OutputTable;

-- Log Results to ODS Log Table
	INSERT INTO [Queue].MergeLog
			(ActionPerformed, TimeInserted, BatchID, Entity,  EntityID , EntityKey , EntityName)
	SELECT   ActionPerformed, GETDATE(),      1,    'Class', EntityID , EntityKey , EntityName  FROM  @OutputTable

-- Now delete records in queue
	DELETE FROM [Queue].[Class]
		
END