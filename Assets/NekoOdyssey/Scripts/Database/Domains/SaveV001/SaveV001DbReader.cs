using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001
{
    public delegate void SaveV001DbReadFunc(SaveV001DbContext dbContext);

    public class SaveV001DbReader
    {
        private SaveV001DbContext _dbContext;

        public void Open()
        {
            _dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true });
        }

        public void Close()
        {
            _dbContext?.Dispose();
        }

        public void Execute(SaveV001DbReadFunc func)
        {
            MainThreadDispatcher.StartCoroutine(ExecuteAsync(func));
        }

        private IEnumerator ExecuteAsync(SaveV001DbReadFunc func)
        {
            func(_dbContext);
            yield return null;
        }
    }
}