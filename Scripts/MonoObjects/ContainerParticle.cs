using System;
using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace MonoObjects
{
    public class ContainerParticle : MonoPooled
    {
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private int baseRateOverTime = 100;
        [SerializeField] private float sizeWeight = 0.7f;
        
        
        private IEnumerator Initialize()
        {
            yield return Wait.EndOfFrame;
            particleSystem.Play();
            yield return Wait.ForSeconds(2f);
            ReturnToPool();
        }

        public void Activate(int row, int column, Vector3 position)
        {
            var shape = particleSystem.shape;
            var emmission = particleSystem.emission;
            emmission.rateOverTimeMultiplier = baseRateOverTime + ((row * column) * sizeWeight);
            shape.scale = new Vector3(column, row, 0);
            transform.position = position +  Vector3.up*1f;
            StartCoroutine(nameof(Initialize));
        }
    }
}