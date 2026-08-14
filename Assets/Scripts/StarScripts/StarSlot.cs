using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.UI
{
//Dolu Yýldýzýn Geçiþ Anim
    public class StarSlot : MonoBehaviour
    {
        [Tooltip("Doldurulunca beliren uzaktan gelen sarý yýldýz.")]
        [SerializeField] private RectTransform filledStar;

        [SerializeField] private float flyDistance = 300f;
        [SerializeField] private float flyDuration = 0.5f;

        private Vector2 _restPosition;
        private bool _filled;

        private void Awake()
        {
            _restPosition = filledStar.anchoredPosition;
            filledStar.gameObject.SetActive(false);
        }

        public void FillAnimated()
        {
            if (_filled) return;
            _filled = true;
            StartCoroutine(FlyInRoutine());
        }

        private IEnumerator FlyInRoutine()
        {
            filledStar.gameObject.SetActive(true);

            Vector2 startPos = _restPosition + new Vector2(0f, flyDistance);
            filledStar.anchoredPosition = startPos;
            filledStar.localScale = Vector3.one * 0.3f;

            float elapsed = 0f;
            while (elapsed < flyDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / flyDuration;
                filledStar.anchoredPosition = Vector2.Lerp(startPos, _restPosition, t);
                filledStar.localScale = Vector3.one * Mathf.Lerp(0.3f, 1f, t);
                yield return null;
            }

            filledStar.anchoredPosition = _restPosition;
            filledStar.localScale = Vector3.one;
        }
    }
}