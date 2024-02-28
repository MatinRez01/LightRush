using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;
        [SerializeField] private float shakeMagnitude = 0.5f;
        [SerializeField] private float shakeDuration = 0.1f;
        [SerializeField, Range(0,1)] private float distortionMagnitude;
        [SerializeField, Range(0,1)] private float glitchMagnitude;
        [SerializeField] private float blurTime;
        [SerializeField] private float blurAmount;

        private MobilePostProcessing postFx;
        private void OnEnable()
        {
            instance = this;
            postFx = GetComponent<MobilePostProcessing>();
        }
        

        IEnumerator _Shake()
        {
            Vector3 initialPosition = transform.localPosition;

            float elapsedTime = 0.0f;
            while (elapsedTime < shakeDuration)
            {
                Vector3 newPosition = UnityEngine.Random.insideUnitSphere * shakeMagnitude;
                transform.localPosition = newPosition;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = initialPosition;
        }
        public void Shake()
        {
            StartCoroutine(_Shake());
    }

        public void StartGame()
        {
            transform.DOLocalMove(Vector3.zero, 2.5f).SetEase(Ease.OutBack);
        }

        public void GameLost()
        {
            Shake();
            postFx.Blur = true;
            Sequence s1 = DOTween.Sequence();
            s1.Append(DOVirtual.Float(0, blurAmount, blurTime, v => postFx.BlurAmount = v));
            s1.Play();

        }

        public void OnPlayerDamage(float invincTime)
        {
            Shake();
            float fxRaiseT = invincTime / 2;
            float fxFallT = invincTime / 2;
            postFx.ChromaticAberration = true;
            postFx.Distortion = true;
            Sequence s1 = DOTween.Sequence();
            s1.Append(DOVirtual.Float(0, glitchMagnitude, fxRaiseT, v => postFx.GlitchAmount = v)).
                Append(DOVirtual.Float(glitchMagnitude, 0f, fxFallT, v => postFx.GlitchAmount = v)).
                OnComplete(() => { postFx.ChromaticAberration = false; });
            Sequence s2 = DOTween.Sequence();
            s2.Append(DOVirtual.Float(0, distortionMagnitude, fxRaiseT, v => postFx.LensDistortion = v)).
                Append(DOVirtual.Float(distortionMagnitude, 0f, fxFallT, v => postFx.LensDistortion = v)).
                OnComplete(() => { postFx.Distortion = false; });
            s1.Play();
            s2.Play();
        }

    }
}

    
