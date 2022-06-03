VRNeckSafer tries to help virtual pilots flying in VR to not break their neck while trying to check their six.
It adds an angular offset to the current viewing angle by pressing a joystick button. Currently working with IL2 and DCS **with SteamVR**.

<img src="https://gitlab.com/NobiWan/vrnecksafer/-/raw/master/VRNeckSafer/Release/VRNSv209.JPG">    <img src="https://gitlab.com/NobiWan/vrnecksafer/-/raw/master/VRNeckSafer/Release/VRNSv209b.JPG"> 
 

**How to use it:**

Simply choose the two joystick/HOTAS buttons you want to use for left and right offset and the required rotation angle and set the Reset button as shown on the app.
When in the game, press the assigned reset button to calibrate.
Thats it. It works with normal SteamVR (no Beta required).


If you prefer adding up the offset angle with every button click, select the "Accumulative" option.

If you want to move your head position a few centimeters when using snap view (e.g. to look around your seat) use the Translation feature.

Enable the "Autorotation" feature to automatically activate the offset when turning your head over defined activation angles and deactivated when below a deactivation angle. No joystick buttons required.
This can be done in several steps. To temporarily inhibit autorotation use the Hold buttons.

**Update v1.1:**

I added the option to use a joystick button to reset the HMD Yaw angle. To use the autorotate feature look straight ahead and press that button (or the reset button in the GUI). This will tell the app from which central Yaw position the activation/deactivation angles are measured.  

**Update v1.2:**

I included a feature to define translational offsets. Better use only a few centimeters here and reset HMD yaw...
Still some  AV checker might give false alerts, haven't got an idea how to avoid that.  

**Update v1.3:** 

Minor bugfix when no button was selected and a beautiful new icon! ;)

**Update v1.4:**

Added an option to use diagonal Hat positions.
To use it:
-     run VRNeckSafer v1.4 once to update the VRNeckSafer.cfg file
-     exit VRNeckSafer v1.4
-     open VRNeckSafer.cfg in a text editor
-     at the line "Use8WayHat" change false to true
-     safe and exit editor
 
**Update v1.5:**

Separate rotation angles for snap and autorotation

**Update v1.6:**

Compatibility to other major combat sim. 
Added a "temporary freeze" button for autorotation 

**Update v2.0:**

new features/changes:
+ seperate joysticks for each button
+ modifiers
+ several steps for autorotate 
+ buttons can be scanned/recognized 
+ More than one Hold button
+ buttons can be inverted
+ buttons can toggle
+ only a single instance allowed
+ start minimized
+ minimize to tray
+ config file can be added as parameter
+ options to fiddle with: 
   force seated/standing mode
   position compensation
   app mode can be "Background" (requires SteamVR to already run) or "Overlay" (starts SteamVR on its own)
 
**Update v2.01:**

minor change: typo fix in GUI and allowed Autorotation with Accumulative mode.
 
**Update v2.02:**

hotfix for DCS   

**Update v2.03:**

fix for fwd translation when autorotation to left 

**Update v2.04:**

**Update v2.05:**
- added fancy diagram to help understand the autorot values
- fixed offset bug reintroduced in v2.04

**Update v2.06:**
- corrected error in translation when HMD Offset is big for IL-2
- reset button resets accumulative offset
- added pitch limit (options menu). when HMD pitch (looking up) is bigger than limit autorot goes on hold. default is 90 deg = not effective 

 **Update v2.07:**
 - fixed GUI font
 - Added option to select more than one button for manual rotation

 **Update v2.08:**
 - Added option to select more than one reset button

 **Update v2.09:**
 - Fixed crash when joystick disconnects. Reacquires joystick when reconnected.
 - Added seperate reset button for accumulative mode

known issues:
still problems with Valve Index and Varjo Aero

Download link: [VRNeckSaferV20VRNeckSaferV204VRNeckSaferV209.zip](https://gitlab.com/NobiWan/vrnecksafer/-/blob/master/VRNeckSafer/Release/VRNeckSaferV209.zip)
