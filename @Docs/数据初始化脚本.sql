-- 首先，保存当前的 sql_mode
SELECT @OLD_SQL_MODE := @@SESSION.sql_mode;
-- 设置 sql_mode 来允许显式插入值
SET @@SESSION.sql_mode = 'NO_AUTO_VALUE_ON_ZERO';
 
-- 显式插入具体的值到自增字段
INSERT INTO `User`(Id, UserName,Password,DisplayName,Email,EmailConfirmed,CreatorUserId,IsDeleted,CreationTime) VALUES
	 (-1, 'admin','AAg7N+aceSneb0yQWUI/ILCFSw90R7rdJx/IMBnOnKbGwIVbnQFbikLEnwF74vtg+A==','管理员','',0,NULL,0,NOW()),
	 (1, 'xzc','AFv6vTfOPGcLWS8rtdsejf4JugYEHIXNMXiV0iGGLvhISpdbGG9eDPtzNiLzZczKeQ==','许自成','xzc@cohl.com',0,-1,0,NOW()),
	 (2, 'wanger',NULL,'王二','wanger@test.cn',0,1,0,NOW()),
	 (3, 'zhangsan',NULL,'张三','zhangsan@test.cn',0,1,0,NOW()),
	 (4, 'lisi',NULL,'李四','lisi@test.cn',0,1,0,NOW()),
	 (5, 'yangge',NULL,'杨歌','yangge@test.cn',0,1,0,NOW());

-- 重置 sql_mode
SET @@SESSION.sql_mode = @OLD_SQL_MODE;


insert into Team(Id, Name, ProposedTags, LeaderId, IsDeleted, CreatorUserId, CreationTime)
  values(1, 'llama3本地化应用', '中文微调;ollama;pdf识别', 1, 0, -1, now()),
  (2, 'K3进销存', '订单;付款;流程', 5, 0, 1, now());
  

insert into `Member`(Id, TeamId, UserId, IsSupervisor, IsDeleted, CreatorUserId, CreationTime)
  values(1, 1, 2, 0, 0, 1, now()),
  (2, 1, 3, 0, 0, 1, now()),
  (5, 1, 4, 0, 0, 1, now());
  
insert into `Member`(Id, TeamId, UserId, IsSupervisor, IsDeleted, CreatorUserId, CreationTime)
  values(3, 2, 1, 1, 0, 1, now()),
  (4, 2, 4, 0, 0, 1, now());


/*
  SELECT * FROM `User`;
  SELECT * FROM Team;
  SELECT * FROM `Member` m 
  select * from Matter m order by Importance desc, deadline 
*/
