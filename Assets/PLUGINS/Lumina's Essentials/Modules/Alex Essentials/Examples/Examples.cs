#region
using System;
using System.Collections;
using UnityEngine;
using Lumina.Essentials.Modules;
using Lumina.Essentials.Sequencer;
using Lumina.Essentials.Attributes;
using Sequence = Lumina.Essentials.Sequencer.Sequence;
#endregion

// Disables the warning from the use of the Deprecated class.
#pragma warning disable 0618

/// <summary>
///     <remarks>
///         None of the methods here are intended to be used in production.
///         They are simply examples of how to use the attributes and methods provided by Lumina's Essentials. !!
///     </remarks>
/// </summary>
public sealed class Examples : MonoBehaviour
{
    #region Attributes
    ////////////////////////////
    //   ReadOnly Attribute   //
    ////////////////////////////

    #region Readonly Attribute
    [Header("ReadOnly Attribute")]
    
    // A simple example of how to use the ReadOnly attribute. // Use a static namespace import to make this easier to read. (Rather than Attributes.ReadOnly)
    [SerializeField, ReadOnly] bool readOnlyBool;

    // A simple example of how to use the ReadOnly attribute to display a property in the inspector but not allow it to be edited.
    [SerializeField, ReadOnly] int health;
    public int Health
    {
        get => health;
        set => health = value;
    }
    #endregion

    ////////////////////////////
    // Ranged Float Attribute //
    ////////////////////////////

    #region Ranged Float Attribute
    [Space(25), Header("Ranged Floats")] 
    
    [Header("Ranged Float Without Attribute"), SerializeField] 
     RangedFloat exampleOne;

    [Header("Ranged Float With Locked Limits"), RangedFloat(2, 25), SerializeField]
     RangedFloat exampleTwo;

    [Header("Ranged Float With Editable Limits"), RangedFloat(0, 1, RangedFloatAttribute.RangeDisplayType.EditableRanges), SerializeField]
     RangedFloat exampleThree;

    [Header("Ranged Float With Hidden (But Locked) Limits"), RangedFloat(5, 10, RangedFloatAttribute.RangeDisplayType.HideRanges), SerializeField]
     RangedFloat exampleFour;
    #endregion
    // End of Attributes //
    #endregion

    #region Sequencer
    // Sequencing Examples //
    void Example()
    {
        // Example 1 -- This is the recommended way to use Sequencing.
        Sequence sequenceOne = Sequencing.CreateSequence(this);
        sequenceOne.Execute(FirstExample).WaitForSeconds(2f).Execute(SecondExample);


        // Example 2 -- An example of how to create longer sequences.
        Sequence sequenceTwo = Sequencing.CreateSequence(this);
        sequenceTwo.WaitThenExecute(5f, ThirdExample).ContinueWith(() => Debug.Log("Finished!")).WaitForSeconds(2f).Execute(FirstExample);


        // Example 3 -- An example of how you can make a more readable sequence.
        Sequencing.CreateSequence(this)
                  .Execute(() => Debug.Log("Hello World!"))
                  .WaitForSeconds(3f)
                  .ContinueWith(() => Debug.Log("Goodbye World!"));
    }

    // End of Sequencing Examples //
    #endregion
    
    #region Helpers

    ////////////////////////////
    //   Cached Camera.main   //
    ////////////////////////////

    // Allows you to call camera.main without it being an expensive call, as it gets cached after the first call.
    void CachedCameraMain()
    {
        // Rather than making a call to Camera.main or caching the camera in every script you need to reference it, you can use this cached version.
        // Helpers.CameraMain
        
        // Example of how to use it.
        Helpers.CameraMain.transform.position = Vector3.zero;
        Helpers.CameraMain.orthographicSize   = 5;
        float lerp = Mathf.Lerp(Helpers.CameraMain.transform.position.x, 5, 0.5f);
        // etc.
        
        // Don't forget to use the namespace import at the top of the script to make this easier to read. (Rather than Helpers.Camera, it becomes simply "Camera")
    }

    //////////////////////////////
    //  Play Audio Random Pitch //
    //////////////////////////////
    
