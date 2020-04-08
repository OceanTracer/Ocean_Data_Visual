create database OT_user;

DROP table collect;
DROP table user_info;

create table user_info
(
	umail varchar(40) not null primary key,
	upsword varchar(30) not null,
	uname varchar(30) not null,
	desire varchar(64) default null,	/*long enough*/
	u_status int not null,
	sex char(2)
		check( sex in ('男','女')),
	describe varchar(300)
)
GO

create table collect
(
	umail varchar(40) not null foreign key references user_info,	
	collect_num int,
	collect_time date,
	primary key(umail,collect_num)
)
GO


insert into user_info  VALUES('111@qq.com','123321','test_user',null,1,null,null)

select uname, sex, desire, describe from user_info where umail='111@qq.com'

update user_info set desire='厄尔尼诺现象,拉尼娜,海表温度,海表剖面温度,洋流运动,其他' where umail='111@qq.com'