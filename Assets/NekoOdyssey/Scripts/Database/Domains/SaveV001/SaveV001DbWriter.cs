using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UniRx;
using UnityEngine;

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

        private SaveV001DbContext _dbContext;
        private bool _running;
        private IDisposable _subscription;

        public void Open()
        {
            _dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = false });
        }

        public void Close()
        {
            if (_dbContext == null) return;
            _dbContext.Dispose();
        }

        public void Add(SaveV001DbQueueFunc func)
        {
            _queue.Enqueue(func);
            if (!_running) Execute();
        }

        private void Execute()
        {
            _subscription = Observable
                .FromCoroutine(ExecuteAsync)
                .Subscribe(Finish);
        }

        private IEnumerator ExecuteAsync()
        {
            _running = true;
            while (_queue.Size > 0)
            {
                var func = _queue.Dequeue();
                func(_dbContext);
            }
            yield return null;
            _running = false;
        }

        private void Finish(Unit _)
        {
            if (_subscription == null)
            {
                _running = false;
                return;
            }

            _subscription.Dispose();
            _subscription = null;
            _running = false;
        }
    }
}