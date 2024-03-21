# Sounds Good Plugin

Thank you for choosing Sounds Good, a powerful audio management plugin for Unity developed by Melenitas Dev.
I appreciate your support and trust in my product <3.

## How to use it ○

To start using Sounds Good and bring your video games to life with music and sound, follow these simple steps:

1.  Open the "Audio Creator" window, which you can find at Tools > Melenitas Dev > Sounds Good.
2.  Create a sound or music by adding one or multiple audio clips (if you add more than one, a random clip will be chosen each time to avoid monotony) 
    and assign a tag for identification. Also, choose the compression preset that best suits the sound to enhance your game's performance.
3.  Open the script from where you want to play the sound/music and import the "MelenitasDev.SoundsGood" library. Create a variable of any available 
    type based on the audio you need to play (Sound, Music, Playlist, or Dynamic Music) and initialize it in the Start or Awake events by passing the 
    tag of the audio you've created or, alternatively, its variable in the auto-generated enumerator for easier use.
    Example:
    Sound jump = new Sound(SFX.Jump);
4.  Customize the sound by chaining the functions provided by Sounds Good. 
    For example:
    jump.SetLoop(true).SetRandomPitch().SetVolume().SetRandomClip().SetFollowTarget(transform);
5.  Play the sound anywhere in the code you need it using the Play() function.

If you have any questions or need assistance with using Sounds Good, please refer to the comprehensive documentation. 
It provides detailed information on how to integrate and maximize the use of the plugin in your Unity projects. 
You can access the documentation here:

- ENGLISH: https://melenitasdev.notion.site/melenitasdev/SOUNDS-GOOD-English-documentation-e2102ec5bafa411cb7991dba60081075
- SPANISH: https://melenitasdev.notion.site/melenitasdev/SOUNDS-GOOD-Documentaci-n-espa-ol-b8fd14728ba74050a55a4c4f41ec0727

## Credits ☻

I would like to acknowledge and extend my gratitude to the following individuals and resources
for their valuable contributions to produce de Demo of this project:

- Kenney (www.kenney.nl): UI Pack
- Nicky Case!: Music tracks of his collection 'Cute Gay Nerd'
- www.sweetsymmetry.com: Music tracks of the dynamic/adaptative music 'Gravity'

These individuals and resources have greatly contributed to the success of the Sounds Good plugin, and I am thankful for their support.

## Licensing ©

Sounds Good is protected by copyright © 2023 Melenitas Dev. 
All rights are reserved. Distribution of the standalone asset is strictly prohibited.
