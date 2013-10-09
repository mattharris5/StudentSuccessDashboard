CREATE PROCEDURE [Queue].[MergeStudentToSSD]
	--ALTER PROCEDURE [Queue].[MergeStudentToSSD]

AS	
BEGIN

	DECLARE @OutputTable TABLE (ActionPerformed nvarchar(10), EntityID int, EntityKey nvarchar(50), EntityName nvarchar(50))

	MERGE [SSD].[Student]  as T
	USING [Queue].[Student] as S
	ON (T.StudentSISId = S.StudentKey)

	WHEN NOT MATCHED THEN
		INSERT (StudentKey,     StudentSISId,   LastName,   MiddleName,   FirstName,   Grade,   DateOfBirth, Parents, HasParentalOptOut, SchoolId)
		VALUES (S.StudentKey, S.StudentSISId, S.LastName, S.MiddleName, S.FirstName, S.Grade, S.DateOfBirth, S.Parents, 0, 
				(SELECT SchoolId FROM SSD.School WHERE SchoolKey = S.SchoolKey ))

	WHEN MATCHED THEN
		UPDATE SET 
			T.StudentKey = S.StudentSISId,
			T.StudentSISId = S.StudentSISId,
			T.LastName = S.LastName,
			T.FirstName = S.FirstName,
			T.MiddleName = S.MiddleName,
			T.Grade = S.Grade,
			T.DateOfBirth = S.DateOfBirth,
			T.Parents = S.Parents

	OUTPUT $action, Inserted.StudentId, Inserted.StudentKey, Inserted.LastName + ', ' + Inserted.FirstName INTO @OutputTable;

-- Log Results to ODS Log Table
	INSERT INTO [Queue].MergeLog
			(ActionPerformed, TimeInserted, BatchID, Entity,  EntityID , EntityKey , EntityName)
	SELECT   ActionPerformed, GETDATE(),      1,    'Student', EntityID , EntityKey , EntityName  FROM  @OutputTable

-- Now delete records in queue
	DELETE FROM [Queue].[Student]	
END