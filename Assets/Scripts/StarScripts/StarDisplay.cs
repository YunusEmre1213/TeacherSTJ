using System.Collections.Generic;
using UnityEngine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.UI
{
    public class StarDisplay : MonoBehaviour
    {
        [SerializeField] private Transform starContainer;
        [SerializeField] private GameObject starSlotPrefab;

        private readonly List<StarSlot> _slots = new List<StarSlot>();
        private int _filledCount;

        private void OnEnable()
        {
            GameEvents.OnTotalStepsKnown += BuildSlots;
            GameEvents.OnStarsEarned += FillNextStars;
        }

        private void OnDisable()
        {
            GameEvents.OnTotalStepsKnown -= BuildSlots;
            GameEvents.OnStarsEarned -= FillNextStars;
        }

        private void BuildSlots(int totalSteps)
        {
            int totalStars = totalSteps * 3;
            for (int i = 0; i < totalStars; i++)
            {
                GameObject go = Instantiate(starSlotPrefab, starContainer);
                _slots.Add(go.GetComponent<StarSlot>());
            }
        }

        private void FillNextStars(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_filledCount >= _slots.Count) break;
                _slots[_filledCount].FillAnimated();
                _filledCount++;
            }
        }
    }
}