create table dbo.UserFeedGroup (
  Id int identity(1, 1) not null,
  UserAccountId int not null,
  SpecialType nvarchar(64) null,
  Title nvarchar(256) not null,
  constraint PK_UserFeedGroup primary key clustered (
    Id asc
  ),
  constraint FK_UserFeedGroup_UserAccount foreign key (UserAccountId) references dbo.UserAccount (Id)
)