    // Play an audio clip with a random pitch.
    void RandomPitch()
    {
        
        // Example components for the method.
        AudioSource audioSource = null;
        AudioClip   audioClip   = null;
        
        // Example of how to use it.
        Helpers.PlayRandomPitch(audioClip, audioSource, 0.5f, 1.5f);
    }

    ////////////////////////////
    //      Miscellaneous     //
    ////////////////////////////

    // Destroys all children of a transform.
    void DeleteAllChildrenOfObject()
    {
        // Example components for the method.
        Transform parent = null;
        
        // Example of how to use it.
        Helpers.DestroyAllChildren(parent);
        
        // Alternatively: // Can be used as an extension method of the transform class.
        parent.DestroyAllChildren();
    }

    // Returns a random Vector2 or 3 between the given min and max values. 
    void RandomVector()
    {
        Vector2 randomVector2 = Vector2.zero; // VECTOR 2 EXAMPLE
        Vector3 randomVector3  = Vector3.zero; // VECTOR 3 EXAMPLE
        
        // Includes an overload for Vector2 and Vector3.
        Helpers.RandomVector(randomVector2, 5, 10);
        Helpers.RandomVector(randomVector3, 2, 7);
        
        // Alternatively: // Can be used as an extension method of the Vector2 and Vector3 classes.
        randomVector2.RandomVector(5, 10);
        randomVector3.RandomVector(2, 7);
    }
    
    // End of Helpers //
    
    #endregion
    
    #region Shortcuts
    ////////////////////////////
    //      Clear Console     //
    ////////////////////////////
    
    // Clear the console through a keyboard shortcut. (Alt + C)
    // Can also be used as a method.
    void ClearConsole() => Shortcuts.ClearConsole();

    ////////////////////////////
    //      Reload Scene      //
    ////////////////////////////
    
    // Reload the scene through a keyboard shortcut. (Alt + Backspace)
    // Can also be used as a method.
    void ReloadScene() => Shortcuts.ReloadScene();
    
    // End of Shortcuts //
    #endregion
    
    #region Deprecated (Sequencing)
    ////////////////////////////
    //   Coroutine Examples   //
    ////////////////////////////

    void SequencingMethodsCoroutine()
    {
        // Coroutine example of how to sequence actions.
        IEnumerator sequenceActions = Deprecated.SequenceActions(
            () => FirstExample(), 
            2.5f, 
            () => SecondExample());

        // How to run the coroutine.
        StartCoroutine(sequenceActions);

        // Alternatively: // This eliminates the need for the IEnumerator variable, but it becomes harder to read.
        StartCoroutine(Deprecated.SequenceActions(() => FirstExample(), 2.5f, () => SecondExample()));
    }

    // Example of how to run a method after a delay using a coroutine in the event that invoke is not an option.
    void DelayedActionCoroutine()
    {
        // Coroutine example of how to delay an action.
        IEnumerator delayedAction = Deprecated.DelayedAction(() => FirstExample(), 2.5f);

        // How to run the coroutine.
        StartCoroutine(delayedAction);

        // Alternatively: // This eliminates the need for the IEnumerator variable, but it becomes harder to read.
        StartCoroutine(Deprecated.DelayedAction(() =>FirstExample(), 2.5f));
    }

    ////////////////////////////
    //     Async Examples     //
    ////////////////////////////

    // Method is async to allow for the use of await in the async example below.
    async void SequenceMethodsAsync()
    {
        // Task example of how to sequence actions.
        await Deprecated.SequenceActionsAsync(() => FirstExample(), 2.5f, () => SecondExample());

        // How to run the method.
        SequenceMethodsAsync();
    }

    // Method is async to allow for the use of await in the async example below.
    async void DelayedActionAsync()
    {
        // Task example of how to delay an action.
        await Deprecated.DelayedActionAsync(() => FirstExample(), 2.5f);

        // How to run the method.
        DelayedActionAsync();
    }
    // End of Deprecated //
    #endregion
    
    #region Example Methods (ignore)
    // Example Methods for the Sequencing Examples.
    void FirstExample() => Debug.Log("Example Method 1!");

    void SecondExample() => Debug.Log("Example Method 2!");

    void ThirdExample()
    { 
        Debug.Log("Example Method 3!");

        // Throws an exception to test the exception handling.
        throw new Exception("Example Exception!");
    }
    
    // End of Example Methods //
    #endregion
}
