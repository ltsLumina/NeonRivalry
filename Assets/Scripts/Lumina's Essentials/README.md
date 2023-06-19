# A collection of scripts to make life with Unity a little more bearable.
> Written and customized by me, along with features from Joel's essentials.

## TL;DR Features:
#### My personal collection of scripts; "Alex_Essentials".
- Sequencing methods, Delayed action methods, and a ReadOnly attribute.
- Cinemachine camera shake instance calls for easy use.
- Extension for the RigidbodyConstraints2D class for better control.
- Initialization script for manager classes to always keep track of them.
- Helper methods for general purposes.

#### Joel's own essentials folder.
- Create singleton patterns for your classes easily.
- Object pooling system.
- Music manager for controlling music only. (No sound effect support)
- An extension to the SceneManager class in Unity. Includes commonly used scene management methods.

# Alex Essentials includes:

### The All-in-One Essentials script. 
Features a sequencing method for creating action sequences, a method for running an action after a delay, as well as a ReadOnly attribute that allows you make readonly variables in the inspector. The sequencing and delay methods both feature an async/await counterpart, in case you wish to use those instead. 
> There is also a class for deprecated features that still work if you are using the old versions of this essentials package.

### A simple camera shake script for Cinemachine.
Simple script for making a Cinemachine Virtual camera shake using the 6D Shake cinemachine component.

### An extension to Unity's RigidbodyConstraints2D.
A class that allows you to freeze/unfreeze individual constraints, rather than Unity's default setting of unfreezing all at once, which is known to cause problems. This class also allows you to call the methods directly from your Rigidbody2D reference.
Example: `myRigidBody2D.FreezeConstraints(RigidbodyConstraints2DExtended.Constraints.FreezeX);`

**It is highly recommended to import the Constraints class as a static member to avoid writing out the full syntax every time.**

The method also allows you to freeze/unfreeze multiple constraints at once using the bitwise " **|** " (or) operator.
Example: `myRigidBody2D.FreezeConstraints(FreezeX | FreezeY);`

### Project Initialization
A simple, but very powerful script that loads a prefab that includes all your manager scripts. The script runs before the scene has loaded and creates a DontDestroyOnLoad prefab by name "Systems" with childed GameObjects containing your managers. By doing it this way, you always have a singleton reference for all your managers, while still being able to serialize fields to the inspector. It also circumvents the need to create a preload scene with all your managers as this script runs before scene load.
> Keep in mind before use!: You **MUST** create a prefab by name `Systems` and place it in the `Resources` folder in the Alex_Essentials folder for this to work. The prefab should be an empty game object, and each of your manager scripts should be placed on their own gameobject, childed to the Systems parent. 
[Example of how it should look in the editor.](https://gyazo.com/439639ce3e9e79f4e8f4baad023df8cd) (The Music Player in the image is only for example, not required.)

### A general purpose Helper Class
Includes a few random, yet very helpful methods that people find themselves using in almost every single project. Find yourself caching a reference to the camera in almost every script? Worry no more, as you can call Helpers.Camera to access the main camera without it being an expensive method call as we cache it after the first use.

# Notes
#### This essentials package has only been tested by me on my personal project, so in case something breaks, let me know :)
