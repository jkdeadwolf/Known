﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Known.WorkFlows;

namespace Known.Demo.Entities;

/// <summary>
/// 申请单实体类。
/// </summary>
public class TbApply : FlowEntity
{
    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, CodeType = nameof(ApplyType))]
    [DisplayName("业务类型")]
    [Required(ErrorMessage = "业务类型不能为空！")]
    [MinLength(1), MaxLength(50)]
    public ApplyType BizType { get; set; }

    /// <summary>
    /// 取得或设置业务单号。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, IsForm = true, IsViewLink = true)]
    [DisplayName("业务单号")]
    [Required(ErrorMessage = "业务单号不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string BizNo { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, IsForm = true)]
    [DisplayName("业务名称")]
    [Required(ErrorMessage = "业务名称不能为空！")]
    [MinLength(1), MaxLength(100)]
    public string BizTitle { get; set; }

    /// <summary>
    /// 取得或设置业务内容。
    /// </summary>
    [Column(IsForm = true)]
    [DisplayName("业务内容")]
    public string BizContent { get; set; }

    /// <summary>
    /// 取得或设置业务附件。
    /// </summary>
    [Column(IsForm = true)]
    [DisplayName("业务附件")]
    [MinLength(1), MaxLength(250)]
    public string BizFile { get; set; }

    public override Result ValidCommit()
    {
        var vr = base.Validate();
        vr.Required("业务内容", BizContent);
        return vr;
    }
}