using UnityEngine;

namespace KingGuardians.Core
{
    public interface ICardDeployController
    {
        void SelectSlot(int slotIndex);
        bool TryDeployAtScreen(Vector2 screenPos);
    }
}
