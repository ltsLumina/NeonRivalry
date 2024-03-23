using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ensures all the necessary components are attached to the object.
/// Also sets the name of the object to the name of the currently used icon.
/// </summary>
[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Image)), RequireComponent(typeof(ContentSizeFitter)), DisallowMultipleComponent]
public class ButtonPrompt : MonoBehaviour
{
    [Tooltip("Appends the name of the icon to the object's name.")]
    [SerializeField] bool appendIconName;

    /// <summary>
    /// Toggles the state of the prompt.
    /// </summary>
    /// <param name="enabled">true = visible.</param>
    public void Toggle(bool enabled) => gameObject.SetActive(enabled);

    void OnValidate()
    {
        // If the gameobject has been duplicated, remove the '(X)' at the end of the name.
        if (gameObject.name.EndsWith(")"))
        {
            int index                       = gameObject.name.LastIndexOf('(');
            if (index >= 0) gameObject.name = gameObject.name[..index].Trim();
        }
        
        if (!appendIconName)
        {
            // Remove the appended icon name.
            gameObject.name = gameObject.name.Split('[')[0].Trim();
            return;
        }

        string iconName;
        if (TryGetComponent(out Image image)) iconName = image.sprite.name;
        else return;

        string newName = $"{gameObject.name} [{iconName}]";
        
        // Ensure the name is not already appended.
        if (gameObject.name.Contains(iconName)) return;
        gameObject.name = newName;
    }
}
