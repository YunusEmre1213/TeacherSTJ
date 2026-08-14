using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Tasks
{
    public interface ITask
    {
        string TaskName { get; }
        IEnumerator ExecuteRoutine(Transform character, Transform target);
        bool Validate(Transform character, Transform target);
        IEnumerator PlayHintRoutine(Transform character, Transform target);
    }
}