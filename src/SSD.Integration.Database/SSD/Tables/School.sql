

CREATE TABLE [SSD].[School](
	[SchoolId]  [int] IDENTITY(1,1) NOT NULL,
	[SchoolKey] [nvarchar](68)      NULL,
	[Name]      [nvarchar](50)      NULL,
 CONSTRAINT [PK_SSD.School] PRIMARY KEY CLUSTERED 
	(
		[SchoolId] ASC
	)
) 