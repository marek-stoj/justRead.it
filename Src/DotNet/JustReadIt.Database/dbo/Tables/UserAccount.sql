create table dbo.UserAccount (
  Id int identity(1, 1) not null,
  DateCreated datetime2 not null,
  EmailAddress nvarchar(256) not null,
  IsEmailAddressVerified bit not null,
  AuthProviderId nvarchar(512) null,
  PasswordHash nvarchar(32) null,
  constraint PK_UserAccount primary key clustered (
    Id asc
  )
)
go

create unique nonclustered index IX_UserAccount_EmailAddress on dbo.UserAccount
(
  EmailAddress asc
)
go
