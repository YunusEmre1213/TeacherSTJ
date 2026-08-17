using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Character
{
    public class BonePoseController : MonoBehaviour
    {
        [System.Serializable]
        public class BoneTarget
        {
            public Transform bone;
            public Vector3 targetLocalEulerAngles;
        }

        [Tooltip("Bu poza, PlayGestureTask'ýn Pose Name alanýndan hangi isimle eriþileceði.")]
        [SerializeField] private string poseName = "DurIsareti";

        [SerializeField] private BoneTarget[] bones;
        [SerializeField] private float blendDuration = 0.3f;

        [Tooltip("Poz tutulurken hafif bir yukarý-aþaðý sallanma eklensin mi")]
        [SerializeField] private bool oscillate;
        [SerializeField] private float oscillateAmplitude = 8f;
        [SerializeField] private float oscillateSpeed = 6f;

        public string PoseName => poseName;

        private Quaternion[] _restRotations;
        private Quaternion[] _appliedRotations;
        private bool _isPosing;

        private void Awake()
        {
            _restRotations = new Quaternion[bones.Length];
            _appliedRotations = new Quaternion[bones.Length];
            for (int i = 0; i < bones.Length; i++)
                _restRotations[i] = bones[i].bone.localRotation;
        }

        private void LateUpdate()
        {
            if (!_isPosing) return;
            for (int i = 0; i < bones.Length; i++)
                bones[i].bone.localRotation = _appliedRotations[i];
        }

        public IEnumerator PlayPose(float holdDuration)
        {
            for (int i = 0; i < bones.Length; i++)
                _appliedRotations[i] = bones[i].bone.localRotation;
            _isPosing = true;

            yield return BlendTo(true);

            if (oscillate)
                yield return HoldWithOscillation(holdDuration);
            else
                yield return new WaitForSeconds(holdDuration);

            yield return BlendTo(false);

            _isPosing = false;
        }

        private IEnumerator HoldWithOscillation(float duration)
        {
            Debug.Log($"[BonePoseController:{poseName}] HoldWithOscillation baþladý. Süre: {duration}, Speed: {oscillateSpeed}, Amplitude: {oscillateAmplitude}, Bone sayýsý: {bones.Length}");

            float elapsed = 0f;
            float logTimer = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float offset = Mathf.Sin(elapsed * oscillateSpeed) * oscillateAmplitude;

                logTimer += Time.deltaTime;
                if (logTimer >= 0.3f)
                {
                    logTimer = 0f;
                    Debug.Log($"[BonePoseController:{poseName}] elapsed: {elapsed:F2}, offset: {offset:F2}");
                }

                for (int i = 0; i < bones.Length; i++)
                {
                    Quaternion basePose = Quaternion.Euler(bones[i].targetLocalEulerAngles);
                    _appliedRotations[i] = basePose * Quaternion.Euler(offset, 0f, 0f);
                }
                yield return null;
            }
        }

        private IEnumerator BlendTo(bool toPose)
        {
            var from = new Quaternion[bones.Length];
            var to = new Quaternion[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                from[i] = _appliedRotations[i];
                to[i] = toPose ? Quaternion.Euler(bones[i].targetLocalEulerAngles) : _restRotations[i];
            }

            float elapsed = 0f;
            while (elapsed < blendDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / blendDuration;
                for (int i = 0; i < bones.Length; i++)
                    _appliedRotations[i] = Quaternion.Slerp(from[i], to[i], t);
                yield return null;
            }

            for (int i = 0; i < bones.Length; i++)
                _appliedRotations[i] = to[i];
        }
    }
}