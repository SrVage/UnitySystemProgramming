using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = System.Random;

namespace Code
{
    public class JobSystem:MonoBehaviour
    {
        private NativeArray<int> _array;
        [SerializeField] private int _count;
        [SerializeField] private JobParallelFor _jobParallelFor;

        private void Start()
        {
            _array = new NativeArray<int>(_count, Allocator.Persistent);
            var createRandom = new CreateRandomMassive()
            {
                Array = _array,
                Count = _count
            };
            var job = new Job(_array);
            var firstHande = createRandom.Schedule();
            var secondSchedule = job.Schedule(firstHande);
            secondSchedule.Complete();
            PrintMassive();
            _array.Dispose();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (var vector in _jobParallelFor.Execute())
                {
                    Debug.Log(vector);
                }
            }
        }

        private void PrintMassive()
        {
            foreach (var element in _array)
            {
                Debug.Log(element);
            }
            Debug.Log("_____________");
        }
    }

    public struct Job:IJob
    {
        public NativeArray<int> Array;
        public Job(NativeArray<int> array)
        {
            Array = array;
        }
        public void Execute()
        {
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i] > 10)
                    Array[i] = 0;
            }
        }
    }
    
    public struct CreateRandomMassive:IJob
    {
        public NativeArray<int> Array;
        public int Count;
        public void Execute()
        {
            Random random = new Random();
            for (int i = 0; i < Count; i++)
            {
                Array[i] = random.Next(50);
            }
        }
    }
}