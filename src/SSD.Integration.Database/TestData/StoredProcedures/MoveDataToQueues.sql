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