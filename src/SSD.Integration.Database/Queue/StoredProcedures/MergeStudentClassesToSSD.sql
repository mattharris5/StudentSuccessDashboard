CREATE PROCEDURE [Queue].[MergeStudentClassesToSSD]
	--ALTER PROCEDURE [Queue].[MergeStudentClassesToSSD]

AS	
BEGIN

	DECLARE @OutputTable TABLE (ActionPerformed nvarchar(10), EntityID int, EntityKey nvarchar(50), EntityName nvarchar(50))

	MERGE [SSD].[StudentClasses]  as T
	USING [Queue].[StudentClasses] as S
	ON (T.ClassId	=		(SELECT ClassId from SSD.Class Where ClassKey = S.ClassKey)
		AND T.StudentId = (SELECT StudentId from SSD.Student Where StudentKey = S.StudentKey))

	WHEN NOT MATCHED THEN
		INSERT (ClassId,StudentId)
		VALUES ((SELECT ClassId from SSD.Class Where ClassKey = S.ClassKey), 
					(SELECT StudentId from SSD.Student Where StudentKey = S.StudentKey))

-- Note - No need to WHEN MATCHED (update), since only fields are ones merging. Either exist or not.

	OUTPUT $action, Inserted.ClassId, Inserted.StudentId, 'None' INTO @OutputTable;

-- Log Results to ODS Log Table
	INSERT INTO [Queue].MergeLog
			(ActionPerformed, TimeInserted, BatchID, Entity,  EntityID , EntityKey , EntityName)
	SELECT   ActionPerformed, GETDATE(),      1,    'StudentClass', EntityID , EntityKey , EntityName  FROM  @OutputTable

-- Now delete records in queue
	DELETE FROM [Queue].[StudentClasses]
	
END