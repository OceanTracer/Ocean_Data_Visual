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
		check(enabled in ('Y','N'))	--可用/禁用
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
	collect_num int not null primary key identity(1,1),
	collect_txt varchar(2048),
	collect_pic image,
	create_by varchar(40) default null
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

insert into user_info  VALUES('user@test.com','123321','test_user',null,1,null,null,'Y')
insert into user_info  VALUES('admin@1.com','admin','Admin',null,0,null,null,'Y')
insert into user_info VALUES('1@qq.com','123321','1',null,1,null,null,'Y')
insert into user_info  VALUES('11@qq.com','123321','123',default,default,default,default,default)
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
GO


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