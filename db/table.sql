﻿CREATE TABLE [SysDictionary] (
	[Id] [varchar](50) NOT NULL,
	[CreateBy] [nvarchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[ModifyBy] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
	[Version] [int] NOT NULL,
	[Extension] [ntext] NULL,
	[CompNo] [varchar](50) NOT NULL,
	[Category] [varchar](50) NULL,
	[CategoryName] [nvarchar](50) NULL,
	[Code] [varchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[Sort] [int] NOT NULL,
	[Enabled] [int] NOT NULL,
	[Note] [ntext] NULL,
    CONSTRAINT [PK_SysDictionary] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE SysModule (
	[Id] [varchar](50) NOT NULL,
	[CreateBy] [nvarchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[ModifyBy] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
	[Version] [int] NOT NULL,
	[Extension] [ntext] NULL,
	[CompNo] [varchar](50) NOT NULL,
	[ParentId] [varchar](50) NULL,
	[Type] [varchar](50) NOT NULL,
	[Code] [varchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Icon] [varchar](50) NULL,
	[Url] [varchar](200) NULL,
    [Target] [varchar](50) NULL,
	[Sort] [int] NOT NULL,
	[Enabled] [int] NOT NULL,
	[Note] [ntext] NULL,
    CONSTRAINT [PK_SysModule] PRIMARY KEY ([Id] ASC)
)
GO

insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('f602c158432241359f69eddd380d71ac','System','2020-05-03',1,'known','','菜单','Test','测试系统','layui-icon-windows','',1,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('1bf6a8f9c9e74841b2aadb135a666a76','System','2020-05-03',1,'known','f602c158432241359f69eddd380d71ac','菜单','System','系统管理','layui-icon-set-sm','',1,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('fccb8f778ff84d398d65a0cc4c506e9c','System','2020-05-03',1,'known','1bf6a8f9c9e74841b2aadb135a666a76','菜单','Dictionary','数据字典','layui-icon-list','/System/DictionaryView',1,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('1390a93abbf042a095c4072805cb2104','System','2020-05-03',1,'known','fccb8f778ff84d398d65a0cc4c506e9c','按钮','addCat','新增类别','layui-icon-addition','',1,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('b34871da9a53484dbff535006ea5f89c','System','2020-05-03',1,'known','fccb8f778ff84d398d65a0cc4c506e9c','按钮','add','新增','layui-icon-addition','',2,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('89761ef1af634a8692034938cd773856','System','2020-05-03',1,'known','fccb8f778ff84d398d65a0cc4c506e9c','按钮','edit','编辑','layui-icon-edit','',3,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('ae8310483f534f0a9b80dfad3e17e87e','System','2020-05-03',1,'known','fccb8f778ff84d398d65a0cc4c506e9c','按钮','remove','删除','layui-icon-delete','',4,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('3e1498a4f54b42faac845685806166cb','System','2020-05-03',1,'known','1bf6a8f9c9e74841b2aadb135a666a76','菜单','Organization','组织架构','layui-icon-templeate-1','/System/OrganizationView',2,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('9e2f4c4bc2d049f29b1c87a226a652b1','System','2020-05-03',1,'known','3e1498a4f54b42faac845685806166cb','按钮','add','新增','layui-icon-addition','',1,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('8a6e4abff9fd42f5846533dd16d530ca','System','2020-05-03',1,'known','3e1498a4f54b42faac845685806166cb','按钮','edit','编辑','layui-icon-edit','',2,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('340143e13e0f42c8971d4c64fd275303','System','2020-05-03',1,'known','3e1498a4f54b42faac845685806166cb','按钮','remove','删除','layui-icon-delete','',3,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('239bcf11fb2e44d5bf2420a6310e9cc9','System','2020-05-03',1,'known','1bf6a8f9c9e74841b2aadb135a666a76','菜单','Role','角色管理','layui-icon-user','/System/RoleView',3,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('7a66eab6ccaf4e169071d9cab66a771f','System','2020-05-03',1,'known','239bcf11fb2e44d5bf2420a6310e9cc9','按钮','add','新增','layui-icon-addition','',1,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('919113b6333a4039bca6fa1b06c9d1a0','System','2020-05-03',1,'known','239bcf11fb2e44d5bf2420a6310e9cc9','按钮','edit','编辑','layui-icon-edit','',2,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('227875b8a26444c29f74cef9ac6b5122','System','2020-05-03',1,'known','239bcf11fb2e44d5bf2420a6310e9cc9','按钮','remove','删除','layui-icon-delete','',3,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('67e8164d709b49eaacb1adc0c11d6b6b','System','2020-05-03',1,'known','239bcf11fb2e44d5bf2420a6310e9cc9','按钮','right','权限','layui-icon-component','',4,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('b2543c1b0fb14fdbafec3bf5ae799b30','System','2020-05-03',1,'known','1bf6a8f9c9e74841b2aadb135a666a76','菜单','User','用户管理','layui-icon-username','/System/UserView',4,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('2a3b90b94915405993235f16f946cb82','System','2020-05-03',1,'known','b2543c1b0fb14fdbafec3bf5ae799b30','按钮','add','新增','layui-icon-addition','',1,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('63de4b4b66c24c3090633658beced7b3','System','2020-05-03',1,'known','b2543c1b0fb14fdbafec3bf5ae799b30','按钮','edit','编辑','layui-icon-edit','',2,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('0026acf13bc743f88b5f0f2475a5df8a','System','2020-05-03',1,'known','b2543c1b0fb14fdbafec3bf5ae799b30','按钮','remove','删除','layui-icon-delete','',3,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('4d07b7f7e1754167b8c4e1501c2424f5','System','2020-05-03',1,'known','b2543c1b0fb14fdbafec3bf5ae799b30','按钮','role','角色','layui-icon-group','',5,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('1af5fb78639746fa800333dd18a36fae','System','2020-05-03',1,'known','b2543c1b0fb14fdbafec3bf5ae799b30','按钮','setPwd','重置密码','layui-icon-refresh','',6,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('3fd7e543a93c4ac39e321c8b761ee843','System','2020-05-03',1,'known','b2543c1b0fb14fdbafec3bf5ae799b30','按钮','enable','启用','layui-icon-ok-circle','',7,1)
GO
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('10f45befd0bb46889a14b69e08076faa','System','2020-05-03',1,'known','b2543c1b0fb14fdbafec3bf5ae799b30','按钮','disable','停用','layui-icon-reduce-circle','',8,1)
GO

CREATE TABLE [SysOrganization] (
	[Id] [varchar](50) NOT NULL,
	[CreateBy] [nvarchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[ModifyBy] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
	[Version] [int] NOT NULL,
	[Extension] [ntext] NULL,
	[CompNo] [varchar](50) NOT NULL,
	[ParentId] [varchar](50) NULL,
	[Code] [varchar](50) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ManagerId] [varchar](50) NULL,
	[Note] [ntext] NULL,
	CONSTRAINT [PK_SysOrganization] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE [SysRole] (
	[Id] [varchar](50) NOT NULL,
	[CreateBy] [nvarchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[ModifyBy] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
	[Version] [int] NOT NULL,
	[Extension] [ntext] NULL,
	[CompNo] [varchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Enabled] [int] NOT NULL,
	[Note] [ntext] NULL,
    CONSTRAINT [PK_SysRole] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysRoleModule] (
	[RoleId] [varchar](50) NOT NULL,
	[ModuleId] [varchar](50) NOT NULL,
    CONSTRAINT [PK_SysRoleModule] PRIMARY KEY ([RoleId] ASC,[ModuleId] ASC)
) 
GO

CREATE TABLE [SysUser] (
	[Id] [varchar](50) NOT NULL,
	[CreateBy] [nvarchar](50) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[ModifyBy] [nvarchar](50) NULL,
	[ModifyTime] [datetime] NULL,
	[Version] [int] NOT NULL,
	[Extension] [ntext] NULL,
	[CompNo] [varchar](50) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[EnglishName] [varchar](50) NULL,
	[Gender] [varchar](50) NULL,
	[Phone] [varchar](50) NULL,
	[Mobile] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Enabled] [int] NOT NULL,
	[Note] [ntext] NULL,
	[FirstLoginTime] [datetime] NULL,
	[FirstLoginIP] [varchar](50) NULL,
	[LastLoginTime] [datetime] NULL,
	[LastLoginIP] [varchar](50) NULL,
    CONSTRAINT [PK_SysUser] PRIMARY KEY ([Id] ASC)
) 
GO
insert into SysUser(Id,CreateBy,CreateTime,Version,CompNo,UserName,Password,Name,EnglishName,Enabled)
values('System','System','2020-04-01',1,'known','System','c4ca4238a0b923820dcc509a6f75849b','超级管理员','System',1)
GO
insert into SysUser(Id,CreateBy,CreateTime,Version,CompNo,UserName,Password,Name,EnglishName,Enabled)
values('101ffa5246714083967622761898ea6e','System','2020-04-01',1,'known','admin','c4ca4238a0b923820dcc509a6f75849b','管理员','Administrator',1)
GO

CREATE TABLE [SysUserModule] (
	[UserId] [varchar](50) NOT NULL,
	[ModuleId] [varchar](50) NOT NULL,
    CONSTRAINT [PK_SysUserModule] PRIMARY KEY ([UserId] ASC,[ModuleId] ASC)
) 
GO

CREATE TABLE [SysUserRole] (
	[UserId] [varchar](50) NOT NULL,
	[RoleId] [varchar](50) NOT NULL,
    CONSTRAINT [PK_SysUserRole] PRIMARY KEY ([UserId] ASC,[RoleId] ASC)
) 
GO