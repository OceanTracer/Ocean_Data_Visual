create database OT_user;

DROP table collect;
DROP table notice;
DROP table user_info;

create table user_info
(
	umail varchar(40) not null primary key,
	upsword varchar(30) not null,
	uname varchar(30) not null,
	desire varchar(64) default null,	--long enough
	u_status int not null default 1,
	sex char(2) default null
		check( sex in ('男','女')),
	describe varchar(300) default null,
	enabled char(1) default 'Y'
		check(enabled in ('Y','N')),	--可用/禁用
	experience int not null default 0,
	last_sign datetime default null,
	portrait image
)
GO

create table collect
(
	umail varchar(40) not null foreign key references user_info,
	collect_num int foreign key references collect_info,
	collect_time date,
	primary key(umail,collect_num)
)
GO

create table collect_info
(
	collect_num int not null primary key ,
	collect_txt varchar(2048),
	collect_pic image,
	create_by varchar(40) default null,
	approved char(1) default 'N', check(approved in ('Y','N'))
)
GO

create table notice
(
	umail varchar(40) foreign key references user_info,
	notice_content varchar(400) not null,
	notice_time datetime not null
	primary key(umail,notice_time)
)
GO

create table posts
(
  post_id int not null identity(1,1) primary key,
  umail varchar(40) foreign key references user_info,
  post_title varchar(256) not null,
  post_content text null default null,
  post_time datetime not null default current_timestamp,
  post_repcnt int not null default 0,
  post_section smallint not null default 1,
  post_deleted char(1) default 'N', check(post_deleted in ('Y','N'))
)
GO

create table replies
(
  rep_id int not null identity(1,1) primary key ,
  umail varchar(40) foreign key references user_info,
  post_id int not null foreign key references posts,
  rep_content text not null,
  rep_time datetime not null,
  rep_deleted char(1) default 'N', check(rep_deleted in ('Y','N'))
)
GO


insert into user_info  VALUES('user@test.com','123321','test_user',null,1,null,null,'Y',0,null,null)
insert into user_info  VALUES('admin@1.com','admin','Admin',null,0,null,null,'Y',0,null,null)
insert into user_info VALUES('1@qq.com','123321','1',null,1,null,null,'Y',0,null,null)
insert into user_info  VALUES('11@qq.com','123321','123',default,default,default,default,default,0,null,null)
GO

insert into collect VALUES('user@test.com',1,GETDATE())
insert into collect VALUES('user@test.com',2,GETDATE())
insert into collect VALUES('user@test.com',3,GETDATE())
insert into collect VALUES('user@test.com',4,GETDATE())
insert into collect VALUES('user@test.com',5,GETDATE())
insert into collect VALUES('user@test.com',6,GETDATE())
insert into collect VALUES('1@qq.com',2,GETDATE())
insert into collect VALUES('1@qq.com',3,GETDATE())
insert into collect VALUES('1@qq.com',4,GETDATE())
insert into collect VALUES('11@qq.com',3,GETDATE())

insert into collect_info VALUES('1','\\pic_all\\1.jpg','\\pic_all\\1.txt')
insert into collect_info VALUES('2','\\pic_all\\2.jpg','\\pic_all\\2.txt')
insert into collect_info VALUES('3','\\pic_all\\3.jpg','\\pic_all\\3.txt')
insert into collect_info VALUES('4','\\pic_all\\4.jpg','\\pic_all\\4.txt')
insert into collect_info VALUES('5','\\pic_all\\5.jpg','\\pic_all\\5.txt')
insert into collect_info VALUES('6','\\pic_all\\6.jpg','\\pic_all\\6.txt')
insert into collect_info VALUES('7','\\pic_all\\7.jpg','\\pic_all\\7.txt')
insert into collect_info VALUES('8','\\pic_all\\8.jpg','\\pic_all\\8.txt')

select uname, sex, desire, describe from user_info where umail='user@test.com'

update user_info set desire='厄尔尼诺现象,拉尼娜,海表温度,海表剖面温度,洋流运动,其他' where umail='user@test.com'

update user_info set enabled='N' where umail='user@test.com'

select collect_num, count(*) as 'count' from collect group by collect_num
UNION
select collect_num, 0 as 'count' from collect_info where collect_num not in(select collect_num from collect)

select desire from user_info

insert into notice values('user@test.com','notice_content',GETDATE())
insert into notice values('user@test.com','notice2',GETDATE())

select notice_content, notice_time from notice where umail='user@test.com' order by notice_time desc

