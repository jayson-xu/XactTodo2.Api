using System.ComponentModel.DataAnnotations;

namespace XactTodo.Api.Queries
{
    /// <summary>
    /// 事项查询
    /// </summary>
    public class MatterQo
    {
        /// <summary>
        /// 事项主题
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// 事项内容
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 负责人Id
        /// </summary>
        public int? ExecutantId { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool? Finished { get; set; }
        
        /// <summary>
        /// 创建日期(起始)
        /// </summary>
        public DateTime? FromCreationTime { get; set; }

        /// <summary>
        /// 创建日期(截止)
        /// </summary>
        public DateTime? ToCreationTime { get; set;}

        /// <summary>
        /// 完成时间(起始)
        /// </summary>
        public DateTime? FromFinishTime { get; set; }

        /// <summary>
        /// 完成时间(截止)
        /// </summary>
        public DateTime? ToFinishTime { get; set;}

        /// <summary>
        /// 最后期限(起始)
        /// </summary>
        public DateTime? FromDeadline { get; set; }

        /// <summary>
        /// 最后期限(截止)
        /// </summary>
        public DateTime? ToDeadline { get; set; }

        /// <summary>
        /// 事项来源
        /// </summary>
        public string? CameFrom { get; set; }

        /// <summary>
        /// 关联事项
        /// </summary>
        public int? RelatedMatterId { get; set; }

        /// <summary>
        /// 所属小组，此属性值为null时表示归属个人
        /// </summary>
        public int? TeamId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

    }
}
