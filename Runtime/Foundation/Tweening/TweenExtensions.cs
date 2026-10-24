using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Tweening
{
    public static class TweenExtensions
    {
        public static UniTask TweenPosition(this Transform transform, Vector3 to, TweenSettings settings, CancellationToken cancellationToken = default)
        {
            return Run(transform, cancellationToken, token => Tween.Vector3(transform.position, to, settings, x => transform.position = x, token));
        }

        public static UniTask TweenLocalPosition(this Transform transform, Vector3 to, TweenSettings settings, CancellationToken cancellationToken = default)
        {
            return Run(transform, cancellationToken, token => Tween.Vector3(transform.localPosition, to, settings, x => transform.localPosition = x, token));
        }

        public static UniTask TweenLocalRotation(this Transform transform, Quaternion to, TweenSettings settings, CancellationToken cancellationToken = default)
        {
            return Run(transform, cancellationToken, token => Tween.Quaternion(transform.localRotation, to, settings, x => transform.localRotation = x, token));
        }

        public static UniTask TweenLocalScale(this Transform transform, Vector3 to, TweenSettings settings, CancellationToken cancellationToken = default)
        {
            return Run(transform, cancellationToken, token => Tween.Vector3(transform.localScale, to, settings, x => transform.localScale = x, token));
        }

        public static UniTask TweenAnchoredPosition(this RectTransform transform, Vector2 to, TweenSettings settings, CancellationToken cancellationToken = default)
        {
            return Run(transform, cancellationToken, token => Tween.Vector2(transform.anchoredPosition, to, settings, x => transform.anchoredPosition = x, token));
        }

        public static UniTask TweenAlpha(this CanvasGroup group, float to, TweenSettings settings, CancellationToken cancellationToken = default)
        {
            return Run(group, cancellationToken, token => Tween.Float(group.alpha, to, settings, x => group.alpha = x, token));
        }

        public static UniTask TweenColor(this Graphic graphic, Color to, TweenSettings settings, CancellationToken cancellationToken = default)
        {
            return Run(graphic, cancellationToken, token => Tween.Color(graphic.color, to, settings, x => graphic.color = x, token));
        }

        public static UniTask TweenAlpha(this Graphic graphic, float to, TweenSettings settings, CancellationToken cancellationToken = default)
        {
            Color target = graphic.color;
            target.a = to;
            return graphic.TweenColor(target, settings, cancellationToken);
        }

        private static async UniTask Run(Component component, CancellationToken cancellationToken, Func<CancellationToken, UniTask> tween)
        {
            CancellationToken destroyToken = component.GetCancellationTokenOnDestroy();
            if (!cancellationToken.CanBeCanceled)
            {
                await tween(destroyToken);
                return;
            }

            using (CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, cancellationToken))
            {
                await tween(linked.Token);
            }
        }
    }
}
