**Changelog for beta2b:**
- smooth autorotation values are now saved correctly
- fixed extreme amplify values at very first start
- hold button works now in smooth autorotation
- manual L/R buttons work now in smooth autorotation

(If you update from an earlier version you can just replace XR_APILAYER_NOVENDOR_XRNeckSafer.dll and XRNeckSaferApp.exe with the new versions. Stop Mixed Reality Portal before replacing the .dll)

**Changelog for beta2:**
- smooth autorotation (linear multiplication for now) Thanks Firdimigdi!
- a memory leak fixed
- work on compatibility with native OpenXR games (MSFS) but not there yet.
- some UI work
- Reset button is now called Center button to avoid confusion



## **Installation:**

0. Download [XRNeckSaferBeta2b.zip](https://gitlab.com/NobiWan/xrnecksafer/-/blob/master/Assets/XRNeckSaferBeta2b.zip)
1. Create a folder and unzip XRNeckSaferBeta2b.zip into it.
2. Stop Mixed-Reality-Portal and copy the OpenXrApiLayer folder to %ProgramFiles% (e.g. C:\Program Files\OpenXrApiLayer) 
   You will need admin rights to put it there. The rest of the files in the .zip including the .exe can stay out of the %ProgramFiles% folder. 
3. Go to the %ProgramFiles%\OpenXrApiLayer folder and run "Install-XR_APILAYER_NOVENDOR_XRNeckSafer.ps1" as a PowerShell script 
   (right click on it and select "Execute with PowerShell"). This will install the API Layer.
4. Start/Restart Mixed-Reality-Portal 
5. Start OpenXR Developer Tools and check on system status. XR_APILAYER_NOVENDOR_XRNeckSafer should be listed as API Layer
6. **(disregard 6, for now, menu disabled until fixed)** Start XRNeckSaferApp.exe in the main folder and go to OpenXR in the Menu on top. Select "Show active OpenXR API Layers". 
   This should list all the active Layers and should now include "XR_APILAYER_NOVENDOR_XRNeckSafer". 
7. If this works you can configure XRNeckSafer similar to VRNeckSafer.
8. Once in game press the combined Game/XRNS center button.
9. Check that the HMD yaw value is changing when moving the HMD.

## **Known issues:**

- The Menu "Activate XRNS OpenXR API Layer" is not working yet. That's why you have to use the powershell script...
- On some systems 6. might fail. If 5. was successfull disregard the error. XRNS should work anyway.
- If translation is not working correctly reset HOME and ENVIRONMENT in Mixed Reality Portal (thanks Melanor8807).
- When a rotation offset is activated by XRNS your translational movement dos not account for it yet. 
  Hard to describe but you will notice it when moving around in the cockpit with an activated offset.
- "Smooth" autorotation might cause nausea for some users! It does for me... So be careful und stop it when you feel uncomfortable!
- When you are experiencing FPS loss with XRNS (especially with Oculus HMDs) try to minimize the XRNS App.
- XRNS works with games that can use OpenComposite (e.g. IL-2 and DCS). Native OpenXR games are not working yet. 

## **Roadmap:**
 - pitch axis
 - user curves for smooth autorotation 
 - fix for translational movement when rotated
 - UI overhaul
 - make it work with native OpenXR games (MSFS)
 - build instructions
 - better documentation
 - installer

## **Feedback**

The easiest way to give feedback is to use the XRNS discord server:  https://discord.gg/pwcxxTE8TF

## **Thanks**

The OpenXR API Layer is heavily based on mbucchia's [XR_APILAYER_NOVENDOR_fov_modifier](https://github.com/mbucchia/XR_APILAYER_NOVENDOR_fov_modifier). Thanks for this and his kind help! 
Thanks to Firdimigdi for his support on smooth autorotation.

This project uses several third-party libraries, which are used and distributed under their own license terms.

Thanks for you help and patience guys!

Cheers,
 J2_NobiWan


### Download link: [XRNeckSaferBeta2b.zip](https://gitlab.com/NobiWan/xrnecksafer/-/blob/master/Assets/XRNeckSaferBeta2b.zip)

# **Description**

XRNeckSafer tries to help virtual pilots flying in VR to not break their neck while trying to check their six.
It adds an angular offset to the current viewing angle by pressing a joystick button. Currently working with IL2 and DCS with the **OpenXR** version of [**OpenComposite**](https://gitlab.com/znixian/OpenOVR/-/tree/openxr/).

<img src="https://gitlab.com/NobiWan/xrnecksafer/-/raw/master/Assets/XRNS1.JPG">  <img src="https://gitlab.com/NobiWan/xrnecksafer/-/raw/master/Assets/XRNS2.JPG">
 <img src="https://gitlab.com/NobiWan/vrnecksafer/-/raw/master/VRNeckSafer/Release/VRNSv209b.JPG"> 
 
## **How to use it:**

Simply choose the two joystick/HOTAS buttons you want to use for left and right offset and the required rotation angle and set the Reset button as shown on the app.
When in the game, press the assigned reset button to calibrate.
Thats it. 


If you prefer adding up the offset angle with every button click, select the "Accum(ulative)" option.

If you want to move your head position a few centimeters when using snap view (e.g. to look around your seat) use the "Translation" feature.

Enable one of the "Autorotation" features to automatically activate the offset when turning your head over defined activation angles. No joystick buttons required.
This can be done in several steps or continuous (smooth). To temporarily inhibit autorotation use the Hold buttons.

### **Stepwise Autorotation**

 There is a (very) small explanation coming up when you hover your mouse over the labels on top of the table in the app.

**"act"** is the turn angle of your head when the extra rotation "rot" is activated. In my example this means when you turn your head more than 60 deg it will add an extra 10 deg. More than 70 deg head movement will add 20 deg extra rotation and so on.

**"de"** ist the turn angle of your head when an extra rotation angle is beeing deactivated again.

Lets assume you turned your head 85 deg to the right. This will have activated an extra rotation of 30 deg. Now, when turning your head back to to say 75 deg you will still have that 30 deg extra rotation because 75 is more than "de"=71. Only if you reduce your head rotation to less tha 71 deg the extra offset will be reduced to 20 deg and so on. When your head is turned back to the front to less than 51 deg there is no extra extra angle added any more.

**"L/R"** and **"Fwd"** are the amounts of cm that your virtual position is shifted Left/Right and Forward, whith respect to the activation/deactivation angles, similar to the rotation.

Pressing the graph button (above the table) shows you a representation of the entered values and the resulting visual vs actual yaw motion: 

### **Smooth Autorotation**

Smooth Autorotation gives a linear amplification of your head rotation, beginning at the **"Start at"** value. **"Amplify by"** 100%  means that for every degree head rotation you get one extra degree visual rotation. So, with a "Start at" value of 90 deg, if you turn your head to 100 you get 110 deg visual rotation (10 deg from 90 deg + 10 deg amplification). 200% gives 2 extra degrees for every "real" degree head rotation, and so on.



