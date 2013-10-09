CREATE TABLE [Queue].[MergeLog]
(
	[ID]				INT  IDENTITY (1, 1)	NOT NULL,
	[ActionPerformed]   NVARCHAR (10)			NOT NULL,
	[TimeInserted]		DATETIME				NOT NULL,
	[BatchID]			INT						NULL,
	[Entity]			NVARCHAR (50)			NULL,
	[EntityID]			INT						NULL,
	[EntityKey]			NVARCHAR (50)			NULL,
	[EntityName]		NVARCHAR (50)			NULL,
    CONSTRAINT [PK_MergeLog] PRIMARY KEY CLUSTERED ([ID] ASC)
)
