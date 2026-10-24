using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PhotoMode
{
    public class CustomInputProvider : MonoBehaviour, AxisState.IInputAxisProvider
    {
        [SerializeField] private InputActionReference XYAxisAction;
        public bool active = true;

        public float GetAxisValue(int axis)
        {
            if (!active || !isActiveAndEnabled || XYAxisAction == null || XYAxisAction.action == null)
                return 0;
            var value = XYAxisAction.action.ReadValue<Vector2>();
            return axis == 0 ? value.x : axis == 1 ? value.y : 0;
        }
    }
}
