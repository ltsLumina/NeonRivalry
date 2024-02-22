**************************************
*          TRANSITIONS PLUS          *
*         Created by Kronnect        * 
*            README FILE             *
**************************************


Quick help: how to use this asset?
----------------------------------

2 ways to use the asset:

1) Right click in the Hierarchy and select Effects -> Transitions Plus.
This will create a Transitions Plus gameobject with the main component. Customize style and behaviour settings as you wish. Play to run the transition.

2) Using C#: call TransitionAnimator.Start(...) with the desired profile or parameters.

Please check the demo scenes for examples of the two usages above.


Help & Support Forum
--------------------

Check the Documentation folder for detailed instructions.

Have any question or issue?
* Support-Web: https://kronnect.com/support
* Support-Discord: https://discord.gg/EH2GMaM
* Email: contact@kronnect.com
* Twitter: @Kronnect

If you like the asset, please rate it on the Asset Store. It encourages us to keep improving it! Thanks!



Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of the asset will be eventually available on the Asset Store.



More Cool Assets!
-----------------
Check out our other assets here:
https://assetstore.unity.com/publishers/15018



Version history
---------------

4.2
- Added "useUnscaledTime" option

4.1
- Inspector improvements and previews of camera targets
- Fade to camera improvements in Editor time

4.0
- Added "Melt" effect
- Added "Cube" effect
- Added "Contrast" option to Double Slide effect

3.0
- Added Smear effect
- Added Slide effect
- Added Double Slide effect

2.6
- Added Follow Position Offset parameter

2.5
- Added Splits parameter to Shape effect
- Added another effect to On Demand demo scene (button Cartoon 2)
- Added new demo scene with a combined transition effect
- Added Sorting Order parameter to Transition Animator inspector

2.0
- Added shape fade effect (uses a SDF texture for the custom shape)
- Added onTransitionEnd event to inspector

1.4
- Added "autoFollow" option. This option will center the transition on a target gameobject.
- Added "Time Multiplier" option to profile. Useful to adjust how the overall progress (0-1) impacts the animation. Some animation options require certain multiplier so the coverage from empty to full matches the 0-1 range perfectly.

1.3
- Added "autoPlay" option. Use SetProgress() method to specify current progress.

1.2
- Added VR support

1.1
- Ability to render effects inside a custom UI
- [Fix] API: fixed an parameter bug with the static function TransitionAnimator.Start

1.0 Sep/2023 First launch
