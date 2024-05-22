<a name="readme-top"></a>
# Word Training - A Unity Mobile Educational App for Android

</br>
<div align="center">
<img src="https://github.com/A-Morabet/word-training/blob/main/02-screenshot.png" width="600"/>
</div>
</br>

Made with Unity Engine and Adobe Suite.

## Summary

Word Training is a mobile application made with the purpose of helping people develop their day-to-day english vocabulary in an engaging way.
Project started around first-half of 2022, work was done a few hours on the weekends, then during the first quarter of 2023 I picked up the pace
and was able to complete the app during summer.
App was made in the Unity Environment, more than 15.000 lines of C# code were written to define the App's logic, front-end and back-end aspects.

During the App session the user is presented with a small television on the upper-right corner with a hangman section below it; the television shows
the image of the item that the user must recognise and then hit the props containing the letters that form the item's word in any order until 
he completes the word, he then is rewarded with additional points and the next word is presented shortly after.</br><div align="middle"><a href="https://aminemorabet.com/vids/portfolio-02.mp4#t=0.1" target="__blank" align="middle">Video Demonstration</a></div></br>

In the App, the user can:

- Choose from three different vocabulary themes related to the house: Bedroom, Bathroom and Kitchen.
- Select from three levels of Word Difficulty which sets the level of frequency of the words being presented...etc.
- Adjust a series of options in the Options Menu such as App Difficulty which determines the types of props that are presented to the user, as well as Left-Handed Mode.
- Play a Coin Minigame that provides him a series of "continues" after watching a video ad.
- Watch a short video tutorial that explains the main mechanics of the App and a Credits Section.

## Code samples

This repository contains some of the scripts that the app relies on, their purpose in the repository is to showcase their construction and functionality:

* <b>"AnimCentral.cs"</b> : This script has two functions: first it handles all transition animations from one menu canvas to another which is a total of 14. Secondly, it handles the video player logic in the How To Play Section.
* <b>"SceneLoader.cs"</b> : Script sets up all variables and references whenever a scene is loaded, this includes button event listeners and scene objects.
* <b>"GameSession.cs"</b> : This script acts as a nexus for all other game logic when user starts a level, it receives and sends information to all other logic-involved scripts. It also manages music and sound effects.
* <b>"DestroyerHandler.cs"</b> : This script interfaces mainly with "GameSession" and "NewWordPacker", providing information on hit collision and extending prop behavior, p.ex. if the user touches a clay plate the script recognises it and applies its intended behavior: to lose one hitpoint and move to the opposite side from here it was touched. It has an additional function which is score display on hit position.
* <b>"NewWordPacker.cs"</b> : Charged with handling word logic during playthrough. It is one of the "hearts" of the App since it handles many crucial gameplay-related functions such as word-picking and slicing, letter-comparing on hit, word filling, score assignment, word-related animations and the boss intro and outro animations.
* <b>"Leaderboard.cs"</b> : Script handles the local App leaderboard, it creates a default JSON database for all themes and difficulty combinations the first time is opened. Each time the user reaches a highscore, it prompts the user to register his score on the chosen theme and difficulty. It also contains logic for score template display among other functions.
* <b>"CarryOvers.cs"</b> : This script is charged with storing and setting all variables that are shared across all scripts as well as saving local settings such as sound volume, last picked difficulty, number of owned game coins...etc.

<i>Additional Notes:</i> There are other features such as a score multiplier system that rewards flawless execution and a speed system that dynamically balances difficulty by increasing the game's speed the better the user performs and slowing it now if the user struggles too much during the playthrough.

### Prerequisites

No packages are needed to run this project on mobile.

### Installation

<ins>Application is currently unavailable</ins> due to undergoing Google Play Publishing Process.

## Contact

Amine Morabet - hey@aminemorabet.com

<p align="right">(<a href="#readme-top">back to top</a>)</p>

