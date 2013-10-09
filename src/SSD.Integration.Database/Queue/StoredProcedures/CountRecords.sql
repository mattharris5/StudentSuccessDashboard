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