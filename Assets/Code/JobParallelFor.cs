using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = System.Random;

namespace Code
{
    public class JobParallelFor:MonoBehaviour
    {
        private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _velocities;
        private NativeArray<Vector3> _finalPositions;
        [SerializeField] private int _count;

        public IEnumerable<Vector3> Execute()
        {
            _positions = new NativeArray<Vector3>(_count, Allocator.Persistent);
            _velocities = new NativeArray<Vector3>(_count, Allocator.Persistent);
            _finalPositions = new NativeArray<Vector3>(_count, Allocator.Persistent);
            var worker1 = new CreateRandomVector(_positions).Schedule(_count, 5);
            var worker2 = new CreateRandomVector(_velocities).Schedule(_count, 5);
            JobHandle.CompleteAll(ref worker1, ref worker2);
            var sumWorker = new VectorSummary(_positions, _velocities, _finalPositions).Schedule(_count, 5);
            sumWorker.Complete();
            _velocities.Dispose();
            _positions.Dispose();
            foreach (var finalPosition in _finalPositions)
            {
                yield return finalPosition;
            }
        }

        private void OnDestroy()
        {
            if (_finalPositions.IsCreated)
                _finalPositions.Dispose();
        }

        public struct CreateRandomVector:IJobParallelFor
        {
            private NativeArray<Vector3> _array;
            public CreateRandomVector(NativeArray<Vector3> array)
            {
                _array = array;
            }

            public void Execute(int index)
            {
                Random random= new Random();

                _array[index] = new Vector3((float) random.NextDouble(), (float) random.NextDouble(),
                    (float) random.NextDouble());
            }
        }
        
        public struct VectorSummary:IJobParallelFor
        {
            [ReadOnly] private NativeArray<Vector3> _positions;
            [ReadOnly] private NativeArray<Vector3> _velocities;
            [WriteOnly] private NativeArray<Vector3> _result;

            public VectorSummary(NativeArray<Vector3> positions, NativeArray<Vector3> velocities, NativeArray<Vector3> result)
            {
                _positions = positions;
                _velocities = velocities;
                _result = result;
            }
            public void Execute(int index)
            {
                _result[index] = _positions[index] + _velocities[index];
            }
        }
    }
}