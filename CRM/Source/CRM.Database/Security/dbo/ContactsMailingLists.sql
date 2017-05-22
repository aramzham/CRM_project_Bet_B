CREATE TABLE [dbo].[ContactsMailingLists]
(
	[ContactID] int not null foreign key references Contacts(ContactID),
	[MailingListID] int not null foreign key references MailingLists(MailingListID)
)
