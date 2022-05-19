using System;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace Code
{
    public class JobTransform:MonoBehaviour
    {
        private TransformAccessArray _transformAccessArray;
        private Transform[] _transforms;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Vector3 _rotationAxis;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _count;
        [SerializeField] private bool _multythreading;
        private RotateJobTransform _rotateJob;

        private void Start()
        {
            _transforms = new Transform[_count];
            for (int i = 0; i < _count; i++)
            {
                _transforms[i] = Instantiate(_prefab,
                        new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5)), 
                        Quaternion.identity)
                    .transform;
            }
            _transformAccessArray = new TransformAccessArray(_transforms);
            _rotateJob = new RotateJobTransform()
            {
                Speed = _rotationSpeed,
                Direction = _rotationAxis,
                DeltaTime = Time.deltaTime
            };
        }

        private void OnDestroy()
        {
            _transformAccessArray.Dispose();
        }

        private void Update()
        {
            if (_multythreading)
                RotateJob();
            else
                RotateMainThread();
        }

        private void RotateMainThread()
        {
            for (int i = 0; i < _transforms.Length; i++)
            {
                _transforms[i].rotation = Quaternion.Euler(_rotationAxis * _rotationSpeed*Time.deltaTime)*_transforms[i].rotation;
            }
        }

        private void RotateJob()
        {
            _rotateJob.DeltaTime = Time.deltaTime;
            var handle = _rotateJob.Schedule(_transformAccessArray);
            handle.Complete();
        }
    }
    
    public struct RotateJobTransform:IJobParallelForTransform
    {
        public float Speed;
        public Vector3 Direction;
        public float DeltaTime;
        public void Execute(int index, TransformAccess transform)
        {
            transform.rotation = Quaternion.Euler(Direction * Speed*DeltaTime)*transform.rotation;
        }
    }
}