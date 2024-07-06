--  ¬œÓ ”Õº
create view vMatter
as
  select m.Id, m.Subject, m.Content, m.ExecutantId, eu.DisplayName ExecutantName, m.CameFrom, m.Password, m.RelatedMatterId, m.Importance,
         m.EstimatedTimeRequired_Num, m.EstimatedTimeRequired_Unit, m.Deadline, m.Finished, m.FinishTime,
         m.Periodic, m.IntervalPeriod_Num, m.IntervalPeriod_Unit, m.Remark, m.TeamId, t.Name TeamName,
         m.CreatorUserId, cu.DisplayName CreatorName, m.CreationTime,
         m.LastModifierUserId, mu.DisplayName LastModifierName, m.LastModificationTime
    from Matter m
    join `User` cu on m.CreatorUserId=cu.Id
    left join `User` eu on m.ExecutantId=eu.Id
    left join `User` mu on m.LastModifierUserId=mu.Id
    left join Team t on m.TeamId=t.Id
    where m.IsDeleted=0

