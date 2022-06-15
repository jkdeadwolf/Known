﻿create table `APrototype` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  null,
    `CompNo`     varchar(50)  not null,
    `Type`       varchar(50)  null,
    `HeadId`     varchar(50)  null,
    `Json`       text         null,
    PRIMARY KEY (`Id`)
);

create table `SysModule` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  null,
    `CompNo`     varchar(50)  not null,
    `ParentId`   varchar(50)  null,
    `Type`       varchar(50)  not null,
    `Code`       varchar(50)  null,
    `Name`       varchar(50)  not null,
    `Icon`       varchar(50)  null,
    `Url`        varchar(200) null,
    `Target`     varchar(50)  null,
    `Sort`       int          not null,
    `Enabled`    int          not null,
    `Note`       text         null,
    PRIMARY KEY (`Id`)
);

create table `SysConfig` (
    `AppId`       varchar(50)   not null,
    `ConfigKey`   varchar(50)   not null,
    `ConfigValue` varchar(4000) not null,
    PRIMARY KEY(`AppId`,`ConfigKey`)
);

create table `SysLog` (
    `Id`         varchar(50) not null,
    `CreateBy`   varchar(50) not null,
    `CreateTime` datetime    not null,
    `ModifyBy`   varchar(50) null,
    `ModifyTime` datetime    null,
    `Version`    int         not null,
    `Extension`  text        null,
    `AppId`      varchar(50) not null,
    `CompNo`     varchar(50) not null,
    `Type`       varchar(50) null,
    `Target`     varchar(50) null,
    `Content`    text        null,
    PRIMARY KEY (`Id`)
);

create table `SysFile` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  not null,
    `CompNo`     varchar(50)  not null,
    `Category1`  varchar(50)  not null,
    `Category2`  varchar(50)  null,
    `Name`       varchar(50)  not null,
    `Type`       varchar(50)  null,
    `Path`       varchar(250) not null,
    `Size`       int          not null,
    `SourceName` varchar(50)  not null,
    `ExtName`    varchar(50)  not null,
    `Note`       varchar(500) null,
    `BizId`      varchar(50)  null,
    PRIMARY KEY(`Id`)
);

create table `SysTask` (
    `Id`         varchar(50)   not null,
    `CreateBy`   varchar(50)   not null,
    `CreateTime` datetime      not null,
    `ModifyBy`   varchar(50)   null,
    `ModifyTime` datetime      null,
    `Version`    int           not null,
    `Extension`  text          null,
    `AppId`      varchar(50)   not null,
    `CompNo`     varchar(50)   not null,
    `BizId`      varchar(50)   not null,
    `Type`       varchar(50)   not null,
    `Name`       varchar(50)   not null,
    `Target`     varchar(200)  not null,
    `Status`     varchar(50)   not null,
    `BeginTime`  datetime      null,
    `EndTime`    datetime      null,
    `Note`       varchar(4000) null,
    PRIMARY KEY(`Id`)
);

create table `SysDictionary` (
    `Id`           varchar(50) not null,
    `CreateBy`     varchar(50) not null,
    `CreateTime`   datetime    not null,
    `ModifyBy`     varchar(50) null,
    `ModifyTime`   datetime    null,
    `Version`      int         not null,
    `Extension`    text        null,
    `AppId`        varchar(50) not null,
    `CompNo`       varchar(50) not null,
    `Category`     varchar(50) null,
    `CategoryName` varchar(50) null,
    `Code`         varchar(50) null,
    `Name`         varchar(50) null,
    `Sort`         int         not null,
    `Enabled`      int         not null,
    `Note`         text        null,
    PRIMARY KEY (`Id`)
);

create table `SysOrganization` (
    `Id`         varchar(50) not null,
    `CreateBy`   varchar(50) not null,
    `CreateTime` datetime    not null,
    `ModifyBy`   varchar(50) null,
    `ModifyTime` datetime    null,
    `Version`    int         not null,
    `Extension`  text        null,
    `AppId`      varchar(50) not null,
    `CompNo`     varchar(50) not null,
    `ParentId`   varchar(50) null,
    `Code`       varchar(50) null,
    `Name`       varchar(50) not null,
    `ManagerId`  varchar(50) null,
    `Note`       text        null,
    PRIMARY KEY (`Id`)
);

