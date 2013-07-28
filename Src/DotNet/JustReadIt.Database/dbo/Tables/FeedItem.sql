create table dbo.FeedItem (
  Id int identity(1, 1) not null,
  FeedId int not null,
  DateCreated datetime not null,
  Title nvarchar(256) not null,
  Url nvarchar(1024) not null,
  UrlChecksum as (checksum(Url)) persisted not null,
  constraint PK_FeedItem primary key clustered (
    Id asc
  ),
  constraint FK_FeedItem_Feed foreign key (FeedId) references dbo.Feed (Id)
)
