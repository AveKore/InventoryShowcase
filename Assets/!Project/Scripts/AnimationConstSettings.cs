using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts
{
    public static class AnimationConstSettings
    {
        public const float ScaleAnimationDuration = 0.1f;
        public const float ScaleAnimationEndValue = 1.1f;
        public const float ScaleAnimationStartValue = 1f;
        
        public static void AnimateSelect(Transform transform)
        {
            var seq = DOTween.Sequence();
            
            seq.Append(transform.DOScale(ScaleAnimationEndValue, ScaleAnimationDuration));
            seq.Append(transform.DOScale(ScaleAnimationStartValue, ScaleAnimationDuration));
        }
    }
}