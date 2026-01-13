using UnityEngine;
using UnityEngine.UI;

namespace KingGuardians.UI
{
    /// <summary>
    /// Simple energy bar view. Uses Slider for MVP.
    /// </summary>
    public sealed class EnergyBarView : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public void Set(int current, int max)
        {
            if (slider == null) return;

            slider.maxValue = max;
            slider.value = current;
        }
    }
}
