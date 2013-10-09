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