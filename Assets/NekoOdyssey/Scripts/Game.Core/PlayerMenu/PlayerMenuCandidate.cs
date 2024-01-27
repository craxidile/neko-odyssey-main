using System;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using UnityEngine;

namespace Assets.NekoOdyssey.Scripts.Game.Core.PlayerMenu
{
    public class PlayerMenuCandidate
    {
        public PlayerMenuAction[] Actions { get; set; }
        public PlayerMenuSite Site { get; set; } = PlayerMenuSite.None;
        public GameObject GameObject { get; set; }
        public bool AutoActive { get; set; } = false;
        public float DistanceFromPlayer { get; set; } = float.MaxValue;

        public void CopyFrom(PlayerMenuCandidate other)
        {
            Actions = other.Actions;
            Site = other.Site;
            GameObject = other.GameObject;
            AutoActive = other.AutoActive;
            DistanceFromPlayer = other.DistanceFromPlayer;
        }

    }
}