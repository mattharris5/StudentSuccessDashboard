CREATE PROCEDURE [Queue].[MergeTeacherToSSD]
	--ALTER PROCEDURE [Queue].[MergeTeacherToSSD]

AS	
BEGIN

	DECLARE @OutputTable TABLE (ActionPerformed nvarchar(10), EntityID int, EntityKey nvarchar(50), EntityName nvarchar(50))

	MERGE [SSD].[Teacher]  as T
	USING [Queue].[Teacher] as S
	ON (T.TeacherKey = S.TeacherKey)

	WHEN NOT MATCHED THEN
		INSERT (TeacherKey, LastName, FirstName, MiddleName, Phone, Email)
		VALUES (S.TeacherKey, S.LastName, S.FirstName, S.MiddleName, S.Phone, S.Email)

	WHEN MATCHED THEN
		UPDATE SET 
			T.TeacherKey = S.TeacherKey,
			T.LastName = S.LastName,
			T.FirstName = S.FirstName,
			T.MiddleName = S.MiddleName,
			T.Phone = S.Phone,
			T.Email = S.Email

	OUTPUT $action, Inserted.TeacherId, Inserted.TeacherKey, Inserted.LastName + ', ' + Inserted.FirstName INTO @OutputTable;

-- Log Results to ODS Log Table
	INSERT INTO [Queue].MergeLog
			(ActionPerformed, TimeInserted, BatchID, Entity,  EntityID , EntityKey , EntityName)
	SELECT   ActionPerformed, GETDATE(),      1,    'Teacher', EntityID , EntityKey , EntityName  FROM  @OutputTable


-- Now delete records in queue
	DELETE FROM [Queue].[Teacher]	
	
END