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