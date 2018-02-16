CREATE TABLE [dbo].[Alarms]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[AlarmType] nvarchar(64) NOT NULL,
	[DataUsed] int NULL
)
