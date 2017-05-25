CREATE TABLE [dbo].[Templates]
(
	[ID] INT NOT NULL PRIMARY KEY identity(1,1),
	[TemplateName] varchar(100) not null,
	[PathToFile] varchar(max) not null
)