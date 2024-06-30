using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001
{
    public delegate void SaveV001DbQueueFunc(SaveV001DbContext dbContext);

    internal class SaveV001DbQueue
    {
        private List<SaveV001DbQueueFunc> _funcList = new();

        public int Size => _funcList.Count;

        public void Enqueue(SaveV001DbQueueFunc func)
        {
            _funcList.Add(func);
        }

        public SaveV001DbQueueFunc Dequeue()
        {
            if (_funcList.Count == 0) return null;

            var firstFunc = _funcList.First();
            _funcList.Remove(firstFunc);
            return firstFunc;
        }
    }

    public class SaveV001DbWriter
    {
        private readonly SaveV001DbQueue _queue = new();

        private bool _running;
        private SaveV001DbContext _dbContext;

        public void Add(SaveV001DbQueueFunc func)
        {
            _queue.Enqueue(func);
            if (!_running) Execute();
        }

        private void Execute()
        {
            _running = true;
            Debug.Log(">>db_writer<< running true");
            MainThreadDispatcher.StartCoroutine(ExecuteAsync());
        }

        private IEnumerator ExecuteAsync()
        {
            var func = _queue.Dequeue();
            if (func == null)
            {
                _running = false;
                Debug.Log(">>db_writer<< running false");
                yield break;
            }

            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = false }))
            {
                func(dbContext);
            }

            yield return null;

            MainThreadDispatcher.StartCoroutine(ExecuteAsync());
        }
    }
}