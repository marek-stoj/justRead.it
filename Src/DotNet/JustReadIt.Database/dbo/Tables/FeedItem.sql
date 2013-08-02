create table dbo.FeedItem (
  Id int identity(1, 1) not null,
  FeedId int not null,
  DateCreated datetime2 not null,
  Title nvarchar(256) null,
  Url nvarchar(1024) not null,
  UrlChecksum as (checksum(Url)) persisted not null,
  DatePublished datetime2 null,
  Author nvarchar(256) null,
  Summary ntext null,
  Content ntext null,
  constraint PK_FeedItem primary key clustered (
    Id asc
  ),
  constraint FK_FeedItem_Feed foreign key (FeedId) references dbo.Feed (Id)
)
