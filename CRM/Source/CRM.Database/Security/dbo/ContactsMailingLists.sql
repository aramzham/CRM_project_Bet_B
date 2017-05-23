CREATE TABLE [dbo].[ContactsMailingLists]
(
	[ContactID] int not null foreign key references Contacts(ContactID) on delete cascade,
	[MailingListID] int not null foreign key references MailingLists(MailingListID) on delete cascade
)