insert into posts VALUES('user@test.com','TESTPOST1','content',GETDATE(),default,1,default)
insert into posts VALUES('1@qq.com','TESTPOST2','content',GETDATE(),default,1,default)
insert into posts VALUES('1@qq.com','TESTPOST3','content',GETDATE(),default,1,default)
insert into posts VALUES('0508@test.com','TESTPOST4','content',GETDATE(),default,1,default)
insert into posts VALUES('11@qq.com','TESTPOST5','content',GETDATE(),default,1,default)
insert into posts VALUES('0508@test.com','TESTPOST6','content',GETDATE(),default,1,default)
insert into posts VALUES('0508@test.com','TESTPOST7','content',GETDATE(),default,2,default)
insert into posts VALUES('0508@test.com','TESTPOST8','content',GETDATE(),default,3,default)
insert into posts VALUES('0508@test.com','TESTPOST9','content',GETDATE(),default,4,default)
insert into posts VALUES('0508@test.com','有人在吗','我新来的',GETDATE(),default,3,default)

insert into replies VALUES('admin@1.com',1,'Hi there',GETDATE(),default)
insert into replies VALUES('admin@1.com',1,'I am your admin',GETDATE(),default)
insert into replies VALUES('admin@1.com',1,'Have a good day',GETDATE(),default)
insert into replies VALUES('0508@test.com',1,'Hello',GETDATE(),default)
insert into replies VALUES('0508@test.com',1,'I am new here',GETDATE(),default)
insert into replies VALUES('user@test.com',1,'test',GETDATE(),default)
insert into replies VALUES('1@qq.com',2,'Hey there',GETDATE(),default)
insert into replies VALUES('admin@1.com',3,'Hello there',GETDATE(),default)
insert into replies VALUES('admin@1.com',4,'Hi there',GETDATE(),default)
insert into replies VALUES('user@test.com',5,'Hi there',GETDATE(),default)
insert into replies VALUES('admin@1.com',6,'Hi there',GETDATE(),default)
insert into replies VALUES('user@test.com',7,'Hi there',GETDATE(),default)
insert into replies VALUES('11@qq.com',8,'loveu',GETDATE(),default)
insert into replies VALUES('admin@1.com',9,'thanks for feedback',GETDATE(),default)

SELECT post_title,uname,post_content,post_time,post_repcnt
FROM posts,user_info
WHERE post_deleted='N' and post_id=1 and posts.umail=user_info.umail

SELECT uname,rep_content,rep_time
FROM replies,user_info
WHERE rep_deleted='N' and post_id=1 and replies.umail=user_info.umail
ORDER BY rep_time

GO


TRUNCATE TABLE replies

/*回复帖子时更新被回复贴的回复总数*/
CREATE TRIGGER replyPost
ON replies
AFTER INSERT
AS
	DECLARE @post_id INT
	SELECT @post_id=post_id FROM inserted i
	UPDATE posts SET post_repcnt=post_repcnt+1 WHERE post_id=@post_id
GO


/*获取指定页数的帖子*/
CREATE PROC fetchPosts
@page_num INT,		--当前页  
@page_size INT,		--每页多少条
@section SMALLINT	--版块
AS  
	SELECT post_id,uname,post_title,post_time,post_repcnt
	FROM posts,user_info
	WHERE post_deleted='N' and post_section=@section and posts.umail=user_info.umail
	ORDER BY post_time DESC
	OFFSET (@page_size*(@page_num - 1)) ROW FETCH NEXT @page_size ROWS ONLY;
GO
--测试
DECLARE @page_num INT,@page_size INT,@section SMALLINT
SELECT @page_num=1,@page_size=5,@section=1
EXEC fetchPosts @page_num,@page_size,@section
GO


/*获取指定帖子的回复
CREATE PROC fetchReplies
@page_num INT,		--当前页  
@page_size INT,		--每页多少条
@post_id SMALLINT	--帖子id
AS  
	SELECT uname,rep_content,rep_time
	FROM replies,user_info
	WHERE rep_deleted='N' and post_id=@post_id and replies.umail=user_info.umail
	ORDER BY rep_time
	OFFSET (@page_size*(@page_num - 1)) ROW FETCH NEXT @page_size ROWS ONLY;
GO*/



/*向所有用户群发通知*/
CREATE PROC groupNotice
@notice_content VARCHAR(400)
AS
	DECLARE @_umail VARCHAR(40)
	DECLARE lcursor CURSOR
	FOR (SELECT umail FROM user_info WHERE u_status=1)
	OPEN lcursor
	FETCH NEXT FROM lcursor INTO @_umail
	WHILE @@FETCH_STATUS=0
	BEGIN
		INSERT INTO notice VALUES(@_umail,@notice_content,GETDATE())
		FETCH NEXT FROM lcursor INTO @_umail
	END
	CLOSE lcursor
	DEALLOCATE lcursor
GO
--测试
DECLARE @notice_content VARCHAR(400)
SELECT @notice_content='helloworld!'
EXEC groupNotice @notice_content
GO