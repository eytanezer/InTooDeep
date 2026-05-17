using System;
using DG.Tweening;
using UnityEngine;

namespace Effects
{
    public class Floatings : MonoBehaviour
    {
        [SerializeField] private float amplitude = 0.25f;
        [SerializeField] private float frequency = 1.5f;
        
        private Vector3 _startPosition;


        private void Start()
        {
            _startPosition  = transform.localPosition;
        }
 

        private void Update()
        {
            float newy = _startPosition.y + MathF.Sin(Time.time * frequency) * amplitude;
            transform.localPosition = new Vector3(_startPosition.x, newy, _startPosition.z);
            
            // transform.DOLocalMoveY(transform.localPosition.y + amplitude, frequency).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }
}