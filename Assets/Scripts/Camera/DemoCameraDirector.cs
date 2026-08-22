using UnityEngine;
using Unity.Cinemachine;

namespace OgretmenGorevSistemi.CameraSystem
{
    public class DemoCameraDirector : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera wideShot;
        [SerializeField] private CinemachineCamera ablaShot;
        [SerializeField] private CinemachineCamera ablaChildShot;
        [SerializeField] private CinemachineCamera anneChildShot;
        [SerializeField] private CinemachineCamera tableShot;

        [SerializeField] private int activePriority = 20;
        [SerializeField] private int inactivePriority = 10;

        private void Awake()
        {
            ShowWide();
        }

        public void ShowWide() => CutTo(wideShot);
        public void ShowAbla() => CutTo(ablaShot);
        public void ShowAblaChild() => CutTo(ablaChildShot);
        public void ShowAnneChild() => CutTo(anneChildShot);
        public void ShowTable() => CutTo(tableShot);

        private void CutTo(CinemachineCamera activeCam)
        {
            SetPriority(wideShot, activeCam == wideShot);
            SetPriority(ablaShot, activeCam == ablaShot);
            SetPriority(ablaChildShot, activeCam == ablaChildShot);
            SetPriority(anneChildShot, activeCam == anneChildShot);
            SetPriority(tableShot, activeCam == tableShot);
        }

        private void SetPriority(CinemachineCamera cam, bool isActive)
        {
            if (cam == null) return;
            cam.Priority = isActive ? activePriority : inactivePriority;
        }
    }
}