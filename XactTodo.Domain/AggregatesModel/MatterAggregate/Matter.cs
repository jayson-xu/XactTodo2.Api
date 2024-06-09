﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XactTodo.Domain.SeedWork;
using XactTodo.Security;
using XactTodo.Security.Session;

namespace XactTodo.Domain.AggregatesModel.MatterAggregate
{
    public class Matter : FullAuditedEntity, IAggregateRoot
    {
        private const int MaxPasswordLength = 128;
        public const int MaxSubjectLength = 200;
        public const int MaxRemarkLength = 500;
        public const int MaxCameFromLength = 50;

        [Required]
        [StringLength(MaxSubjectLength)]
        public string Subject { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Content { get; set; }

        /// <summary>
        /// 负责人Id
        /// </summary>
        public int? ExecutantId { get; set; }

        /// <summary>
        /// 事项来源
        /// </summary>
        [StringLength(MaxCameFromLength)]
        public string CameFrom { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <remarks>如果设定了此密码，则在查看或编辑事项详情时必须先核对密码，事项创建人可重置此密码</remarks>
        [StringLength(MaxPasswordLength)]
        public string Password { get; set; }
        
        /// <summary>
        /// 关联事项
        /// </summary>
        public int? RelatedMatterId { get; set; }

        public Importance Importance { get; set; }

        public PeriodOfTime EstimatedTimeRequired { get; set; }

        public DateTime? Deadline { get; set; }

        public bool Finished { get; set; }

        public DateTime? FinishTime { get; set; }

        public bool Periodic { get; set; }

        public PeriodOfTime IntervalPeriod {get; set;}

        [StringLength(MaxRemarkLength)]
        public string Remark { get; set; }

        /// <summary>
        /// 所属小组，此属性值为null时表示归属个人
        /// </summary>
        public int? TeamId { get; set; }

        public ICollection<Evolvement> Evolvements { get; set; } = new List<Evolvement>();

        //public IEnumerable<Attachment> Attachments { get; set; }

        public bool SetFinished(DateTime? finishTime, string comment, IClaimsSession session)
        {
            this.Finished = finishTime.HasValue;
            this.FinishTime = finishTime;
            this.Evolvements.Add(new Evolvement
            {
                Comment = (this.Finished ? "事项完成" : "重启事项") + (string.IsNullOrWhiteSpace(comment) ? "" : "：")
                + $"{comment} by {session.NickName}({session.UserName})",
                MatterId = this.Id,
            });
            return true;
        }
    }
}
