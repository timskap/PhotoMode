<img src="https://user-images.githubusercontent.com/7104693/128102446-01f5ef64-e445-4a1b-9bd8-64aeb2c03a15.png" width="100%" />

## Overview

The Photo Mode package is a gateway for adding in-game photography systems to your PC and console Unity projects. The included prefab works across a variety of game styles and can be customized to your specific needs. The package also comes with a demo scene for reference and all required scripts, UI, and assets for out-of-the-box functionality. 

The primary Unity features used in this package include the Universal Render Pipeline, UI system, Cinemachine, Input System, Post-Processing, and Shader Graph. 

Photo Mode comes with the following features: View Roll, View Distance, Focus Distance, Aperture, Exposure, Contrast, Saturation, Vignette, Filters, Frames, Stickers, Camera Height, Camera Rotation, Rule of Thirds Grid, & Hide UI. 

<p align="center"> 
  <img src="https://user-images.githubusercontent.com/67757696/130872485-f46baabd-bae0-4bfa-b7c8-afed790c2b94.png" width="48%" />
  &nbsp;&nbsp;
  <img src="https://user-images.githubusercontent.com/7104693/128102643-6964ab3b-208d-4f3e-9867-c8ca025c68ed.png" width="48%" />
</p>

<br>

## Requirements

- This compatibility branch targets **Unity 6000.6.7f1 with URP 17.6.0 and Render Graph enabled**. Earlier Editor/package versions and Compatibility Mode have not been validated for these changes.

- The standalone consumer resolved Cinemachine **6.6.0**, Input System **1.20.0**, and uGUI/TextMeshPro **2.6.0**. Import the TMP Essential Resources supplied with uGUI. The camera deliberately retains Cinemachine's deprecated FreeLook compatibility types; a full camera migration is outside this repair.

- A scene containing a Cinemachine Brain and Virtual Camera (set to your preference)

<br>

## Unity 6000.6.7f1 consumer setup

The repository is a complete historical sample, not a UPM package. Its original ProjectVersion and package manifest are retained; those files do not describe the verified new consumer. Do not infer compatibility with the old sample Editor from this branch.

1. Create a fresh Universal 3D project using the exact Editor above and the resolved package versions listed under Requirements.
2. Copy `Assets/PhotoMode` and its `.meta` files from this branch. Exclude `PM-TutorialInfo` and its folder metadata: its global `Readme` classes conflict with the new-project template tutorial. Do not copy Library, Temp, UserSettings or the donor ProjectSettings.
3. Import **TMP Essential Resources** from the installed uGUI package. The tested import used this step after copying PhotoMode, updating the embedded TMP resource GUIDs in the consumer. Missing Essentials can leave all menu text blank.
The prefab's Filter Materials list must retain its six typed material assets under `Materials/Filters`. They preserve shader-specific texture/default dependencies in Player builds; storing those values only as inactive properties on the Unlit blit material produced a missing DotOverlay circle mask in native tests. Runtime changes copy template properties into an isolated material and never mutate the templates.

4. In the consumer, create a scene with a visible subject, Main Camera plus Cinemachine Brain, and a normal game camera. Enable post-processing on the Main Camera. Instantiate the PhotoMode prefab and assign its Player Object.
5. Add a **Blit** renderer feature to the renderer actually used by the consumer camera, assign `PhotoMode/Materials/BlitMaterial`, and assign that same feature to the prefab's Blit field. Do not rely on an unused donor renderer. Give the game camera priority above the inactive PhotoMode camera (for example 10), and below its active priority (99).
6. Use Input System Dynamic Update and a single active EventSystem. PhotoMode temporarily takes input/camera/cursor ownership and restores the captured state on exit or disable. Test P, exit, and the UI with Game View focused.

Filter and Frame support left-clicks on their existing arrow indicators; each click selects one option. Keyboard/controller slider navigation is preserved. The bottom info bar displays input-key hints, rather than clickable buttons.

The renderer now records a Render Graph raster pass using the existing `_MainTex` Shader Graph filters, an intermediate color texture, clip-space drawing and restored camera matrices. No legacy Execute/RTHandle path remains. Multi-camera stacks, XR, physical controller behavior and other Unity versions are not covered by this pilot.

API cleanup uses FindAnyObjectByType for the single-host fallback and an owned persistent fullscreen quad with disposal on renderer-feature recreation/shutdown. This preserves the existing Unlit Shader Graph vertex/texture contract. With multiple eligible hosts, fallback selection is arbitrary; explicitly assign Player Object.

The camera still uses deprecated CinemachineFreeLook and AxisState.IInputAxisProvider compatibility interfaces. A modern CinemachineCamera/OrbitalFollow/InputAxisController migration requires coordinated prefab and control changes and is deferred. These warnings remain visible; no warning suppression is applied.

