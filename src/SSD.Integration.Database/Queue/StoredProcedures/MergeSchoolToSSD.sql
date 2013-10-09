CREATE PROCEDURE [Queue].[MergeSchoolToSSD]
	--ALTER PROCEDURE [Queue].[MergeSchoolToSSD]

AS	
BEGIN

	DECLARE @OutputTable TABLE (ActionPerformed nvarchar(10), EntityID int, EntityKey nvarchar(255), EntityName nvarchar(255))

	MERGE [SSD].[School]   as T
	USING [Queue].[School] as S
	ON (T.SchoolKey = S.SchoolKey)

	WHEN NOT MATCHED THEN
		INSERT (SchoolKey, Name)
		VALUES (S.SchoolKey, S.Name)

	WHEN MATCHED THEN
		UPDATE SET 
			T.Name = S.Name

	OUTPUT $action, Inserted.SchoolId, Inserted.SchoolKey, Inserted.Name INTO @OutputTable;

-- Log Results to ODS Log Table
	INSERT INTO [Queue].MergeLog
			(ActionPerformed, TimeInserted, BatchID, Entity,  EntityID , EntityKey , EntityName)
	SELECT   ActionPerformed, GETDATE(),      1,    'School', EntityID , 	LEFT(EntityKey, 50) , LEFT(EntityName, 50)   FROM  @OutputTable

-- Now delete records in queue
	DELETE FROM [Queue].[School]
		
END