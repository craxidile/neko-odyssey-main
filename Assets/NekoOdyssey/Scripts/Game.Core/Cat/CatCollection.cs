using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NekoOdyssey.Scripts.Models;

namespace NekoOdyssey.Scripts.Game.Core.Cat
{
    public class CatCollection
    {
        public List<Cat> Cats { get; } = new();
        public string CurrentCatCode { get; set; }
        public Cat CurrentCat => GetCatByCode(CurrentCatCode);

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
            foreach (var cat in Cats)
                cat.Unbind();
        }

        [CanBeNull]
        public Cat GetCatByCode(string code)
        {
            return Cats.FirstOrDefault(cat => cat.Code == code);
        }

        public Cat RegisterCat(string code)
        {
            var cat = new Cat(code);
            cat.Bind();
            Cats.Add(cat);
            return cat;
        }
    }
}