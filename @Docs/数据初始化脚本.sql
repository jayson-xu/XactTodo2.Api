select * from `user` u 

insert into `user` (Id, UserName, Password, DisplayName, Email, EmailConfirmed, IsDeleted, CreatorUserId, CreationTime)
  values(2, 'wanger', null, '王二', 'wanger@test.cn', 0, 0, 1, now()),
  (3, 'zhangsan', null, '张三', 'zhangsan@test.cn', 0, 0, 1, now()),
  (4, 'lisi', null, '李四', 'lisi@test.cn', 0, 0, 1, now()),
  (5, 'yangge', null, '杨歌', 'yangge@test.cn', 0, 0, 1, now())

SELECT * FROM team;

insert into team(Id, Name, ProposedTags, LeaderId, IsDeleted, CreatorUserId, CreationTime)
  values(1, 'llama3本地化应用', '中文微调;ollama;pdf识别', 1, 0, -1, now())

insert into team(Id, Name, ProposedTags, LeaderId, IsDeleted, CreatorUserId, CreationTime)
  values(2, 'K3进销存', '订单;付款;流程', 5, 0, 1, now())
  
select * from `member` m 

insert into `member`(Id, TeamId, UserId, IsSupervisor, IsDeleted, CreatorUserId, CreationTime)
  values(1, 1, 2, 0, 0, 1, now()),
  (2, 1, 3, 0, 0, 1, now())
  (5, 1, 4, 0, 0, 1, now())
  
insert into `member`(Id, TeamId, UserId, IsSupervisor, IsDeleted, CreatorUserId, CreationTime)
  values(3, 2, 1, 1, 0, 1, now()),
  (4, 2, 4, 0, 0, 1, now())

  
  select * from matter m order by Importance desc, deadline 