Regression checks cover timeScale 1/0.5/0, 10 activation cycles, disabled PlayerInput preservation, component/prefab disable, disabled-component activation rejection, callback lifetime, camera blend restoration and shared-material isolation. Visual checks cover all six filters, four frames, DOF/color settings, grid, stickers and Hide UI. Windows Mono build results and private lab evidence are maintained separately; a successful build is not a manual native-input verdict.

## Historical sample setup

Context for these steps can be viewed in our [Photo Mode Overview Video](https://youtu.be/GEPJZwFt2DM).

0. &nbsp;Download the Photo Mode package from this repo’s [Releases](https://github.com/UnityTechnologies/PhotoMode/releases) page.

0. &nbsp;Open a Unity project with the requirements listed above. Ensure all required packages are added before importing Photo Mode and that your scene with the Cinemachine Brain and Virtual Camera is open.

0. &nbsp;Import the Photo Mode package into your project by navigating to Assets > Import Package > Custom Package.

0. &nbsp;In your Project window, locate the “PhotoMode” prefab within PhotoMode > Prefabs and drag it into your scene.

0. &nbsp;Select your scene’s newly added PhotoMode prefab and use the Inspector to assign your Player object (or other object you want Photo Mode to focus on) to the “Player Object” field within the Photo Mode script component.

0. &nbsp;Within the Project window, select your project’s main Renderer asset (usually listed as “Forward Renderer”). Go to the asset’s settings within the Inspector window and add a Blit Render Feature to the asset by selecting Add Renderer Feature > Blit.

0. &nbsp;The Render Feature you just created contains a field called “Blit Material.” Set its target to be the material called “BlitMaterial” located within the PhotoMode > Materials folder.

0. &nbsp;Select the “PhotoMode” prefab in your scene and locate the “Blit” field within its “Photo Mode” script component. Ensure that field is populated with the Blit you just added to the Renderer.

0. &nbsp;Open your Project Settings, navigate to the Input System Package settings, and ensure the Update Mode is set to "Process Events in Dynamic Update."

0. &nbsp;Enter Play mode to test Photo Mode. By default, Photo Mode can be activated by pressing “P” on a keyboard or “SELECT” on a gamepad.

<br>

## Saving Screenshots
Once you apply your desired effects with Photo Mode, use the Hide UI option to see an unobstructed view of your creation. You can then save it as an image using your device’s native Share or Screenshot button. 

<br>

## Customization Notes

- You can adjust the range for standard sliders — such as View Roll, Exposure, and Vignette — by changing their corresponding Min & Max values within the Photo Mode script component on the main PhotoMode prefab object: <br><br><img src="https://user-images.githubusercontent.com/7104693/128586864-64a1d63b-5cef-4ad8-ae3e-5cc442acc3a4.png" width="50%"> <br><br>Options and values for the text-based sliders can be changed via the Text Slider component within the corresponding feature’s Slider object:  <br><br><img src="https://user-images.githubusercontent.com/7104693/128586931-bf39069f-a13d-4de2-908e-c3d766f85ad3.png" width="50%"> <br><br>The value of each slider in Photo Mode defaults to the average between its corresponding Min & Max values. For example, if the default value you want for Post Exposure is 1.5, the Min/Max range for it could be -0.5/3.5. <br><br>  <br>



- Photo Mode contains a Volume with post-processing effects that may override your existing Volume’s Depth of Field or Color Adjustments settings. You can update Photo Mode to work with your Volume’s values by following these two steps:
  - In the Hierarchy, select the PhotoMode_Volumes child object of the Photo Mode prefab. Change any conflicting values within its Depth of Field or Color Adjustments settings to match what you have in your original Volume. Also, make sure the PhotoMode_Volumes priority is set to a higher value than your original Volume. <br>
  - On the main PhotoMode prefab object, update the Min/Max ranges for the corresponding settings within the Photo Mode script component. Since the value of each slider defaults to the average between its Min & Max values, be sure to set the ranges to have an average that matches the value of the respective setting in your post-processing Volume. <br><br><br>


- If you’d like to enable Photo Mode using a different input, UI event, or other custom setup, be sure to turn off the Pause Action Activation checkbox within the Photo Mode Pauser script component on the main PhotoMode prefab object. Once turned off, Photo Mode will no longer open when pressing the default “P” key so you can integrate Photo Mode into your preferred setup.

<br> 

## Resources
Here are some links to helpful context regarding the Photo Mode Demo Package:
- [Photo Mode Overview Video](https://youtu.be/GEPJZwFt2DM)
- [Blog Post](https://on.unity.com/3kqi8m2)
- [Forum Thread](https://on.unity.com/3BfRFyd)
