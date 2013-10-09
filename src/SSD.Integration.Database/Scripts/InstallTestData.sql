/* Install script for SSD Integration Test tables */

USE [SSD]
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;
GO

SET NUMERIC_ROUNDABORT OFF;
GO


CREATE SCHEMA [TestData]
    AUTHORIZATION [dbo];


GO
PRINT N'Creating [TestData].[Class]...';


GO
CREATE TABLE [TestData].[Class] (
    [id]         INT IDENTITY (1, 1) NOT NULL,
    [ClassKey]   NVARCHAR (68)       NULL,
    [Name]       NVARCHAR (60)       NULL,
    [Number]     NVARCHAR (20)       NULL,
    [TeacherKey] NVARCHAR (68)       NULL,
    CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
PRINT N'Creating [TestData].[School]...';


GO
CREATE TABLE [TestData].[School] (
    [id]        INT IDENTITY(1,1) NOT NULL,
    [SchoolKey] NVARCHAR (68)     NOT NULL,
    [Name]      NVARCHAR (50)     NULL,
    CONSTRAINT [PK_School] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
PRINT N'Creating [TestData].[Student]...';


GO
CREATE TABLE [TestData].[Student] (
    [id]           INT IDENTITY (1, 1) NOT NULL,
    [StudentKey]   NVARCHAR (68)       NOT NULL,
    [SchoolKey]    NVARCHAR (68)       NOT NULL,
    [StudentSISId] NVARCHAR (36)       NULL,
    [LastName]     NVARCHAR (50)       NULL,
    [MiddleName]   NVARCHAR (50)       NULL,
    [FirstName]    NVARCHAR (50)       NULL,
    [Grade]        INT                 NOT NULL,
    [DateOfBirth]  DATETIME            NULL,
    [Parents]      NVARCHAR (200)      NULL,
    CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
PRINT N'Creating [TestData].[StudentClasses]...';


GO
CREATE TABLE [TestData].[StudentClasses] (
    [id]         INT IDENTITY(1,1) NOT NULL,
    [StudentKey] NVARCHAR (68)     NOT NULL,
    [ClassKey]   NVARCHAR (68)     NOT NULL,
    CONSTRAINT [PK_StudentClasses] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
PRINT N'Creating [TestData].[Teacher]...';


GO
CREATE TABLE [TestData].[Teacher] (
    [id]         INT IDENTITY(1,1) NOT NULL,
    [TeacherKey] NVARCHAR (68)     NOT NULL,
    [LastName]   NVARCHAR (50)     NULL,
    [FirstName]  NVARCHAR (50)     NULL,
    [MiddleName] NVARCHAR (50)     NULL,
    [Phone]      NVARCHAR (15)     NULL,
    [Number]     NVARCHAR (36)     NULL,
    [Email]      NVARCHAR (255)    NULL,
    CONSTRAINT [PK_Teacher] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
PRINT N'Creating [TestData].[CountRecords]...';


GO
CREATE PROCEDURE [TestData].[CountRecords]
	--ALTER PROCEDURE [TestData].[CountRecords]

AS
BEGIN

	Select 'Items in TestData' as 'Place'
	     , (Select count(*) from [TestData].[Student]) as 'Students'
		 , (Select count(*) from [TestData].[School]) as 'Schools'
		 , (Select count(*) from [TestData].[Teacher]) as 'Teacher'
		 , (Select count(*) from [TestData].[Class]) as 'Class'
		 , (Select count(*) from [TestData].[StudentClasses]) as 'Student Classes'

END
GO
PRINT N'Creating [SSD].[CountRecords]...';


GO
CREATE PROCEDURE [TestData].[MoveDataToQueues]
	--ALTER PROCEDURE [TestData].[MoveDataToQueues]

AS
BEGIN

	INSERT INTO [Queue].[School] (Name, SchoolKey)
	SELECT Name, SchoolKey
	FROM [TestData].[School]

	INSERT INTO [Queue].[Teacher] (TeacherKey, LastName, FirstName, MiddleName, Phone, Email)
	SELECT TeacherKey, LastName, FirstName, MiddleName, Phone, Email
	FROM [TestData].[Teacher]

	INSERT INTO [Queue].[Student] (StudentKey, LastName, FirstName, MiddleName, Parents, StudentSISId, SchoolKey, DateOfBirth, Grade)
	SELECT StudentKey, LastName, FirstName, MiddleName, Parents, StudentSISId, SchoolKey, DateOfBirth, Grade
	FROM [TestData].[Student]

	INSERT INTO [Queue].[Class] (ClassKey, TeacherKey, Name, Number)
	SELECT ClassKey, TeacherKey, Name, Number
	FROM [TestData].[Class]

	INSERT INTO [Queue].[StudentClasses] (StudentKey, ClassKey)
	SELECT StudentKey, ClassKey
	FROM [TestData].[StudentClasses]

END
GO
PRINT N'Update complete.';


GO
