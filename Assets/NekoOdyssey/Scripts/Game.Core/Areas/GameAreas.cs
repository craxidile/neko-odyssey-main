using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Game.Core.Areas.Cat;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Areas
{
    public class GameAreas
    {
        public List<CatArea> CatAreas { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public CatArea RegisterCatArea(Bounds bounds)
        {
            Debug.Log($">>bounds<< add {bounds}");
            var catArea = new CatArea(bounds);
            CatAreas.Add(catArea);
            return catArea;
        }

        public Vector3 CalculateClosestPoint(Vector3 position)
        {
            return CatAreas
                .Select(ca => ca.Bounds.ClosestPoint(position))
                .OrderBy(p => Vector3.Distance(p, position))
                .FirstOrDefault();
        }

        public CatArea CalculateClosestCatArea(Vector3 position)
        {
            if (CatAreas.Count == 0) return null;
            return CatAreas
                .Select(ca => (ca.Bounds.ClosestPoint(position), ca))
                .OrderBy(tuple => tuple.Item1)
                .First()
                .Item2;
        }
    }
}