# Ski Game
This is A gamification Thesis Project By Daniel Friedlander

## Gameplay Video
Unlisted Youtube video of project can be found here
https://youtu.be/6rFnBY4x6eo


Ski Game Readme

Github Link: https://github.com/danfriedz/Ski_Game_Thesis
Author: Daniel Friedlander
Student ID: 19235341

---------------------------------

##How to play:
Connect the motion controls and move the player around the arena. Moving to any side of the screen will start a stretch.
Complete tstretch until session is over. You can also move to the signposts to do a jump stretch minigame.
---------------------------------
##Threshold Input Mode:
This adapts the game to the players range of motion.
1) Hold the "T" key and move your wrist in every direction (show the game your range of motion)
2) Let go of "T" to exit threshold input mode.
3) Press "Y" To confirm the user’s input. The player will now move to a new centre point on the screen.

Due to the new centre point, you won't need to stretch so far if you have reduced range of motion in one direction.
Pressing "Y" also generates a excel file with the user’s thresholds in degrees for each direction.

Note: If threshold input mode isn't used, a full range of motion is assumed. If the user’s range of motion is exceptionally low a min safe value is used (30 degrees). This is because very low values would make the player move with high sensitivity.
---------------------------------

##How to use the motion controls:
1) Connect the XSENS DOT IMU sensors to your computer via Bluetooth (either one or two will work, with no extra steps)
2) Run xsensdot_pc_sdk_receive_data.py python script in VSCode
	2.1) there are some steps involved to get this working. including installing the sdk and .NET.
3) Wait until the sensor(s) start streaming Euler values as a string in VSCODE terminal
4) Switch to Unity project and hit play
5) You should be able to control the game with the motion controls now
6) When you are done make sure to press the 'esc' key. This manually stops the Bluetooth connection. If you don't do this you will need to restart Unity as you will get an infinite loading screen (reloading domains) as unity attempts to open a 2nd Bluetooth connection.
---------------------------------

##Future Works:
Some functionality will need to be changed when all the games are combined

	Consider putting threshold input mode into the framework and letting other games use it.	

	eg, stretch timer, stretch counts etc which will need to move into the framework

	the threshold input system has functionality to print logs. Its pretty basic and could use an upgrade similar to what already exists in the framework for logging Euler values.

	The Left and Right Jump scenes stretch timer will also need to be set via framework

	The game doesn’t have an end state. When all stretches are complete just kick the player back to the framework, I didn't do this because it needs to be changed anyway.

---------------------------------
