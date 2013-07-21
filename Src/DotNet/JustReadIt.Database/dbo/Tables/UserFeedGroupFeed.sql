﻿create table dbo.UserFeedGroupFeed (
  Id int identity(1, 1) not null,
  UserFeedGroupId int not null,
  FeedId int not null,
  constraint PK_UserFeedGroupFeed primary key clustered (
    Id asc
  ),
  constraint FK_UserFeedGroupFeed_UserFeedGroup foreign key (UserFeedGroupId) references dbo.UserFeedGroup (Id),
  constraint FK_UserFeedGroupFeed_Feed foreign key (FeedId) references dbo.Feed (Id)
)
