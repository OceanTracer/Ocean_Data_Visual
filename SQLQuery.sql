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
	describe varchar(300),
	enabled char(1) default 'Y'
		check(enabled in ('Y','N'))	/*可用/禁用*/
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

create table collect_info
(	
	collect_num int not null,
	collect_pic varchar(99),
	collect_txt varchar(400),
	primary key(collect_num)
)
GO

insert into user_info  VALUES('111@qq.com','123321','test_user',null,1,null,null)

insert into collect_info VALUES('1','\\pic_all\\1.jpg','\\pic_all\\1.txt')
insert into collect_info VALUES('2','\\pic_all\\2.jpg','\\pic_all\\2.txt')
insert into collect_info VALUES('3','\\pic_all\\3.jpg','\\pic_all\\3.txt')
insert into collect_info VALUES('4','\\pic_all\\4.jpg','\\pic_all\\4.txt')
insert into collect_info VALUES('5','\\pic_all\\5.jpg','\\pic_all\\5.txt')
insert into collect_info VALUES('6','\\pic_all\\6.jpg','\\pic_all\\6.txt')
insert into collect_info VALUES('7','\\pic_all\\7.jpg','\\pic_all\\7.txt')
insert into collect_info VALUES('8','\\pic_all\\8.jpg','\\pic_all\\8.txt')

select uname, sex, desire, describe from user_info where umail='111@qq.com'

update user_info set desire='厄尔尼诺现象,拉尼娜,海表温度,海表剖面温度,洋流运动,其他' where umail='111@qq.com'

update user_info set enabled='N' where umail=umail

select collect_num, count(*) from collect group by collect_num
select desire from user_info