// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Threading.Tasks;
using UnityEngine;

namespace ThreeStudio.IPFS.Internal
{
    public static class AsyncUtil
    {
        /// <summary>
        /// Wraps an AsyncOperation object inside an awaitable Task.
        /// </summary>
        /// <param name="asyncOperation"></param>
        /// <returns></returns>
        public static Task AsIpfsTask(this AsyncOperation asyncOperation)
        {
            var task = new TaskCompletionSource<bool>();
            asyncOperation.completed += _ => { task.SetResult(true); };
            return task.Task;
        }
    }
}