﻿/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

namespace Known.Entities
{
    /// <summary>
    /// 用户消息实体类。
    /// </summary>
    public class SysUserMessage : EntityBase
    {
        /// <summary>
        /// 取得或设置用户ID。
        /// </summary>
        [Column("用户ID", "", true, "1", "50")]
        public string UserId { get; set; }

        /// <summary>
        /// 取得或设置类型（收件、发件、删除）。
        /// </summary>
        [Column("类型", "", true, "1", "50")]
        public string Type { get; set; }

        /// <summary>
        /// 取得或设置收件人ID。
        /// </summary>
        [Column("收件人ID", "", true, "1", "50")]
        public string MsgBy { get; set; }

        /// <summary>
        /// 取得或设置收件姓名。
        /// </summary>
        [Column("收件姓名", "", true, "1", "50")]
        public string MsgByName { get; set; }

        /// <summary>
        /// 取得或设置级别（一般、紧急）。
        /// </summary>
        [Column("级别", "", true, "1", "50")]
        public string MsgLevel { get; set; }

        /// <summary>
        /// 取得或设置分类。
        /// </summary>
        [Column("分类", "", false, "1", "50")]
        public string Category { get; set; }

        /// <summary>
        /// 取得或设置主题。
        /// </summary>
        [Column("主题", "", true, "1", "250")]
        public string Subject { get; set; }

        /// <summary>
        /// 取得或设置内容。
        /// </summary>
        [Column("内容", "", true, "1", "4000")]
        public string Content { get; set; }

        /// <summary>
        /// 取得或设置附件。
        /// </summary>
        [Column("附件", "", false, "1", "500")]
        public string FilePath { get; set; }

        /// <summary>
        /// 取得或设置是否Html。
        /// </summary>
        [Column("是否Html", "", true, "1", "50")]
        public string IsHtml { get; set; }

        /// <summary>
        /// 取得或设置状态（未读、已读）。
        /// </summary>
        [Column("状态", "", true, "1", "50")]
        public string Status { get; set; }

        /// <summary>
        /// 取得或设置业务ID。
        /// </summary>
        [Column("业务ID", "", false, "1", "50")]
        public string BizId { get; set; }
    }
}