create table `SysRole` (
    `Id`         varchar(50) not null,
    `CreateBy`   varchar(50) not null,
    `CreateTime` datetime    not null,
    `ModifyBy`   varchar(50) null,
    `ModifyTime` datetime    null,
    `Version`    int         not null,
    `Extension`  text        null,
    `AppId`      varchar(50) not null,
    `CompNo`     varchar(50) not null,
    `Name`       varchar(50) not null,
    `Enabled`    int         not null,
    `Note`       text        null,
    PRIMARY KEY (`Id`)
);

create table `SysRoleModule` (
    `RoleId`   varchar(50) not null,
    `ModuleId` varchar(50) not null,
    PRIMARY KEY (`RoleId`,`ModuleId`)
);

create table `SysUser` (
    `Id`             varchar(50)  not null,
    `CreateBy`       varchar(50)  not null,
    `CreateTime`     datetime     not null,
    `ModifyBy`       varchar(50)  null,
    `ModifyTime`     datetime     null,
    `Version`        int          not null,
    `Extension`      text         null,
    `AppId`          varchar(50)  not null,
    `CompNo`         varchar(50)  not null,
    `OrgNo`          varchar(50)  not null,
    `UserName`       varchar(50)  not null,
    `Password`       varchar(50)  not null,
    `Name`           varchar(50)  not null,
    `EnglishName`    varchar(50)  null,
    `Gender`         varchar(50)  null,
    `Phone`          varchar(50)  null,
    `Mobile`         varchar(50)  null,
    `Email`          varchar(50)  null,
    `Enabled`        int          not null,
    `Note`           text         null,
    `FirstLoginTime` datetime     null,
    `FirstLoginIP`   varchar(50)  null,
    `LastLoginTime`  datetime     null,
    `LastLoginIP`    varchar(50)  null,
    `Role`           varchar(500) null,
    PRIMARY KEY (`Id`)
);

create table `SysUserModule` (
    `UserId`   varchar(50) not null,
    `ModuleId` varchar(50) not null,
    PRIMARY KEY (`UserId`,`ModuleId`)
);

create table `SysUserRole` (
    `UserId` varchar(50) not null,
    `RoleId` varchar(50) not null,
    PRIMARY KEY (`UserId`,`RoleId`)
);

create table `SysNotice` (
    `Id`          varchar(50)   not null,
    `CreateBy`    varchar(50)   not null,
    `CreateTime`  datetime      not null,
    `ModifyBy`    varchar(50)   null,
    `ModifyTime`  datetime      null,
    `Version`     int           not null,
    `Extension`   text          null,
    `AppId`       varchar(50)   not null,
    `CompNo`      varchar(50)   not null,
    `Status`      varchar(50)   not null,
    `Title`       varchar(50)   not null,
    `Content`     varchar(4000) null,
    `PublishBy`   varchar(50)   null,
    `PublishTime` datetime      null,
    PRIMARY KEY(`Id`)
);

create table `SysUserLink` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  not null,
    `CompNo`     varchar(50)  not null,
    `UserName`   varchar(50)  not null,
    `Type`       varchar(50)  not null,
    `Name`       varchar(50)  not null,
    `Address`    varchar(200) not null,
    PRIMARY KEY(`Id`)
);

