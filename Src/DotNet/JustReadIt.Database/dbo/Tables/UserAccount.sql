create table dbo.UserAccount (
  Id int identity(1, 1) not null,
  DateCreated datetime not null,
  EmailAddress nvarchar(256) not null,
  PasswordHash nvarchar(32) not null,
  IsEmailAddressVerified bit not null,
  constraint PK_UserAccount primary key clustered (
    Id asc
  )
)
