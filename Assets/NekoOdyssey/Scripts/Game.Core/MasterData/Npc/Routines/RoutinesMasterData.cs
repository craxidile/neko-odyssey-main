using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Npc;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineConditionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineRewardEntity.Repo;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.MasterData.Npc.Routines
{
    public class RoutinesMasterData
    {
        public bool Ready { get; private set; }
        public ICollection<Database.Domains.Npc.Entities.RoutineEntity.Models.Routine> Routines { get; private set; }

        public Subject<Unit> OnReady { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
            if (GameRunner.Instance.Core.MasterData.NpcMasterData.DialogsMasterData.Ready)
            {
                ListAll();
                Ready = true;
                OnReady.OnNext(default);
            }
            else
            {
                GameRunner.Instance.Core.MasterData.NpcMasterData.DialogsMasterData.OnReady
                    .Subscribe(_ =>
                    {
                        ListAll();
                        Ready = true;
                        OnReady.OnNext(default);
                    })
                    .AddTo(GameRunner.Instance);
            }
        }

        public void Unbind()
        {
        }

        private void ListAll()
        {
            var dialogs = GameRunner.Instance.Core.MasterData.NpcMasterData.DialogsMasterData.Dialogs;

            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var routineRepo = new RoutineRepo(npcDbContext);
                var routineConditionRepo = new RoutineConditionRepo(npcDbContext);
                var routineRewardRepo = new RoutineRewardRepo(npcDbContext);
                
                Routines = routineRepo.List();

                foreach (var routine in Routines)
                {
                    routine.Conditions = routineConditionRepo.ListByRoutineId(routine.Id);
                    routine.Rewards = routineRewardRepo.ListByRoutineId(routine.Id);
                    if (routine.DialogId == null) continue;
                    routine.Dialog = dialogs.FirstOrDefault(d => d.Id == routine.DialogId.Value);
                }
            }
        }
    }
}