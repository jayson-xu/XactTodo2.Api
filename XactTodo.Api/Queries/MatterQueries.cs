using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;

namespace XactTodo.Api.Queries
{
    public class MatterQueries : IMatterQueries
    {
        private string _connectionString = string.Empty;

        public MatterQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<Matter> GetAsync(int id)
        {
            const string sql = @"SELECT M.Id, `Subject`, Content, M.`Password`, ExecutantId, CameFrom, RelatedMatterId, Importance, Deadline, Finished, FinishTime, Remark, TeamId, 
M.EstimatedTimeRequired_Num, M.EstimatedTimeRequired_Unit, M.Periodic, M.IntervalPeriod_Num, M.IntervalPeriod_Unit,
M.CreationTime, M.CreatorUserId, U.DisplayName Creator
  FROM Matter M
  JOIN `User` U ON M.CreatorUserId=U.Id
  WHERE M.IsDeleted=0 AND M.Id=@id";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var matter = await connection.QuerySingleOrDefaultAsync<Matter>(sql, new { id });
                if (matter == null)
                    throw new KeyNotFoundException();
                await LoadEvolvements(matter, connection);

                return matter;
            }
        }

        private async Task LoadEvolvements(Matter matter, DbConnection connection)
        {
        }

        public async Task<IEnumerable<UnfinishedMatterOutline>> GetUnfinishedMatterAsync(int userId)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            return await connection.QueryAsync<UnfinishedMatterOutline>(
                @"SELECT M.Id, M.`Subject`, M.Importance, M.Deadline, T.`Name` as TeamName
                     FROM Matter M
                     LEFT JOIN Team T ON M.TeamId = T.Id
                     WHERE M.Finished=0 and (M.CreatorUserId=@userId OR M.ExecutantId=@userId)
                     ORDER BY M.Importance DESC",
                new { userId }
                );
        }

        public async Task<IEnumerable<MatterOutline>> QueryMattersAsync(int userId, MatterQo qo)
        {
            var sql = $"SELECT M.Id, M.Subject, M.Importance, M.Deadline, M.Finished, M.FinishTime, M.TeamName , M.CreatorName , M.CreationTime FROM vMatter M WHERE (M.CreatorUserId={userId} OR M.ExecutantId={userId})";
            // 事项主题
            if (!string.IsNullOrEmpty(qo.Subject))
            {
                sql += " AND Subject like '%'+@subject='%'";
            }
            // 事项内容
            if (!string.IsNullOrEmpty(qo.Content))
            {
                sql += " AND Content like '%'+@Content+'%'";
            }
            // 负责人Id
            if (qo.ExecutantId.HasValue)
            {
                sql += " AND ExecutantId=@ExecutantId";
            }
            // 是否完成
            if (qo.Finished.HasValue)
            {
                sql += " AND Finished=@Finished";
            }
            // 创建日期
            if (qo.FromCreationTime.HasValue)
            {
                sql += " AND CreationTime>=@FromCreationTime";
            }
            if (qo.ToCreationTime.HasValue)
            {
                sql += " AND CreationTime<=@ToCreationTime";
            }
            // 完成时间
            if (qo.FromFinishTime.HasValue)
            {
                sql += " AND FinishTime>=@FromFinishTime";
            }
            if (qo.ToFinishTime.HasValue)
            {
                sql += " AND FinishTime<=@ToFinishTime";
            }
            // 最后期限
            if (qo.FromDeadline.HasValue)
            {
                sql += " AND Deadline>=@FromDeadline";
            }
            if (qo.ToDeadline.HasValue)
            {
                sql += " AND Deadline>=@ToDeadline";
            }
            // 事项来源
            if (!string.IsNullOrEmpty(qo.CameFrom))
            {
                sql += " AND CameFrom like '%'+@CameFrom+'%'";
            }
            // 关联事项
            if (qo.RelatedMatterId.HasValue)
            {
                sql += " AND RelatedMatterId=@RelatedMatterId";
            }
            // 所属小组，此属性值为null时表示归属个人
            if (qo.TeamId.HasValue)
            {
                sql += " AND TeamId=@TeamId";
            }
            // 备注
            if (!string.IsNullOrEmpty(qo.Remark))
            {
                sql += " AND Remark like '%'+@Remark+'%'";
            }

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return await connection.QueryAsync<MatterOutline>(sql, qo);
        }
    }
}
