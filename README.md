Beware, XRNeckSafer is currently not working... We are looking into it... :)

Installation:

1. Create a folder and unzip XRNeckSafer.zip into it.
2. Copy the OpenXrApiLayer to %ProgramFiles% (e.g. C:\Program Files\OpenXrApiLayer)
   You will need admin rights to put them there. The install- and uninstall- scripts should be run from that location.  The rest of the files in the .zip including the .exe can stay out of the %ProgramFiles% folder. No need to run the .exe as admin. 
2. Go into the %ProgramFiles%\OpenXrApiLayer and run "Install-XR_APILAYER_NOVENDOR_XRNeckSafer.ps1" as a PowerShell script. 
   (right click on it and select "Execute with PowerShell") This will install the API Layer.
3. Start/Restart Mixed-Reality-Portal 
4. Start OpenXR Developer Tools and check on system status. XR_APILAYER_NOVENDOR_XRNeckSafer should be listed as API Layer
5. Start XRNeckSaferApp.exe in the main folder and go to OpenXR in the Menu on top. Select "Show active OpenXR API Layers".
   This should list all the active Layers and should now include "XR_APILAYER_NOVENDOR_XRNeckSafer".
6. If this works you can configure XRNeckSafer similar to VRNeckSafer.
7. Once in game press the combined Game/XRNS reset button.
8. Check that the HMD yaw value 
Known issues:

- The Menu "Activate XRNS OpenXR API Layer" is not working yet. That's why you have to use the powershell script...
- Translation is not working correctly. Better leave it at 0 currently.
- When an yaw offset is activated by VRNS your translational movement dos not account for it yet. 
  Hard to describe but you will notice it when moving around in the cockpit with an activated yaw offset.
  
Should work with IL-2 and DCS together with OpenComposite. I tested it on my G2 only though...  

Thanks for you help guys!
Please use the XRNS discord for feedback: https://discord.gg/GgUpAejN

Cheers,
 J2 NobiWan





VRNeckSafer tries to help virtual pilots flying in VR to not break their neck while trying to check their six.
It adds an angular offset to the current viewing angle by pressing a joystick button. Currently working with IL2 and DCS with [OpenComposite](https://gitlab.com/znixian/OpenOVR/-/tree/openxr/).

<img src="https://gitlab.com/NobiWan/vrnecksafer/-/raw/master/VRNeckSafer/Release/VRNSv209.JPG">    <img src="https://gitlab.com/NobiWan/vrnecksafer/-/raw/master/VRNeckSafer/Release/VRNSv209b.JPG"> 
 

**How to use it:**

Simply choose the two joystick/HOTAS buttons you want to use for left and right offset and the required rotation angle and set the Reset button as shown on the app.
When in the game, press the assigned reset button to calibrate.
Thats it. It works with normal SteamVR (no Beta required).


If you prefer adding up the offset angle with every button click, select the "Accumulative" option.

If you want to move your head position a few centimeters when using snap view (e.g. to look around your seat) use the Translation feature.

Enable the "Autorotation" feature to automatically activate the offset when turning your head over defined activation angles and deactivated when below a deactivation angle. No joystick buttons required.
This can be done in several steps. To temporarily inhibit autorotation use the Hold buttons.



Download link: [XRNeckSaferV01.zip](https://gitlab.com/NobiWan/xrnecksafer/-/blob/master/VRNeckSafer/Release/VRNeckSaferV01.zip)
