/* Install script for SSD Integration */

USE [SSD]
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;
GO

SET NUMERIC_ROUNDABORT OFF;
GO


CREATE SCHEMA [Queue]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [Queue].[School]...';

GO
CREATE TABLE [Queue].[School] (
    [id]        INT           IDENTITY (1, 1) NOT NULL,
    [SchoolKey] NVARCHAR (68) NOT NULL,
    [Name]      NVARCHAR (50) NULL,
    CONSTRAINT [PK_Queue.School] PRIMARY KEY CLUSTERED ([id] ASC)
);

GO
PRINT N'Creating [Queue].[Student]...';


GO
CREATE TABLE [Queue].[Student] (
    [id]           INT            IDENTITY (1, 1) NOT NULL,
    [StudentKey]   NVARCHAR (68)  NOT NULL,
    [SchoolKey]    NVARCHAR (68)  NOT NULL,
    [StudentSISId] NVARCHAR (36)  NULL,
    [LastName]     NVARCHAR (50)  NULL,
    [MiddleName]   NVARCHAR (50)  NULL,
    [FirstName]    NVARCHAR (50)  NULL,
    [Grade]        INT            NOT NULL,
    [DateOfBirth]  DATETIME       NULL,
    [Parents]      NVARCHAR (200) NULL,
    CONSTRAINT [PK_Queue.Student] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
PRINT N'Creating [Queue].[Teacher]...';


GO
CREATE TABLE [Queue].[Teacher] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [TeacherKey] NVARCHAR (68)  NOT NULL,
    [LastName]   NVARCHAR (50)  NULL,
    [FirstName]  NVARCHAR (50)  NULL,
    [MiddleName] NVARCHAR (50)  NULL,
    [Phone]      NVARCHAR (15)  NULL,
    [Number]     NVARCHAR (36)  NULL,
    [Email]      NVARCHAR (255) NULL,
    CONSTRAINT [PK_Queue.Teacher] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
PRINT N'Creating [Queue].[MergeLog]...';


GO
CREATE TABLE [Queue].[MergeLog] (
    [ID]              INT           IDENTITY (1, 1) NOT NULL,
    [ActionPerformed] NVARCHAR (10) NOT NULL,
    [TimeInserted]    DATETIME      NOT NULL,
    [BatchID]         INT           NULL,
    [Entity]          NVARCHAR (50) NULL,
    [EntityID]        INT           NULL,
    [EntityKey]       NVARCHAR (50) NULL,
    [EntityName]      NVARCHAR (50) NULL,
    CONSTRAINT [PK_MergeLog] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [Queue].[Class]...';


GO
CREATE TABLE [Queue].[Class] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [ClassKey]   NVARCHAR (68) NULL,
    [Name]       NVARCHAR (60) NULL,
    [Number]     NVARCHAR (20) NULL,
    [TeacherKey] NVARCHAR (68) NULL,
    CONSTRAINT [PK_Queue.Class] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
PRINT N'Creating [Queue].[StudentClasses]...';


GO
CREATE TABLE [Queue].[StudentClasses] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [StudentKey] NVARCHAR (68) NOT NULL,
    [ClassKey]   NVARCHAR (68) NOT NULL,
    CONSTRAINT [PK_Queue.StudentSection] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
PRINT N'Creating [Queue].[MergeSchool]...';


GO
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
GO
PRINT N'Creating [Queue].[MergeStudentToSSD]...';


GO
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
GO
PRINT N'Creating [Queue].[MergeClassToSSD]...';


GO
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
GO
PRINT N'Creating [Queue].[MergeStudentClassesToSSD]...';


GO
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

	OUTPUT $action, Inserted.ClassId, Inserted.StudentID, 'None' INTO @OutputTable;

-- Log Results to ODS Log Table
	INSERT INTO [Queue].MergeLog
			(ActionPerformed, TimeInserted, BatchID, Entity,  EntityID , EntityKey , EntityName)
	SELECT   ActionPerformed, GETDATE(),      1,    'StudentClass', EntityID , EntityKey , EntityName  FROM  @OutputTable

-- Now delete records in queue
	DELETE FROM [Queue].[StudentClasses]

	
END
GO
PRINT N'Creating [Queue].[MergeTeacherToSSD]...';


GO
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
GO
PRINT N'Creating [Queue].[ProcessQueues]...';


GO
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
GO
PRINT N'Creating [Queue].[CountRecords]...';


GO
CREATE PROCEDURE [Queue].[CountRecords]
	--ALTER PROCEDURE [Queue].[CountRecords]

AS
BEGIN

	Select 'Items in Queues' as 'Place'
		 , (Select count(*) from [Queue].[Student]) as 'Students'
		 , (Select count(*) from [Queue].[School]) as 'Schools'
		 , (Select count(*) from [Queue].[Teacher]) as 'Teacher'
		 , (Select count(*) from [Queue].[Class]) as 'Course'
		 , (Select count(*) from [Queue].[StudentClasses]) as 'Student Classes'

END
GO
PRINT N'Creating [SSD].[CountRecords]...';


GO
CREATE PROCEDURE [SSD].[CountRecords]
	--ALTER PROCEDURE [SSD].[CountRecords]

AS
BEGIN

	Select 'Items in SSD' as 'Place'
	     , (Select count(*) from [SSD].[Student]) as 'Students'
		 , (Select count(*) from [SSD].[School]) as 'Schools'
		 , (Select count(*) from [SSD].[Teacher]) as 'Teacher'
		 , (Select count(*) from [SSD].[Class]) as 'Class'
		 , (Select count(*) from [SSD].[StudentClasses]) as 'Student Classes'

END
GO
PRINT N'Update complete.';


GO
