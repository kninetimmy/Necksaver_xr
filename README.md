
Changelog for beta2:
- smooth autorotation (linear multiplication for now) Thanks Firdimigdi!
- a memory leak fixed
- work on compatibility with native OpenXR games (MSFS) but not there yet.
- Some UI work
- Reset button is now called Center button to avoid confusion

Installation:

1. Create a folder and unzip XRNeckSaferBeta2.zip into it.
2. Copy the OpenXrApiLayer to %ProgramFiles% (e.g. C:\Program Files\OpenXrApiLayer) 
   You will need admin rights to put them there. The install- and uninstall- scripts should be run from that location. 
   The rest of the files in the .zip including the .exe can stay out of the %ProgramFiles% folder. No need to run the .exe as admin. 
3. Go into the %ProgramFiles%\OpenXrApiLayer and run "Install-XR_APILAYER_NOVENDOR_XRNeckSafer.ps1" as a PowerShell script. 
   (right click on it and select "Execute with PowerShell") This will install the API Layer.
4. Start/Restart Mixed-Reality-Portal 
5. Start OpenXR Developer Tools and check on system status. XR_APILAYER_NOVENDOR_XRNeckSafer should be listed as API Layer
6. Start XRNeckSaferApp.exe in the main folder and go to OpenXR in the Menu on top. Select "Show active OpenXR API Layers". (disregard 5. for now, menu disabled until fixed)
   This should list all the active Layers and should now include "XR_APILAYER_NOVENDOR_XRNeckSafer". 
7. If this works you can configure XRNeckSafer similar to VRNeckSafer.
8. Once in game press the combined Game/XRNS center button.
9. Check that the HMD yaw value is changing when moving the HMD.

Known issues:

- The Menu "Activate XRNS OpenXR API Layer" is not working yet. That's why you have to use the powershell script...
- On some systems 5. might fail. If 4. was successfull disregard the error. XRNS should work anyway.
- If translation is not working correctly reset HOME and ENVIRONMENT in Mixed Reality Portal (thanks Melanor8807).
- When a rotation offset is activated by XRNS your translational movement dos not account for it yet. 
  Hard to describe but you will notice it when moving around in the cockpit with an activated offset.
- "Smooth" autorotation might cause nausea for some users! It does for me... So be careful und stop it when you feel uncomfortable!
- When you are experiencing FPS loss with XRNS (especially with Oculus HMDs) try to minimize the XRNS App.
- XRNS works with games that can use OpenComposite (e.g. IL-2 and DCS). Native OpenXR games are not working yet. 

Roadmap:
 - pitch axis
 - user curves for smooth autorotation 
 - fix for translational movement when rotated
 - UI overhaul
 - build instructions
 - better documentation
 - installer

Thanks for you help and patience guys!
Please use the XRNS discord for feedback: https://discord.gg/GgUpAejN

Cheers,
 J2 NobiWan


XRNeckSafer tries to help virtual pilots flying in VR to not break their neck while trying to check their six.
It adds an angular offset to the current viewing angle by pressing a joystick button. Currently working with IL2 and DCS with [OpenComposite](https://gitlab.com/znixian/OpenOVR/-/tree/openxr/).

<img src="https://gitlab.com/NobiWan/vrnecksafer/-/raw/master/VRNeckSafer/Release/VRNSv209.JPG">    <img src="https://gitlab.com/NobiWan/vrnecksafer/-/raw/master/VRNeckSafer/Release/VRNSv209b.JPG"> 
 
The OpenXR API Layer is heavily based on mbucchia's [XR_APILAYER_NOVENDOR_fov_modifier](https://github.com/mbucchia/XR_APILAYER_NOVENDOR_fov_modifier). Thanks for this and his kind help! 

This project uses several third-party libraries, which are used and distributed under their own license terms.

**How to use it:**

Simply choose the two joystick/HOTAS buttons you want to use for left and right offset and the required rotation angle and set the Reset button as shown on the app.
When in the game, press the assigned reset button to calibrate.
Thats it. It works with normal SteamVR (no Beta required).


If you prefer adding up the offset angle with every button click, select the "Accumulative" option.

If you want to move your head position a few centimeters when using snap view (e.g. to look around your seat) use the Translation feature.

Enable the "Autorotation" feature to automatically activate the offset when turning your head over defined activation angles and deactivated when below a deactivation angle. No joystick buttons required.
This can be done in several steps. To temporarily inhibit autorotation use the Hold buttons.



Download link: [XRNeckSaferBeta2.zip](https://gitlab.com/NobiWan/xrnecksafer/-/blob/master/Assets/XRNeckSaferBeta2.zip)
