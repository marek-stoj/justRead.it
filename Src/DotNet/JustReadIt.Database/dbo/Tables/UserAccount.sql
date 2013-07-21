create table dbo.UserAccount (
  Id int identity(1, 1) not null,
  Email nvarchar(256) not null,
  constraint PK_UserAccount primary key clustered (
    Id asc
  )
)
