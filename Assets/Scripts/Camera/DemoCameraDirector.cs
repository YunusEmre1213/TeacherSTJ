using UnityEngine;
using Unity.Cinemachine;

namespace OgretmenGorevSistemi.CameraSystem
{
    public class DemoCameraDirector : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera wideShot;
        [SerializeField] private CinemachineCamera ablaShot;
        [SerializeField] private CinemachineCamera anneChildShot;

        [SerializeField] private int activePriority = 20;
        [SerializeField] private int inactivePriority = 10;

        private void Awake()
        {
            ShowWide();
        }

        public void ShowWide() => CutTo(wideShot);
        public void ShowAbla() => CutTo(ablaShot);
        public void ShowAnneChild() => CutTo(anneChildShot);

        private void CutTo(CinemachineCamera activeCam)
        {
            SetPriority(wideShot, activeCam == wideShot);
            SetPriority(ablaShot, activeCam == ablaShot);
            SetPriority(anneChildShot, activeCam == anneChildShot);
        }

        private void SetPriority(CinemachineCamera cam, bool isActive)
        {
            if (cam == null) return;
            cam.Priority = isActive ? activePriority : inactivePriority;
        }
    }
}