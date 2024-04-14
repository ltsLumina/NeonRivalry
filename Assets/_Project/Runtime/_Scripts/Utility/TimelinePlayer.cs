using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    [SerializeField] PlayableDirector sceneTimeline;
    [SerializeField] bool debugPlay;
    [Space]
    [SerializeField] public UnityEvent onTimelineEnd;
    
    static PlayableDirector timeline;
    static bool play;
    
    public static bool IsPlaying => timeline.state == PlayState.Playing;

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
        play = debugPlay;
    }

    void OnEnable() => timeline.stopped += TimelineStopped;
    void OnDisable() => timeline.stopped -= TimelineStopped;

    void Update()
    {
        if (IsPlaying)
        {
            foreach (var player in PlayerManager.Players)
            {
                player.DisablePlayer(true);
            }
        }
    }

    public static void Play()
    {
        if (!play) return;
        
        if (timeline == null)
        {
            Debug.LogWarning("No timeline was found. Please assign a timeline to this script." + "\n");
            return;
        }
        
        timeline.Play();
    }

    static void TimelineStopped(PlayableDirector director)
    {
        foreach (var player in PlayerManager.Players)
        {
            player.DisablePlayer(false);
        }
    }
}
