CREATE TABLE [dbo].[ContactsMailingLists]
(
	[ContactID] int not null foreign key references Contacts(ID) on delete cascade,
	[MailingListID] int not null foreign key references MailingLists(ID) on delete cascade,
	CONSTRAINT [PK_dbo.EmailListContacts] PRIMARY KEY CLUSTERED ([MailingListID] ASC, [ContactID] ASC),
)
