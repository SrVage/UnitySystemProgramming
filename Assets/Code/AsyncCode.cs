using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Code
{
    public class AsyncCode:MonoBehaviour
    {
        private async void Start()
        {
            //Application.targetFrameRate = 30;
            var tokenSource = new CancellationTokenSource();
            var task1 = Task1(tokenSource.Token);
            var task2 = Task2(tokenSource.Token);
            var task = WhatTaskFasterAsync(tokenSource.Token, task1, task2);
            await task;
            Debug.Log(task.Result);
            tokenSource.Cancel();
        }

        public async Task<bool> WhatTaskFasterAsync(CancellationToken token, Task task1, Task task2)
        {
            await Task.WhenAny(task1, task2);
            return task1.IsCompleted;
        }

        private async Task Task1(CancellationToken token)
        {
            await Task.Delay(1000, token);
            Debug.Log("Task1 is completed");
        }

        private async Task Task2(CancellationToken token)
        {
            for (int i = 0; i < 60; i++)
            {
                if (token.IsCancellationRequested)
                    return;
                await Task.Yield();
            }
            Debug.Log("Task2 is completed");
        }
    }
}