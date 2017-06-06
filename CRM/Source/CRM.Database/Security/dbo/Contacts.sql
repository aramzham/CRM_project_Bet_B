CREATE TABLE [dbo].[Contacts]
(
	[ID] INT NOT NULL PRIMARY KEY identity(1,1),
	[FullName] varchar(200) not null,
	[CompanyName] varchar(150) not null,
	[Position] varchar(100) not null,
	[Country] varchar(50) not null,
	[Email] varchar(100) not null unique,
	[DateInserted] datetime2 null default getdate(),
	[Guid] uniqueidentifier null default newid(),
	[DateModified] datetime2 null
)