create table `SysUserMessage` (
    `Id`         varchar(50)   not null,
    `CreateBy`   varchar(50)   not null,
    `CreateTime` datetime      not null,
    `ModifyBy`   varchar(50)   null,
    `ModifyTime` datetime      null,
    `Version`    int           not null,
    `Extension`  text          null,
    `AppId`      varchar(50)   not null,
    `CompNo`     varchar(50)   not null,
    `UserId`     varchar(50)   not null,
    `Type`       varchar(50)   not null,
    `MsgBy`      varchar(50)   not null,
    `MsgByName`  varchar(50)   not null,
    `MsgLevel`   varchar(50)   not null,
    `Category`   varchar(50)   null,
    `Subject`    varchar(250)  not null,
    `Content`    varchar(4000) not null,
    `FilePath`   varchar(500)  null,
    `IsHtml`     varchar(50)   not null,
    `Status`     varchar(50)   not null,
    `BizId`      varchar(50)   null,
    PRIMARY KEY(`Id`)
);

create table `SysFlow` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  not null,
    `CompNo`     varchar(50)  not null,
    `FlowCode`   varchar(50)  not null,
    `FlowName`   varchar(50)  not null,
    `FlowStatus` varchar(50)  not null,
    `BizId`      varchar(50)  not null,
    `BizName`    varchar(200) not null,
    `BizUrl`     varchar(200) not null,
    `BizStatus`  varchar(50)  not null,
    `CurrStep`   varchar(50)  not null,
    `CurrById`   varchar(200) not null,
    `CurrBy`     varchar(100) not null,
    `PrevStep`   varchar(50)  null,
    `PrevById`   varchar(200) null,
    `PrevBy`     varchar(100) null,
    `NextStep`   varchar(50)  null,
    `NextById`   varchar(200) null,
    `NextBy`     varchar(100) null,
    PRIMARY KEY(`Id`)
);

create table `SysFlowLog` (
    `Id`          varchar(50)   not null,
    `CreateBy`    varchar(50)   not null,
    `CreateTime`  datetime      not null,
    `ModifyBy`    varchar(50)   null,
    `ModifyTime`  datetime      null,
    `Version`     int           not null,
    `Extension`   text          null,
    `AppId`       varchar(50)   not null,
    `CompNo`      varchar(50)   not null,
    `BizId`       varchar(50)   not null,
    `StepName`    varchar(50)   not null,
    `ExecuteById` varchar(50)   not null,
    `ExecuteBy`   varchar(50)   not null,
    `ExecuteTime` datetime      not null,
    `Result`      varchar(50)   not null,
    `Note`        varchar(1000) null,
    PRIMARY KEY(`Id`)
);

create table `SysFlowStep` (
    `Id`            varchar(50)   not null,
    `CreateBy`      varchar(50)   not null,
    `CreateTime`    datetime      not null,
    `ModifyBy`      varchar(50)   null,
    `ModifyTime`    datetime      null,
    `Version`       int           not null,
    `Extension`     text          null,
    `AppId`         varchar(50)   not null,
    `CompNo`        varchar(50)   not null,
    `FlowCode`      varchar(50)   not null,
    `FlowName`      varchar(50)   not null,
    `StepCode`      varchar(50)   not null,
    `StepName`      varchar(50)   not null,
    `StepType`      varchar(50)   not null,
    `OperateBy`     varchar(50)   null,
    `OperateByName` varchar(50)   null,
    `Note`          varchar(500)  null,
    `X`             int           null,
    `Y`             int           null,
    `IsRound`       int           null,
    `Arrows`        varchar(4000) null,
    PRIMARY KEY(`Id`)
);

create table `SysNoRule` (
    `Id`          varchar(50)   not null,
    `CreateBy`    varchar(50)   not null,
    `CreateTime`  datetime      not null,
    `ModifyBy`    varchar(50)   null,
    `ModifyTime`  datetime      null,
    `Version`     int           not null,
    `Extension`   text          null,
    `AppId`       varchar(50)   not null,
    `CompNo`      varchar(50)   not null,
    `Code`        varchar(50)   not null,
    `Name`        varchar(50)   not null,
    `Description` varchar(500)  null,
    `RuleData`    varchar(4000) null,
    PRIMARY KEY(`Id`)
);

create table `SysNoRuleData` (
    `AppId`      varchar(50) not null,
    `CompNo`     varchar(50) not null,
    `RuleId`     varchar(50) not null,
    `RuleNo`     varchar(50) not null
);