create table [dbo].[Articles](
	Id uniqueidentifier primary key,
	Title nvarchar(20) not null,
	Content nvarchar(max) not null,
	PostingDate datetime2 not null
)
create table [dbo].[Comments](
	Id uniqueidentifier primary key,
	AuthorName nvarchar(20) not null,
	Text nvarchar(max) not null,
	CommentDate datetime2 not null,
	ArticleId uniqueidentifier foreign key references Articles(Id)
)