CREATE PROCEDURE [Queue].[ProcessQueues]
--ALTER PROCEDURE [Queue].[ProcessQueues]
	
AS
BEGIN

	-- Process Schools
	EXECUTE [Queue].MergeSchoolToSSD

	-- Process Teacher
	EXECUTE [Queue].MergeTeacherToSSD

	-- Process Students
	EXECUTE [Queue].MergeStudentToSSD

	-- Process Students
	EXECUTE [Queue].MergeClassToSSD

	-- Process Student Classes
	EXECUTE [Queue].MergeStudentClassesToSSD

END