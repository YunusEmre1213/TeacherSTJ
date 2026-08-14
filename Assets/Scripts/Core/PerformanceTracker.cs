using UnityEngine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.Tracking
{
    public class PerformanceTracker : MonoBehaviour
    {
        private int _totalStars;
        private float _startTime;
        private bool _isTracking;

        private void OnEnable()
        {
            GameEvents.OnPlayerConfirmedReady += HandlePlayerAttemptStarted;

            GameEvents.OnStarsEarned += HandleStarsEarned;

            GameEvents.OnAllStepsCompleted += HandleAllStepsCompleted;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerConfirmedReady -= HandlePlayerAttemptStarted;
            GameEvents.OnStarsEarned -= HandleStarsEarned;
            GameEvents.OnAllStepsCompleted -= HandleAllStepsCompleted;
        }

        private void HandlePlayerAttemptStarted()
        {
            _totalStars = 0;
            _startTime = Time.time; 
            _isTracking = true;

            Debug.Log("[PerformanceTracker] Oyuncu denemesi baþladý, süre ölçümü devrede.");
        }

        private void HandleStarsEarned(int stars)
        {
            if (_isTracking)
            {
                _totalStars += stars; 
            }
        }

        private void HandleAllStepsCompleted()
        {
            if (!_isTracking) return;

            _isTracking = false;

           
            float totalDuration = Time.time - _startTime;

            
            Debug.Log($"<color=#00FF00><b>[GÖREV TAMAMLANDI]</b></color>\n" +
                      $"Kazanýlan Toplam Yýldýz: <b>{_totalStars}</b>\n" +
                      $"Geçen Süre: <b>{totalDuration:F2} saniye</b>");
        }
    }
}