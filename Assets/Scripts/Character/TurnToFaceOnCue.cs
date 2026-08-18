using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Character
{
    public class TurnToFaceOnCue : MonoBehaviour
    {
        [SerializeField] private float turnSpeed = 60f;

        public IEnumerator TurnToFaceRoutine(Transform target)
        {
            if (target == null) yield break;

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) yield break;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

            while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}