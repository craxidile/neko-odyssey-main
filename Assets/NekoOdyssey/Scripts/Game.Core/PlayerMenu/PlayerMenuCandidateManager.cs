using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts;
using NekoOdyssey.Scripts.Game.Unity;

namespace Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu
{
    public class PlayerMenuCandidateManager
    {
        
        public List<PlayerMenuCandidate> Candidates { get; } = new();

        public void Add(PlayerMenuCandidate candidate)
        {
            if (Candidates.Any(c => c.Site == candidate.Site))
            {
                var existingCandidate = Candidates.First(c => c.Site == candidate.Site);
                existingCandidate.CopyFrom(candidate);
            }
            else
            {
                Candidates.Add(candidate);
            }

            ReorderAndSetPlayerMenu();
        }

        public void Remove(PlayerMenuCandidate candidate)
        {
            if (!Candidates.Any((c => c.Site == candidate.Site))) return;
            Candidates.Remove(Candidates.First(c => c.Site == candidate.Site));
            var currentPlayerMenu = GameRunner.Instance.Core.PlayerMenu;
            if (currentPlayerMenu.Site == candidate.Site) currentPlayerMenu.Reset();
            ReorderAndSetPlayerMenu();
        }

        private void ReorderAndSetPlayerMenu()
        {
            var candidates = Candidates.OrderBy(c => c.DistanceFromPlayer);
            var selectedCandidate = candidates.FirstOrDefault();
            if (selectedCandidate == null) return;
            var currentPlayerMenu = GameRunner.Instance.Core.PlayerMenu;
            if (selectedCandidate.Site == currentPlayerMenu.Site) return;
            currentPlayerMenu.SetActive(false);
            currentPlayerMenu.Site = selectedCandidate.Site;
            currentPlayerMenu.GameObject = selectedCandidate.GameObject;
            currentPlayerMenu.SetActions(selectedCandidate.Actions);
            if (selectedCandidate.AutoActive) currentPlayerMenu.SetActive(true);
        }
        
    }
}