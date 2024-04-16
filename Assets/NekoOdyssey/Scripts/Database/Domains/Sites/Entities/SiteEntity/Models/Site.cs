using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteSceneEntity;
using NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteSceneEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;
using UnityEngine;

namespace NekoOdyssey.Scripts.Database.Domains.Sites.Entities.SiteEntity.Models
{
    [Serializable]
    public class Site : EntityBase<int>, IAggregateRoot
    {
        [NotNull] [Indexed] public string Name { get; set; }

        public string Description { get; set; }

        public float? PlayerX { get; set; }

        public float? PlayerY { get; set; }

        public float? PlayerZ { get; set; }

        public string FacingDirection { get; set; }

        [ForeignKey(typeof(Site))] [Indexed] public int? NextSiteId { get; set; }

        [Indexed] public string NextSiteName { get; set; }

        [Ignore] public virtual Site NextSite { get; set; }

        [Ignore]
        public PlayerFacing Facing
        {
            get
            {
                if (FacingDirection == null) return PlayerFacing.None;
                return (PlayerFacing)FacingDirection[0];
            }
        }

        [Ignore] public virtual ICollection<SiteScene> Scenes { get; set; }

        public Site()
        {
        }

        public Site(string name)
        {
            Name = name;
        }

        public Site(
            string name,
            Vector3 position,
            string facing
        ) : this(name)
        {
            PlayerX = position.x;
            PlayerY = position.y;
            PlayerZ = position.z;
            FacingDirection = facing;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"\n\tname: {Id}");
            builder.Append($"\n\tname: {Name}");
            builder.Append($"\n\tdescription: {Description}");
            builder.Append($"\n\tnext_site_id: {NextSiteId}");
            builder.Append($"\n\tnext_site_name: {NextSiteName}");

            if (Scenes.Count != 0)
            {
                builder.Append("\n\tscenes:");
                foreach (var scene in Scenes)
                {
                    builder.Append($"\n\t\tid: {scene.Id}");
                    builder.Append($"\n\t\tname: {scene.Name}");
                    builder.Append($"\n\t\tactive_game_object: {scene.ActiveGameObject}");
                }
            }

            if (NextSite != null)
            {
                builder.Append("\n\tnext_site:");
                builder.Append($"\n\t\tid: {NextSite.Id}");
                builder.Append($"\n\t\tname: {NextSite.Name}");
            }

            return builder.ToString();
        }
    }
}