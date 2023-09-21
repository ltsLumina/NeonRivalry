using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeviceManager : MonoBehaviour
{
    Dictionary<int, InputDevice> playerDevices = new ();
    List<InputDevice> unassignedDevices = new ();

    void Awake() => InputSystem.onDeviceChange += DeviceChangeHandler;

    void DeviceChangeHandler(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                // Add the new device to the pool of unassigned devices
                unassignedDevices.Add(device);
                break;

            case InputDeviceChange.Removed:
                // Remove the device from both the unassigned pool and the player's assigned devices
                unassignedDevices.Remove(device);
                var item = playerDevices.FirstOrDefault(kvp => kvp.Value == device);
                if (item.Key != null) { playerDevices.Remove(item.Key); }
                break;
        }
    }

    // Call this method when a player performs action with a device, it will assign that device to the player
    public void AssignDeviceToPlayer(int playerId, InputDevice device)
    {
        if (unassignedDevices.Contains(device))
        {
            unassignedDevices.Remove(device);
            playerDevices[playerId] = device;
        }
    }
}