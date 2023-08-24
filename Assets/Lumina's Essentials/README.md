![Luminas Essentials Logo.png](Editor%2FUI%2FImgs%2FLuminas%20Essentials%20Logo.png)

# Lumina's Unity Essentials Package

![Unity Version](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)

## Table of Contents
- [Overview](#overview)
- [TL;DR Features](#tldr-features)
- [Installation](#installation)
- [Getting Started](#getting-started)
- [Essentials Overview](#essentials-overview)
  - [Sequencer](#sequencer)
  - [Attributes](#attributes)
  - [Helpers](#helpers)
  - [Shortcuts](#shortcuts)
  - [Utility Window](#utility-window)
- [Joel's Essentials](#joels-essentials)
- [Contribution and Bug Reporting](#contribution-and-bug-reporting)
- [License](#license)

## Overview
Welcome to Lumina's Unity Essentials Package, a collection of scripts designed to improve your Unity workflow and enhance your project development. 

This package includes various utility features, shortcuts, and extensions to make your life with Unity a little more bearable. From a powerful Sequencer module to a convenient Utility Window, these essentials are designed to boost your productivity and streamline your game development process.

The package also features contributions from Joel's Essentials, providing additional handy tools to complement the collection.

## TL;DR Features
- A utility window with features to enhance your Unity workflow, such as creating default project directories for new projects and converting image settings for different scenarios.
- Quick access to enable "Enter Play Mode" options, allowing you to enter play mode much quicker.
- An auto-save feature for Unity that isn't normally available.
- A powerful Sequencer module to create sequences of methods, events, or any other actions, with support for running methods after a specified delay.
- Various attributes, including ReadOnly and Ranged Float features, for customizing variable display in the Unity inspector.
- A collection of useful Unity shortcuts, such as `ClearConsole()` and `ReloadScene()`, for performing common tasks more efficiently.
- A Helper Class with miscellaneous methods, such as caching `Camera.main` and playing audio with random pitches.

## Installation
1. Open your Unity project.
2. Install the package from the [Releases page](https://github.com/ltsLumina/Lumina-Essentials/releases/latest) on GitHub.
3. Download the latest release of the package.
4. In Unity, either open the package or drag and drop it in from wherever you saved it.
5. Ensure that **all** files are selected and click "Import."
6. You're done! You can now access the Utility Panel and start using the features provided by the package.

## Getting Started
To access the Utility Panel, go to "Tools" -> "Lumina" -> "Open Utility Panel" in the Unity editor. From there, you can explore and utilize various features, including the auto-save feature, enter play mode options, and auto-save.

All the Essentials are under the `Lumina.Essentials` namespace, with the Sequencer and Attributes being sub-namespaces.
Simply add the namespace(s) to any script you are writing and start using the methods provided!

For details on the available methods, attributes, and shortcuts provided by the package, check out the [documentation](Documentation.md).

## Essentials Overview

### Sequencer
The Sequencer module allows you to create sequences of methods, events, or any other actions you can think of. It supports running a method after a specified delay, performing callback actions, as well as various other functions. The implementation is designed through extension methods for ease of use. For example, you can create a sequence like this: 

```
Sequence mySequence = Sequencing.CreateSequence(this)
mySequence().Execute(ExampleMethod).WaitForSeconds(3f).ContinueWith(() => Debug.Log("Finished!"));
```
The provided example will function exactly as you expect. It will run the "ExampleMethod" function, then wait for 3 seconds using a coroutine, then once it has finished waiting it prints a "Finished!" message to the Unity console.
   - Note: The "`this`" inside of `CreateSequence()` is the 'host' of the coroutine. Unless you want a different MonoBehaviour to run your coroutine, always use '`this`'.

### Attributes
> The Attributes Package includes two essential attributes:
- **ReadOnly Attribute:** Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector. Use it like this: `[SerializeField, ReadOnly] bool readOnlyBool;`.
- **Ranged Float Attribute:** Allows you to add a range to a float variable in the inspector. It has various different uses so refer to the "Examples" folder in the package for help on how to use it.

The RangedFloat attribute provides a useful class for defining a range with min and max values. Additionally, an implicit operator is included, allowing you to fetch a random value within the range when a RangedFloat is used as a float. The RangedFloatAttribute provides options for how the range is displayed in the inspector, including locked ranges, editable ranges, and hidden ranges.

### Helpers
> The Helpers class provides miscellaneous helper methods that don't fit into any other category. Some of the essential helper methods include:
- **CameraMain:** A property that allows you to call `Camera.main` without it being an expensive call, as it caches the main camera after the first use. Use it like this: `Helpers.CameraMain.transform.position`.
- **PlayRandomPitch:** A method that plays an audio clip on the given audio source with a random pitch between the provided minimum and maximum values. This can add variation to your audio playback.
- **DestroyAllChildren:** A method that destroys all children of a given transform. This can be used as an extension method, making it easy to clean up child objects.
- **RandomVector:** Two overloads of the method that generate a random Vector2 or Vector3 within specified minimum and maximum values. This can be useful for generating random positions or directions in your game.

### Shortcuts
> The Shortcuts Class includes a collection of useful Unity shortcuts, making it easier to perform common tasks more efficiently.

Some of the included shortcuts are `ClearConsole()` and `ReloadScene()`, allowing you to clear the console or reload the scene with a hotkey.
These methods can also be used as normal in your code, they aren't limited as hotkeys.

### Utility Window
> The Utility Window provides quick access to various features that enhance your Unity workflow, including:
- Creating default project directories for new projects, helping you to maintain a standardized project structure.
- Converting image settings in Unity to be appropriate for any given scenario, simplifying the process of setting up image assets for different platforms or quality levels.
- Enabling "Enter Play Mode" options", allowing you to enter play mode much quicker and avoid unnecessary manual steps.
- Auto-save feature for Unity that isn't normally available, providing an added layer of protection against data loss.

## Joel's Essentials
> Joel's Essentials is a set of additional handy tools that complement Lumina's Unity Essentials Package. It includes the following features:

- **Singleton Class:** Lets you turn any MonoBehaviour into a singleton, ensuring that only one instance of the script exists in the scene.
- **Object Pooling System:** Allows you to easily create object pools for your Unity game, improving performance by reusing objects instead of instantiating and destroying them frequently.
- **SceneManagerExtended:** Extends the SceneManager class with various methods to easily load scenes, reload the current scene, load the next or previous scene, or even load a scene asynchronously.
- **Music System:** Streamlines the process of adding music to your game and includes a MusicTrigger system to play music upon touching a collider.

> Note: The Joel's Essentials part of the package is only available when selecting "Full Package" in the Utility Panel's Setup menu.

## Contribution and Bug Reporting
This essentials package has been tested on various projects, but if you encounter any issues or have suggestions for improvements, please don't hesitate to [submit an issue](https://github.com/ltsLumina/Lumina-Essentials/issues). Your feedback is valuable and helps in making this package even better!

## License
This package is distributed under the [MIT License](LICENSE), allowing you to use and modify it freely in your projects. If you find it helpful, consider giving credit by mentioning "Lumina's Unity Essentials Package" in your project's acknowledgments.

Happy developing! 🚀
