﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XactTodo.Api.DTO;
using XactTodo.Api.Queries;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Security.Session;

namespace XactTodo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatterController : ControllerBase
    {
        private const string KEY_AUTHORIZATION = "authorization";
        private readonly IClaimsSession session;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger logger;
        private readonly IMatterRepository matterRepository;
        private readonly IMatterQueries matterQueries;
        private readonly ITeamQueries teamQueries;

        public MatterController(
            IHttpContextAccessor httpContextAccessor,
            ILogger<MatterController> logger,
            IMatterRepository matterRepository,
            IMatterQueries matterQueries,
            ITeamQueries teamQueries,
            IClaimsSession session)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.matterRepository = matterRepository ?? throw new ArgumentNullException(nameof(matterRepository));
            this.matterQueries = matterQueries ?? throw new ArgumentNullException(nameof(matterQueries)); ;
            this.teamQueries = teamQueries ?? throw new ArgumentNullException(nameof(teamQueries)); ;
        }

        private IEnumerable<KeyValuePair<int,string>> GetEnumKeyValues<T>() where T: Enum
        {
            var values = (T[]) Enum.GetValues(typeof(T));
            var kvs = new KeyValuePair<int, string>[values.Length];
            for(int i=0; i<values.Length; i++)
            {
                kvs[i] = new KeyValuePair<int, string>((int)(object)values[i], values[i].ToString());
            }
            return kvs;
        }

        /// <summary>
        /// 获取重要性枚举值
        /// </summary>
        /// <returns></returns>
        [HttpGet("importances")]
        [ProducesResponseType(typeof(IEnumerable<KeyValuePair<int, string>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetImportances()
        {
            //var kvs = GetEnumKeyValues<Importance>();
            var kvs = new KeyValuePair<int, string>[]
            {
                new((int)Importance.Uncertain, "不确定"),
                new((int)Importance.Unimportant, "不重要"),
                new((int)Importance.Normal, "一般"),
                new((int)Importance.Important, "重要"),
                new((int)Importance.VeryImportant, "非常重要"),
            };
            return Ok(kvs);
        }

        /// <summary>
        /// 获取时间单位枚举
        /// </summary>
        /// <returns></returns>
        [HttpGet("timeunits")]
        [ProducesResponseType(typeof(IEnumerable<KeyValuePair<int,string>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTimeUnits()
        {
            var kvs = new KeyValuePair<int, string>[]
            {
                new((int)TimeUnit.Weekday, "工作日"),
                new((int)TimeUnit.NaturalDay, "自然日"),
                new((int)TimeUnit.Week, "周"),
                new((int)TimeUnit.Month, "月"),
                new((int)TimeUnit.Year, "年"),
            };
            return Ok(kvs);
        }

        /// <summary>
        /// 获取与当前用户相关的未完成事项
        /// </summary>
        /// <param name="excludedTeamsId">排除指定的组</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("unfinished")]
        [ProducesResponseType(typeof(IEnumerable<UnfinishedMatterOutline>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUnfinishedMatters(string? excludedTeamsId)
        {
            session.VerifyLoggedin();
            var unfinishedMatters = await matterQueries.GetUnfinishedMatterAsync(session.UserId.Value);
            return Ok(unfinishedMatters);
        }

        /// <summary>
        /// 查询与当前用户相关的事项
        /// </summary>
        /// <param name="qo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("query")]
        [ProducesResponseType(typeof(IEnumerable<MatterOutline>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> QueryMatters(MatterQo qo)
        {
            session.VerifyLoggedin();
            var unfinishedMatters = await matterQueries.QueryMattersAsync(session.UserId.Value, qo);
            return Ok(unfinishedMatters);
        }

        /// <summary>
        /// 获取指定事项
        /// </summary>
        /// <param name="id">事项Id</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id:int}")] //加上类型声明的好处是，如果传入的参数不是整数则直接返回404，不加则返回400并报告错误"The value 'xxx' is not valid."
        [HttpGet]
        [ProducesResponseType(typeof(Queries.Matter), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var matter = await matterQueries.GetAsync(id);
                var accessible = matter.ExecutantId == session.UserId;
                if (!accessible && matter.TeamId.HasValue)
                {
                    var team = await teamQueries.GetAsync(matter.TeamId.Value);
                    if(team != null)
                    {
                        accessible = team.LeaderId==session.UserId || team.Members.Exists(m => m.UserId == session.UserId);
                    }
                }
                if (!accessible)
                {
                    return Forbid($"当前用户{session.UserId}无权读取指定事项{id}");
                }
                return Ok(matter);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 创建新事项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //POST api/v1/[controller]/[action]
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create(MatterInput input)
        {
            var matter = new Domain.AggregatesModel.MatterAggregate.Matter
            {
                Subject = input.Subject,
                Content = input.Content,
                ExecutantId = input.ExecutantId,
                CameFrom = input.CameFrom,
                Password = input.Password,
                RelatedMatterId = input.RelatedMatterId,
                Importance = input.Importance,
                Deadline = input.Deadline,
                Finished = input.Finished,
                FinishTime = input.FinishTime,
                Periodic = input.Periodic,
                Remark = input.Remark,
                TeamId = input.TeamId
            };
            if(input.EstimatedTimeRequired_Num > 0)
            {
                matter.EstimatedTimeRequired = new PeriodOfTime(input.EstimatedTimeRequired_Num, input.EstimatedTimeRequired_Unit);
            }
            if(input.Periodic && input.IntervalPeriod_Num>0)
            {
                matter.IntervalPeriod = new PeriodOfTime(input.IntervalPeriod_Num, input.IntervalPeriod_Unit);
            }
            matterRepository.Add(matter);
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = matter.Id }, matter.Id);
        }

        /// <summary>
        /// 更新事项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        //PUT api/v1/[controller]/{id}
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update([FromRoute] int id, MatterInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != input.Id)
            {
                return BadRequest();
            }
            var matter = await matterRepository.GetAsync(id);
            matter.Subject = input.Subject;
            matter.Content = input.Content;
            matter.ExecutantId = input.ExecutantId;
            matter.CameFrom = input.CameFrom;
            matter.Password = input.Password;
            matter.RelatedMatterId = input.RelatedMatterId;
            matter.Importance = input.Importance;
            matter.Deadline = input.Deadline;
            matter.Finished = input.Finished;
            matter.FinishTime = input.FinishTime;
            matter.Periodic = input.Periodic;
            matter.Remark = input.Remark;
            matter.TeamId = input.TeamId;
            if (input.EstimatedTimeRequired_Num > 0)
            {
                matter.EstimatedTimeRequired = new PeriodOfTime(input.EstimatedTimeRequired_Num, input.EstimatedTimeRequired_Unit);
            }
            else
            {
                matter.EstimatedTimeRequired = null;
            }
            if (input.Periodic && input.IntervalPeriod_Num > 0)
            {
                matter.IntervalPeriod = new PeriodOfTime(input.IntervalPeriod_Num, input.IntervalPeriod_Unit);
            }
            else
            {
                matter.IntervalPeriod = null;
            }
            matterRepository.Update(matter);
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 删除事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                matterRepository.Delete(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 将指定事项置为已完成状态
        /// </summary>
        /// <param name="id">事项Id</param>
        /// <param name="ask">完成时间及备注等</param>
        /// <returns></returns>
        [HttpPost("{id:int}/[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Finish(int id, MatterFinishAsk ask)
        {
            return await Finish(id, ask.FinishTime, ask.Comment);
        }

        /// <summary>
        /// 将指定事项置为未完成状态
        /// </summary>
        /// <param name="id">事项Id</param>
        /// <param name="comment">备注</param>
        /// <returns></returns>
        [Route("{id:int}/[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Unfinish(int id, string? comment)
        {
            return await Finish(id, null, comment);
        }

        private async Task<IActionResult> Finish(int id, DateTime? finishTime, string? comment)
        {
            var matter = await matterRepository.GetAsync(id);
            if (matter == null)
            {
                return BadRequest($"Matter not found: {id}");
            }
            if (!matter.SetFinished(finishTime, comment, session))
                return BadRequest();
            await matterRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

    }

}