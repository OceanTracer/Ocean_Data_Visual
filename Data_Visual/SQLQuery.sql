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

create table question
(
	num int not null primary key,
	contents varchar(1000) not null,
	A varchar(64) not null,
	B varchar(64) not null,
	C varchar(64) not null,
	D varchar(64) not null,
	right_ans varchar(64) not null,
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

create table report
(
	report_id int not null identity(1,1) primary key,
	reporter varchar(40) foreign key references user_info(umail),	--举报人
	reported varchar(40) foreign key references user_info(umail),	--被举报人
	report_type varchar(5) check(report_type in ('post','reply')),
	content_id int not null,
	report_reason varchar(256),
	report_time datetime default current_timestamp,
	reviewed char(1) default 'N' check(reviewed in ('Y','N'))
)


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

insert into replies VALUES('admin@1.com',1,'Hi there',DATEADD(day,-1,GETDATE()),default)
insert into replies VALUES('admin@1.com',1,'I am your admin',DATEADD(day,-1,GETDATE()),default)
insert into replies VALUES('admin@1.com',1,'Have a good day',DATEADD(day,-1,GETDATE()),default)
insert into replies VALUES('0508@test.com',1,'Hello',DATEADD(day,-1,GETDATE()),default)
insert into replies VALUES('0508@test.com',1,'I ''m new here',DATEADD(day,-1,GETDATE()),default)
insert into replies VALUES('user@test.com',1,'test',GETDATE(),default)
insert into replies VALUES('1@qq.com',2,'Hey there',GETDATE(),default)
insert into replies VALUES('admin@1.com',3,'Hello there',GETDATE(),default)
insert into replies VALUES('admin@1.com',4,'Hi there',GETDATE(),default)
insert into replies VALUES('user@test.com',5,'Hi there',GETDATE(),default)
insert into replies VALUES('admin@1.com',6,'Hi there',GETDATE(),default)
insert into replies VALUES('user@test.com',7,'Hi there',GETDATE(),default)
insert into replies VALUES('11@qq.com',8,'loveu',GETDATE(),default)
insert into replies VALUES('admin@1.com',9,'thanks for feedback',GETDATE(),default)

SELECT post_title,posts.umail,uname,post_content,post_time,post_repcnt
FROM posts,user_info
WHERE post_deleted='N' and post_id=1 and posts.umail=user_info.umail

SELECT rep_content,replies.umail,uname,rep_time,rep_id
FROM replies,user_info
WHERE rep_deleted='N' and post_id=1 and replies.umail=user_info.umail
ORDER BY rep_time

SELECT report_id, reporter, reported, post_id, post_title, report_reason, report_time
FROM report, posts
WHERE report_type='post' and reviewed='Y' and report.content_id=posts.post_id
ORDER BY report_time DESC

SELECT report_id, reporter, reported, rep_id, rep_content, report_reason, report_time
FROM report, replies
WHERE report_type='reply' and reviewed='N' and report.content_id=replies.rep_id
ORDER BY report_time DESC

GO



/*删除科普时删除对应的收藏*/
CREATE TRIGGER deleteCollect
ON collect_info
INSTEAD OF DELETE
AS
	DECLARE @collect_num INT
	SELECT @collect_num=collect_num FROM deleted
	DELETE FROM collect WHERE collect_num=@collect_num
	DELETE FROM collect_info WHERE collect_num=@collect_num
GO
--测试
DELETE FROM collect_info WHERE collect_num=11
GO

/*发帖时增加经验*/
CREATE TRIGGER postExp
ON posts
AFTER INSERT
AS
	DECLARE @umail VARCHAR(40)
	SELECT @umail=umail FROM inserted
	UPDATE user_info SET experience=experience+5 WHERE umail=@umail
GO 


/*发回复时增加经验*/
CREATE TRIGGER replyExp
ON replies
AFTER INSERT
AS
	DECLARE @umail VARCHAR(40)
	SELECT @umail=umail FROM inserted
	UPDATE user_info SET experience=experience+1 WHERE umail=@umail
GO


/*用户增加经验时升级*/
CREATE TRIGGER levelUp
ON user_info
FOR UPDATE
AS
	DECLARE @umail VARCHAR(40)
	DECLARE @exp_new INT, @exp_old INT, @level INT, @type INT
	SELECT @umail=umail,@exp_new=experience FROM inserted
	SELECT @exp_old=experience,@type=u_status FROM deleted
	IF @exp_new!=@exp_old AND @type!=0	--update前后经验变化，且不是管理员
	BEGIN
		IF @exp_new>=6000
			SELECT @level=12
		ELSE IF @exp_new>=3000
			SELECT @level=11
		ELSE IF @exp_new>=2000
			SELECT @level=10
		ELSE IF @exp_new>=1000
			SELECT @level=9
		ELSE IF @exp_new>=500
			SELECT @level=8
		ELSE IF @exp_new>=200
			SELECT @level=7
		ELSE IF @exp_new>=100
			SELECT @level=6
		ELSE IF @exp_new>=50
			SELECT @level=5
		ELSE IF @exp_new>=30
			SELECT @level=4
		ELSE IF @exp_new>=15
			SELECT @level=3
		ELSE IF @exp_new>=5
			SELECT @level=2
		ELSE
			SELECT @level=1
		UPDATE user_info SET u_status=@level WHERE umail=@umail
	END
GO 
--测试
UPDATE user_info SET experience=experience+6 WHERE umail='user@test.com'
SELECT * FROM user_info WHERE umail='user@test.com'
GO


/*回复帖子时更新被回复贴的回复总数*/
CREATE TRIGGER replyPost
ON replies
AFTER INSERT
AS
	DECLARE @post_id INT
	SELECT @post_id=post_id FROM inserted i
	UPDATE posts SET post_repcnt=post_repcnt+1 WHERE post_id=@post_id
GO

/*删除回复时更新被回复贴的回复总数*/
CREATE TRIGGER deleteReply
ON replies
AFTER DELETE
AS
	DECLARE @post_id INT
	SELECT @post_id=post_id FROM deleted i
	UPDATE posts SET post_repcnt=post_repcnt-1 WHERE post_id=@post_id
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


/*向所有用户群发通知*/
CREATE PROC groupNotice
@notice_content VARCHAR(400)
AS
	DECLARE @_umail VARCHAR(40)
	DECLARE lcursor CURSOR
	FOR (SELECT umail FROM user_info WHERE u_status!=0)
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

--0515修复数据
INSERT INTO posts VALUES
('0508@test.com','有人在吗','我新来的','2020/5/14 9:02:21',5,3,'Y'),
('0508@test.com','TESTPOST9','content','2020/5/14 9:02:41',0,4,'N'),
('0508@test.com','你要跳舞吗','你你你你要跳舞吗','2020/5/14 9:02:46',1,3,'N'),
('0508@test.com','TESTPOST7','content','2020/5/14 9:02:50',1,2,'N'),
('0508@test.com','TESTPOST6','content','2020/5/14 9:02:54',0,1,'Y'),
('user@test.com','你可以在这里发帖或提问','这里是帖子的内容','2020/5/14 9:08:12',4,1,'N'),
('0508@test.com','主要是为了测试翻页','请输入内容','2020/5/14 9:15:02',0,3,'Y'),
('0508@test.com','主要是为了测试翻页','请输入内容','2020/5/14 9:15:08',0,3,'N'),
('0508@test.com','主要是为了测试翻页','请输入内容','2020/5/14 9:15:10',0,3,'N'),
('0508@test.com','主要是为了测试翻页','请输入内容','2020/5/14 9:15:14',0,3,'Y'),
('0508@test.com','主要是为了测试翻页','请输入内容','2020/5/14 9:15:20',0,3,'N'),
('1@qq.com','翻页成功','翻页成功','2020/5/14 9:20:21',0,4,'N'),
('1@qq.com','科普内容测试','测试','2020/5/14 9:22:57',1,2,'Y'),
('1@qq.com','请输入标题','请输入内容','2020/5/14 11:03:03',0,0,'Y'),
('0508@test.com','关系型数据库确实是非常好的模型','就是有点费头发','2020/5/14 17:22:52',1,2,'N'),
('1@qq.com','设计是不是太间约了一点','感觉白白的好空','2020/5/14 18:11:53',0,4,'N'),
('admin@1.com','管理员发帖试一试','test1','2020/5/15 8:24:03',1,3,'N'),
('admin@1.com','1','1','2020/5/15 8:30:05',4,3,'N')