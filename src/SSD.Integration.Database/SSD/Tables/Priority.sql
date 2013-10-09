CREATE TABLE [SSD].[Priority] (
    [PriorityId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SSD.Priority] PRIMARY KEY CLUSTERED ([PriorityId] ASC),
    CONSTRAINT [UX_Priority_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

