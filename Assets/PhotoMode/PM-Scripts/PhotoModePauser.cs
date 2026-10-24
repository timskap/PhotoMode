using UnityEngine;
using UnityEngine.InputSystem;

namespace PhotoMode
{
    public class PhotoModePauser : MonoBehaviour
    {
        [SerializeField] private bool pauseActionActivation;
        private PlayerInput playerInput;
        private PhotoModeInputs photoModeInputs;
        private PhotoMode photoModeBehaviors;
        private PhotoModeStickerController stickerController;
        private bool gamePaused;
        private float originalTimeScale;
        private bool originalInputEnabled;

        private void Awake()
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            photoModeInputs = GetComponent<PhotoModeInputs>();
            photoModeBehaviors = GetComponent<PhotoMode>();
            stickerController = GetComponentInChildren<PhotoModeStickerController>(true);
        }

        private void OnEnable()
        {
            photoModeBehaviors.OnPhotoModeActivation.AddListener(HandlePhotoModeActivation);
            if (pauseActionActivation)
                photoModeInputs.PauseEvent.AddListener(PauseAction);
        }

        private void OnDisable()
        {
            photoModeBehaviors.OnPhotoModeActivation.RemoveListener(HandlePhotoModeActivation);
            photoModeInputs.PauseEvent.RemoveListener(PauseAction);
            if (gamePaused)
                PauseGame(false);
        }

        private void HandlePhotoModeActivation(bool active)
        {
            if (!active && gamePaused)
                PauseGame(false);
        }

        public void PauseAction()
        {
            if (!isActiveAndEnabled || !photoModeBehaviors.isActiveAndEnabled)
                return;
            if (gamePaused && stickerController != null && stickerController.IsActive())
            {
                stickerController.ToggleStickerMode(false);
                return;
            }
            PauseGame(!gamePaused);
        }

        private void PauseGame(bool pause)
        {
            if (pause == gamePaused)
                return;

            if (pause)
            {
                originalTimeScale = Time.timeScale;
                originalInputEnabled = playerInput != null && playerInput.enabled;
            }
            gamePaused = pause;
            Time.timeScale = pause ? 0 : originalTimeScale;
            if (playerInput != null)
                playerInput.enabled = pause ? false : originalInputEnabled;
            photoModeBehaviors.Activate(pause);
        }
    }
}
