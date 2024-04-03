using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    [SerializeField] PlayableDirector sceneTimeline;
    
    static PlayableDirector timeline;

    void Awake()
    {
        if (sceneTimeline != null) return;

        // If there are multiple timelines, return.
        var timelines = GetComponents<PlayableDirector>();
        if (timelines.Length > 1)
        {
            Debug.LogError("Multiple timelines were found in this scene. Please assign a timeline to this script." + "\n");
            return;
        }
            
        // Otherwise, assign the timeline to the one in the scene.
        sceneTimeline = GetComponent<PlayableDirector>();

        // Set the static timeline to the scene timeline.
        timeline = sceneTimeline;
    }

    public static void Play()
    {
        if (timeline == null)
        {
            Debug.LogWarning("No timeline was found. Please assign a timeline to this script." + "\n");
            return;
        }
        
        timeline.Play();
    }
}
