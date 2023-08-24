# Alex Essentials
This is the original Lumina's Essentials package created by  me, [**@ltsLumina**] which contains a collection of scripts designed to improve your Unity workflow and enhance your project development.

> Authored, developed and updated by [**@ltsLumina**] on GitHub (https://github.com/ltsLumina)

# Attributes

*The Attributes namespace is a set of tools aimed at giving developers easy customization and control over fields in the
inspector.*

> All credit goes to [**@heisarzola**] on [GitHub](https://github.com/heisarzola)
> for creating the RangedFloat attribute and the drawers. I simply added them to my essentials package for easy access.

**Attributes:**
![attribute example.gif](..%2F..%2FEditor%2FUI%2FImgs%2Fattribute%20example.gif)

## General Notes

- ReadOnly Attribute is used to make a field visible but not editable in the inspector.
  - Usage: [ReadOnly] or [SerializeField, ReadOnly]
- RangedFloat allows developers to assign a range between minimum and maximum float values.
  - Usage: [RangedFloat(min, max, RangeDisplayType)]
- There are three types of display for the range: LockedRanges, EditableRanges, and HideRanges.
  - LockedRanges: range is not editable.
  - EditableRanges: user can edit the range.
  - HideRanges: range is not visible hence locked.
- The GetRandomValue() method fetches a random value between the assigned min and max float values.

## Example:

```csharp
    [Header("ReadOnly Attribute")]
    [SerializeField, ReadOnly] private int readOnlyInt;

    [Header("Ranged Float Without Attribute")]
    [SerializeField] RangedFloat exampleOne;

    [Header("Ranged Float With Locked Limits")]
    [RangedFloat(5, 25, RangedFloatAttribute.RangeDisplayType.LockedRanges)]
    [SerializeField] RangedFloat exampleTwo;

    [Header("Ranged Float With Editable Limits")]
    [RangedFloat(0, 10, RangedFloatAttribute.RangeDisplayType.EditableRanges)]
    [SerializeField] RangedFloat exampleThree;

    [Header("Ranged Float With Hidden (But Locked) Limits")]
    [RangedFloat(5, 10, RangedFloatAttribute.RangeDisplayType.HideRanges)]
    [SerializeField] RangedFloat exampleFour;
```

If any doubt on how to use the attributes arises, please see the provided examples for reference. (Examples.cs)

# Sequencer
*The Sequencer is a tool that allows you to create a sequence of actions that can be triggered by a single call.*

**Sequencer:**
![sequencing example.gif](..%2F..%2FEditor%2FUI%2FImgs%2Fsequencing%20example.gif)

## General Notes

- The Sequencer is a tool that allows you to create a sequence of actions that can be triggered by a single call.
- Don't forget to make use of anonymous methods and lambda expressions to make your code more readable.
- All methods are executed in the same order they are called.
  - i.e. Execute(First).Execute(Second).Execute(Third) will execute First, then Second, then Third.
  - i.e. Execute(First).WaitForSeconds(2f).Execute(Second) will execute First, then wait 2 seconds, then execute Second.
- Take a look at the provided examples script for reference. (Examples.cs)

## Example:

```csharp
        // Example 1 -- This is the recommended way to use Sequencing.
        Sequence sequenceOne = Sequencing.CreateSequence(this); // Creates a sequence.
        sequenceOne.Execute(FirstExample).WaitForSeconds(2f).Execute(SecondExample); 


        // Example 2 -- A longer example of how to create longer sequences.
        Sequence sequenceTwo = Sequencing.CreateSequence(this);
        sequenceTwo.WaitThenExecute(5f, ExampleMethod3).ContinueWith(() => Debug.Log("Finished!")).WaitForSeconds(2f).Execute(FirstExample);


        // Example 3 -- An example of how you can make a more readable sequence.
        Sequencing.CreateSequence(this)
                  .Execute(() => Debug.Log("Hello World!"))
                  .WaitForSeconds(3f)
                  .ContinueWith(() => Debug.Log("Goodbye World!"));
```

# Helpers
*The Helpers class is a set of tools aimed at giving developers easy access to common methods.*

## General Notes

- The Helpers class includes a variety of methods that are commonly used in Unity projects.
- The class is a static class, meaning you do not need to create an instance of it to use it.
- Most notably, the Helpers class includes a `CameraMain` property that allows you to call `Camera.main` without it being an expensive call, as it caches the main camera after the first use. 
- Take a look at the provided examples script for reference. (Examples.cs)

## Example:

```csharp
        // Example 1
            Helpers.CameraMain.transform.position = Vector3.zero;
        //rather than:
            Camera.main.transform.position = Vector3.zero;
        //and caching the camera in Start() or Awake().
                
        // Example 2
        Helpers.CameraMain.orthographicSize = 5;
        
        // Example 3
        float lerp = Mathf.Lerp(Helpers.CameraMain.transform.position.x, 5, 0.5f);
        
        // All of the above examples are more efficient than using Camera.main directly.
        // Depending on the scope of your project, this can make a significant difference.
```

# Shortcuts

*The Shortcuts class consists of a couple of useful shortcuts for performing common tasks more efficiently through hotkeys.*

**Shortcuts Class:**
![shortcuts example.gif](..%2F..%2FEditor%2FUI%2FImgs%2Fshortcuts%20example.gif)

## General Notes

- The Shortcuts class is very basic but can be very useful thanks to the hotkeys.
- It is not restricted to hotkeys, however, and can be used as normal methods in your code.
- Take a look at the provided examples script for reference. (Examples.cs)

## Example:

```csharp
        // Example 1
        Shortcuts.ClearConsole(); // Clears the console.
        
        // Example 2
        Shortcuts.ReloadScene(); // Reloads the current scene.
        
        // Both of these methods have customizable hotkeys inside the Shortcuts.cs script file.
        // By default they are: Ctrl + Shift + C and Ctrl + Shift + R respectively.
```

# Included in Extras ("Full Package")

## Examples

**Examples Class:**
![examples example.gif](..%2F..%2FEditor%2FImgs%2Fexamples%20example.gif)

## General Notes

- The Examples class is aimed at giving developers an easy way to understand and parse the Lumina's Essentials package.
- The Examples class is a collection of methods that easily demonstrate how to use the various features of the package.
- Take a look at the provided examples script for reference. (Examples.cs)

## Misc

*The Misc namespace is a set of tools aimed at giving developers easy access to common methods.*

> Authored, developed and updated by [**@ltsLumina**] on GitHub

**Misc Class:**

## General Notes

- The short but sweet Misc folder provides some obscure but useful methods.
- Namely the CameraShake method, specifically designed for the Cinemachine package, as well as the RigidbodyConstraints2DExtended class, which allows you to set Rigidbody2D constraints with more control than the default Unity implementation.
- The Examples class does **not** include an example of the CameraShake method or the RigidbodyConstraints2DExtended class, but it is very simple to use.

## Example:

```csharp
        // Example 1 -- This is the recommended way to use Misc.
        CameraShake.ShakeCamera(intensity, time,); // Shakes the camera with the given parameters.
```

## Initialization

*The Initialization class is a single script that allows you to customize all your singletons from one place, and then initialize them all before scene load; ensuring they are always loaded at the start of the game.*

**Initialization Class:**

## General Notes

- The Initialization class is a set of tools aimed at giving developers easy access to common methods.
- The Initialization class is a static class, meaning you do not need to create an instance of it to use it.
- The Initialization class is a collection of methods that are commonly used in Unity projects.
- The Initialization class is not included in the Examples.cs script, but will be in a future update.

