create table dbo.EmailVerificationToken (
  Id int identity(1, 1) not null,
  UserAccountId int not null,
  DateCreated datetime2 not null,
  Token uniqueidentifier not null,
  IsUsed bit not null,
  constraint PK_EmailVerificationToken primary key clustered (
    Id asc
  ),
  constraint FK_EmailVerificationToken_UserAccountId foreign key (UserAccountId) references dbo.UserAccount (Id)
)
