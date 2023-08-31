#region
using System;
using UnityEngine;
#endregion

public class InputVisualizer : MonoBehaviour
{
    [SerializeField] float cursorSpeed = 5f;       // Speed at which the cursor moves
    [SerializeField] GameObject cursorBackground; // Reference to the background object
    [SerializeField] Transform cursor;            // Reference to the cursor object
    
    [Serializable]
    public struct GridPoint
    {
        public string name;
        public Vector2 position;
    }

    [SerializeField] GridPoint[] gridPoints;

    Vector2 backgroundPosition;
    InputManager inputManager;

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        
        // Store the initial position of the background object
        backgroundPosition = cursorBackground.transform.position;
    }

    void Update()
    {
        // Get the input values
        float horizontalInput = inputManager.MoveInput.x;
        float verticalInput   = inputManager.MoveInput.y;

        // Calculate the movement direction
        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // Determine the target position based on the input and the grid points
        Vector2 targetPosition = CalculateTargetPosition(moveDirection);

        // Move the cursor towards the target position (plus the backgroundPos) using lerp
        cursor.position = Vector2.Lerp(cursor.position, backgroundPosition + targetPosition * 1.75f, cursorSpeed * Time.fixedDeltaTime);
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
