#region
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.UI;
using Logger = Lumina.Debugging.Logger;
#endregion

public class InputVisualizer : MonoBehaviour
{
    [Header("Reference"), ReadOnly]
    [SerializeField] InputManager inputManager;
    
    [Header("Settings")]
    [SerializeField] float cursorSpeed;       
    [SerializeField] Image cursorBackground;
    [SerializeField] Transform cursor;
    [Space(10)]
    [SerializeField] GridPoint[] gridPoints;

    [Serializable, UsedImplicitly]
    public struct GridPoint
    {
        // "name" is only used in inspector to identify which point is which
        // These variables are marked as ReadOnly to prevent accidental changes in inspector.
        // If they happen to be reset and you wish to change them, simply remove the ReadOnly attribute.
        [ReadOnly] public string name;
        [ReadOnly] public Vector2 position;
    }
    
    Vector2 backgroundPosition;
    readonly static List<InputManager> assignedInputManagers = new ();

    void Awake() => assignedInputManagers.Clear();
    
    void OnEnable() => AssignInputManager();

    void AssignInputManager()
    {
        // Find an InputManager that isn't already assigned
        InputManager[] inputManagers = FindObjectsOfType<InputManager>();

        foreach (InputManager manager in inputManagers)
        {
            if (assignedInputManagers.Contains(manager) || !manager.enabled) continue;
            inputManager = manager;
            assignedInputManagers.Add(manager);
            
            // Update the name of the game object to match the player ID
            gameObject.name = $"Input Visualizer (Player {inputManager.GetComponentInParent<PlayerController>().PlayerID})";
            
            return;
        }

        // If no InputManager is found or the component is disabled, disable the game object
        gameObject.SetActive(false);
        Logger.Debug($"No InputManager found. \n Disabling {gameObject.name}...");
    }

    void UnassignInputManager()
    { 
        // If the component is disabled, release the assigned InputManager so another can use it
        if (inputManager == null) return;
        assignedInputManagers.Remove(inputManager);
        inputManager = null;
    }
    
    void OnDisable() => UnassignInputManager();

    void Start()
    {
        // Store the initial position of the background object
        backgroundPosition = cursorBackground.transform.position;
    }

    void Update()
    {
        if (inputManager == null)
        {
            // Look for an InputManager if none is assigned
            AssignInputManager();

            // Exit from Update if no InputManager found
            if (inputManager == null) return;
        }
        
        // Get the input values
        float horizontalInput = inputManager.MoveInput.x;
        float verticalInput   = inputManager.MoveInput.y;

        // Calculate the movement direction
        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // Determine the target position based on the input and the grid points
        Vector2 targetPosition = CalculateTargetPosition(moveDirection);

        // Move the cursor towards the target position (plus the backgroundPos) using lerp
        cursor.position = Vector2.Lerp(cursor.position, backgroundPosition + targetPosition * 0.65f, cursorSpeed * Time.fixedDeltaTime);
    }

    Vector2 CalculateTargetPosition(Vector2 moveDirection)
    {
        // Get the closest grid point based on the move direction
        int closestGridPointIndex = GetClosestGridPointIndex(moveDirection);

        // Return the position of the closest grid point
        return gridPoints[closestGridPointIndex].position * 50;
    }

    int GetClosestGridPointIndex(Vector2 moveDirection)
    {
        float minDistance  = float.MaxValue;
        int   closestIndex = 0;

        for (int i = 0; i < gridPoints.Length; i++)
        {
            float distance = Vector2.Distance(moveDirection, gridPoints[i].position);

            if (distance < minDistance)
            {
                minDistance  = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
