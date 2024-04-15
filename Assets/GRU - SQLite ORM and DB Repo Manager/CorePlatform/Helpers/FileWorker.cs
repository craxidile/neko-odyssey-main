using System;
using System.Collections;
using UnityEngine;
using SpatiumInteractive.Libraries.Unity.Platform.Dtos;

namespace SpatiumInteractive.Libraries.Unity.Platform.Helpers
{
    public static class FileWorker
    {
        /// <summary>
        /// usage example: FileWorker.ReadAllBytesInCoroutine(monoBehavior, filePath, (bytes) => {
        ///  //here I run an async method which takes bytes as parameter
        /// }
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filePath"></param>
        /// <param name="onComplete"></param>
        public static void ReadAllBytesInCoroutine(MonoBehaviour context, string filePath, Action<ReadBytesInCoroutineResult> onComplete)
        {
            context.StartCoroutine(ReadFileBytesAndTakeAction(filePath, onComplete));
        }

        private static IEnumerator ReadFileBytesAndTakeAction(string filePath, Action<ReadBytesInCoroutineResult> followingAction)
        {
            WWW reader = null;

            try
            {
                reader = new WWW(filePath);
            }
            catch (Exception exception)
            {
                followingAction.Invoke(ReadBytesInCoroutineResult.Failure(exception));
            }

            while (reader != null && !reader.isDone)
            {
                yield return null;
            }

            followingAction.Invoke(ReadBytesInCoroutineResult.Success(reader.bytes));
        }
    }
}
