create table dbo.UserStarredFeedItem (
  Id int identity(1, 1) not null,
  UserAccountId int not null,
  FeedItemId int not null,
  DateCreated datetime2 not null,
  constraint PK_UserStarredFeedItem primary key clustered (
    Id asc
  ),
  constraint FK_UserStarredFeedItem_UserAccount foreign key (UserAccountId) references UserAccount (Id),
  constraint FK_UserStarredFeedItem_FeedItem foreign key (FeedItemId) references FeedItem (Id),
)
go

create unique nonclustered index IX_UserStarredFeedItem_UserAccountId_FeedItemId on dbo.UserStarredFeedItem
(
  UserAccountId asc,
  FeedItemId asc
)
go
