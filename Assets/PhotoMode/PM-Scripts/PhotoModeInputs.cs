using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using PhotoMode;

namespace PhotoMode
{

    [System.Serializable] public class PhotoModeDeviceEvent : UnityEvent<InputDevice> { }
    [System.Serializable] public class PhotoModeButtonEvent : UnityEvent { }
    [System.Serializable] public class PhotoModeAxisEvent : UnityEvent<Vector2> { }
    [System.Serializable] public class PhotoModeIntEvent : UnityEvent<int> { }

    public class PhotoModeInputs : MonoBehaviour
    {
        private InputDevice currentInputDevice;

        //References for dynamically changing the UI input icons
        [Header("Device Configuration")]
        [SerializeField] private InfoBarController infoBarController;

        //Reference for the Action Asset and all of needed actions
        [Space]
        [Header("Input System Actions - Photo Mode")]
        [SerializeField] private InputActionAsset photoModeActionAsset;
        public InputActionReference submitAction;
        public InputActionReference navigationAction;
        public InputActionReference lookAction;
        public InputActionReference pauseAction;
        public InputActionReference resetAction;
        public InputActionReference toggleInterfaceAction;
        public InputActionReference toggleGridAction;

        [Space]
        [Header("Input System Actions - Sticker Mode")]
        public InputActionReference swapStickerAction;
        public InputActionReference deleteStickerAction;
        public InputActionReference flipStickerAction;

        [Space]
        [Header("Input System Actions - Modifiers")]
        public InputActionReference modifier1Action;
        public InputActionReference modifier2Action;

        //Events
        [HideInInspector] public PhotoModeDeviceEvent DeviceChangeEvent;
        [HideInInspector] public PhotoModeButtonEvent SubmitEvent;
        [HideInInspector] public PhotoModeButtonEvent PauseEvent;
        [HideInInspector] public PhotoModeButtonEvent ResetEvent;
        [HideInInspector] public PhotoModeButtonEvent ToggleInterfaceEvent;
        [HideInInspector] public PhotoModeButtonEvent ToggleGridEvent;
        [HideInInspector] public PhotoModeButtonEvent SwapStickerEvent;
        [HideInInspector] public PhotoModeButtonEvent DeleteStickerEvent;
        [HideInInspector] public PhotoModeButtonEvent FlipStickerEvent;

        //Input results
        [Header("Input Values")]
        public Vector2 moveAxis;
        public Vector2 lookAxis;
        public float modifier1_value;
        public float modifier2_value;

        private readonly System.Collections.Generic.List<InputAction> ownedActions = new System.Collections.Generic.List<InputAction>();

        private void OnEnable()
        {
            BindActions(true);
            if (photoModeActionAsset == null)
                return;
            foreach (var map in photoModeActionAsset.actionMaps)
            {
                map.actionTriggered += PhotoModeInputs_actionTriggered;
                foreach (var action in map.actions)
                {
                    if (!action.enabled)
                    {
                        ownedActions.Add(action);
                        action.Enable();
                    }
                }
            }
        }

        private void OnDisable()
        {
            BindActions(false);
            if (photoModeActionAsset != null)
                foreach (var map in photoModeActionAsset.actionMaps)
                    map.actionTriggered -= PhotoModeInputs_actionTriggered;
            foreach (var action in ownedActions)
                action.Disable();
            ownedActions.Clear();
            moveAxis = lookAxis = Vector2.zero;
            modifier1_value = modifier2_value = 0;
        }

        private void BindActions(bool bind)
        {
            Bind(submitAction, SubmitAction_performed, bind);
            Bind(navigationAction, NavigateAction_performed, bind, true);
            Bind(lookAction, LookAction_performed, bind, true);
            Bind(resetAction, ResetAction_performed, bind);
            Bind(pauseAction, PauseAction_performed, bind);
            Bind(toggleGridAction, GridAction_performed, bind);
            Bind(toggleInterfaceAction, InterfaceAction_performed, bind);
            Bind(swapStickerAction, SwapStickerAction_performed, bind);
            Bind(deleteStickerAction, DeleteStickerAction_performed, bind);
            Bind(flipStickerAction, ExtraAction_performed, bind);
            Bind(modifier1Action, Modifier1Action_performed, bind, true);
            Bind(modifier2Action, Modifier2Action_performed, bind, true);
        }

        private static void Bind(InputActionReference reference, System.Action<InputAction.CallbackContext> callback, bool bind, bool canceled = false)
        {
            if (reference == null || reference.action == null)
                return;
            if (bind)
            {
                reference.action.performed += callback;
                if (canceled) reference.action.canceled += callback;
            }
            else
            {
                reference.action.performed -= callback;
                if (canceled) reference.action.canceled -= callback;
            }
        }
        private void ResetAction_performed(InputAction.CallbackContext callback)
        {
            ResetEvent.Invoke();
        }

        private void DeleteStickerAction_performed(InputAction.CallbackContext callback)
        {
            DeleteStickerEvent.Invoke();
        }

        private void SwapStickerAction_performed(InputAction.CallbackContext callback)
        {
            SwapStickerEvent.Invoke();
        }

        private void InterfaceAction_performed(InputAction.CallbackContext callback)
        {
            ToggleInterfaceEvent.Invoke();
        }

        private void GridAction_performed(InputAction.CallbackContext callback)
        {
            ToggleGridEvent.Invoke();
        }

        private void SubmitAction_performed(InputAction.CallbackContext callback)
        {
            SubmitEvent.Invoke();
        }

        private void NavigateAction_performed(InputAction.CallbackContext callback)
        {
            moveAxis = callback.ReadValue<Vector2>();
        }

        private void LookAction_performed(InputAction.CallbackContext callback)
        {
            lookAxis = callback.ReadValue<Vector2>();
        }

        private void PauseAction_performed(InputAction.CallbackContext callback)
        {
            PauseEvent.Invoke();
        }

        private void Modifier1Action_performed(InputAction.CallbackContext callback)
        {
            modifier1_value = callback.ReadValue<float>();
        }

        private void Modifier2Action_performed(InputAction.CallbackContext callback)
        {
            modifier2_value = callback.ReadValue<float>();
        }

        private void ExtraAction_performed(InputAction.CallbackContext callback)
        {
            FlipStickerEvent.Invoke();
        }

        private void PhotoModeInputs_actionTriggered(InputAction.CallbackContext obj)
        {

            //Set current device and switch instructions
            if (obj.control != null && currentInputDevice != obj.control.device)
            {
                currentInputDevice = obj.control.device;

                // Match device types, including renamed or synthetic devices.
                bool isDeviceKeyboard = obj.control.device is Keyboard;
                bool isDeviceMouse = obj.control.device is Mouse;

                if (infoBarController != null) infoBarController.SetKeyboardModeActive(isDeviceKeyboard || isDeviceMouse);
            }
        }


    }
}
