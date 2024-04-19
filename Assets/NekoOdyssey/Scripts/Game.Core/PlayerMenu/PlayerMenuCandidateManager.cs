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
            // if (Candidates.Any(c => c.Site == candidate.Site))
            // {
            //     var existingCandidate = Candidates.First(c => c.Site == candidate.Site);
            //     existingCandidate.CopyFrom(candidate);
            // }
            if (Candidates.Any(c => c.SiteName == candidate.SiteName))
            {
                var existingCandidate = Candidates.First(c => c.SiteName == candidate.SiteName);
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
            // if (!Candidates.Any((c => c.Site == candidate.Site))) return;
            // Candidates.Remove(Candidates.First(c => c.Site == candidate.Site));
            if (!Candidates.Any((c => c.SiteName == candidate.SiteName))) return;
            Candidates.Remove(Candidates.First(c => c.SiteName == candidate.SiteName));
            //// var currentPlayerMenu = GameRunner.Instance.Core.PlayerMenu;
            //// if (currentPlayerMenu.Site == candidate.Site) currentPlayerMenu.Reset();
            Debug.Log($">>set_site_active<< remove");
            var currentPlayerMenu = GameRunner.Instance.Core.PlayerMenu;
            // if (currentPlayerMenu.Site == candidate.Site)
            if (currentPlayerMenu.SiteName == candidate.SiteName)
            {
                // Debug.Log($">>set_site_active<< {currentPlayerMenu.Site} false");
                // currentPlayerMenu.SetSiteActive(currentPlayerMenu.Site, false);
                Debug.Log($">>set_site_active<< {currentPlayerMenu.SiteName} false");
                currentPlayerMenu.SetSiteNameActive(currentPlayerMenu.SiteName, false);
            }

            ReorderAndSetPlayerMenu();
        }

        private void ReorderAndSetPlayerMenu()
        {
            ////Debug.Log($">>set_site_active<< reorder");
            var candidates = Candidates.OrderBy(c => c.DistanceFromPlayer);
            var selectedCandidate = candidates.FirstOrDefault();
            if (selectedCandidate == null) return;
            var currentPlayerMenu = GameRunner.Instance.Core.PlayerMenu;
            // if (selectedCandidate.Site == currentPlayerMenu.Site) return;
            if (selectedCandidate.SiteName == currentPlayerMenu.SiteName) return;
            //// Debug.Log($">>current_site<< {currentPlayerMenu.Site} {selectedCandidate.Site}");
            //// currentPlayerMenu.SetSite(selectedCandidate.Site);
            //// currentPlayerMenu.SetActive(false);
            // Debug.Log($">>set_site_active<< {currentPlayerMenu.Site} false");
            // currentPlayerMenu.SetSiteActive(currentPlayerMenu.Site, false);
            Debug.Log($">>set_site_active<< {currentPlayerMenu.SiteName} false");
            currentPlayerMenu.SetSiteNameActive(currentPlayerMenu.SiteName, false);
            currentPlayerMenu.GameObject = selectedCandidate.GameObject;
            // Debug.Log($">>set_site_active<< {selectedCandidate.Site} true");
            Debug.Log($">>set_site_active<< {selectedCandidate.SiteName} true");
            currentPlayerMenu.SetActions(selectedCandidate.Actions);
            if (selectedCandidate.AutoActive)
            {
                Debug.Log($">>site_name_active<< {selectedCandidate.SiteName}");
                // currentPlayerMenu.SetSiteActive(selectedCandidate.Site, true);
                currentPlayerMenu.SetSiteNameActive(selectedCandidate.SiteName, true);
            }
        }
    }
}