using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NekoOdyssey.Scripts;
using NekoOdyssey.Scripts.Game.Unity;
using UnityEngine;

namespace Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu
{
    public class PlayerMenuCandidateManager
    {
        
        public List<PlayerMenuCandidate> Candidates { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }
        
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
            // var currentPlayerMenu = GameRunner.Instance.Core.PlayerMenu;
            // if (currentPlayerMenu.Site == candidate.Site) currentPlayerMenu.Reset();
            Debug.Log($">>set_site_active<< remove");
            var currentPlayerMenu = GameRunner.Instance.Core.PlayerMenu;
            if (currentPlayerMenu.Site == candidate.Site)
            {
                Debug.Log($">>set_site_active<< {currentPlayerMenu.Site} false");
                currentPlayerMenu.SetSiteActive(currentPlayerMenu.Site, false);
            }
            ReorderAndSetPlayerMenu();
        }

        private void ReorderAndSetPlayerMenu()
        {
            Debug.Log($">>set_site_active<< reorder");
            var candidates = Candidates.OrderBy(c => c.DistanceFromPlayer);
            var selectedCandidate = candidates.FirstOrDefault();
            if (selectedCandidate == null) return;
            var currentPlayerMenu = GameRunner.Instance.Core.PlayerMenu;
            if (selectedCandidate.Site == currentPlayerMenu.Site) return;
            // Debug.Log($">>current_site<< {currentPlayerMenu.Site} {selectedCandidate.Site}");
            // currentPlayerMenu.SetSite(selectedCandidate.Site);
            // currentPlayerMenu.SetActive(false);
            Debug.Log($">>set_site_active<< {currentPlayerMenu.Site} false");
            currentPlayerMenu.SetSiteActive(currentPlayerMenu.Site, false);
            currentPlayerMenu.GameObject = selectedCandidate.GameObject;
            Debug.Log($">>set_site_active<< {selectedCandidate.Site} true");
            currentPlayerMenu.SetActions(selectedCandidate.Actions);
            if (selectedCandidate.AutoActive)
            {
                currentPlayerMenu.SetSiteActive(selectedCandidate.Site, true);
            }
        }
        
    }
}