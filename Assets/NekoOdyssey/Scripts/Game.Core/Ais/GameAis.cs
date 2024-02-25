using System.Collections.Generic;
using System.Reflection;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat;
using NekoOdyssey.Scripts.Models;

namespace NekoOdyssey.Scripts.Game.Core.Ais
{
    public class GameAis
    {
        public List<CatAi> CatAis { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
            foreach (var catAi in CatAis)
                catAi.Unbind();
        }

        public CatAi RegisterCatAi(CatProfile profile)
        {
            var catAi = new CatAi(profile);
            catAi.Bind();
            CatAis.Add(catAi);
            return catAi;
        }
        
    }
}