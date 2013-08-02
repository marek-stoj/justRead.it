create table dbo.Feed (
  Id int identity(1, 1) not null,
  DateCreated datetime2 not null,
  Title nvarchar(256) not null,
  FeedUrl nvarchar(1024) not null,
  FeedUrlChecksum as (checksum(FeedUrl)) persisted not null,
  SiteUrl nvarchar(1024) not null,
  SiteUrlChecksum as (checksum(SiteUrl)) persisted not null,
  DateLastCrawlStarted datetime2 null,
  constraint PK_Feed primary key clustered (
    Id asc
  )
)
