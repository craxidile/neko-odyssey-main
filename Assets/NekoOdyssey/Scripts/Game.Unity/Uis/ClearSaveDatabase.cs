using System;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using Unity.VisualScripting;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Uis
{
    public class ClearSaveDatabase : MonoBehaviour
    {
        private void Start()
        {
            using (new SaveV001DbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        }
    }
}