﻿create table dbo.UserFeedGroupFeed (
  Id int identity(1, 1) not null,
  UserFeedGroupId int not null,
  FeedId int not null,
  DateCreated datetime2 not null,
  CustomTitle nvarchar(256) null,
  constraint PK_UserFeedGroupFeed primary key clustered (
    Id asc
  ),
  constraint FK_UserFeedGroupFeed_UserFeedGroup foreign key (UserFeedGroupId) references dbo.UserFeedGroup (Id),
  constraint FK_UserFeedGroupFeed_Feed foreign key (FeedId) references dbo.Feed (Id)
)
go

create unique nonclustered index IX_UserFeedGroupFeed_UserFeedGroupId_FeedId on dbo.UserFeedGroupFeed
(
  UserFeedGroupId asc,
  FeedId asc
)
go
