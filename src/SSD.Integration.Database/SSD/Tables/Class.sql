

CREATE TABLE [SSD].[Class] (
    [ClassId]   INT           IDENTITY (1, 1) NOT NULL,
    [ClassKey]  NVARCHAR (68) NULL,
    [Name]      NVARCHAR (60) NULL,
    [Number]    NVARCHAR (20) NULL,
    [TeacherId] INT           NOT NULL,
    CONSTRAINT [PK_SSD.Class] PRIMARY KEY CLUSTERED ([ClassId] ASC),
    CONSTRAINT [FK_SSD.Class_SSD.Teacher_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [SSD].[Teacher] ([TeacherId]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_TeacherId]
    ON [SSD].[Class]([TeacherId] ASC);

