using Lumina.Essentials.Modules;
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
    static bool hasPlayed;
    
    public static bool IsPlaying => timeline.state == PlayState.Playing;
    
    void Awake()
    {
        // If previous scene was character select, reset the timeline.
        if (SceneManagerExtended.PreviousScene == SceneManagerExtended.CharacterSelect)
        {
            debugPlay = true;
            hasPlayed = false;
        }
        
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
        //hasPlayed = false;
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
        if (hasPlayed) return;
        
        if (!play) return;
        
        if (timeline == null)
        {
            Debug.LogWarning("No timeline was found. Please assign a timeline to this script." + "\n");
            return;
        }
        
        // Disable the colliders of the camera while the timeline is playing.
        var colliders = Helpers.CameraMain.GetComponents<BoxCollider>();

        foreach (BoxCollider collider in colliders)
        {
            collider.enabled = false;
        }
        
        // Disable the UI Camera while the timeline is playing.
        GameObject.FindWithTag("UI Camera").GetComponent<Camera>().enabled = false;
        
        timeline.Play();

        hasPlayed = true;
    }

    static void TimelineStopped(PlayableDirector director)
    {
        foreach (var player in PlayerManager.Players)
        {
            player.DisablePlayer(false);
        }

        // Disable the colliders of the camera while the timeline is playing.
        var colliders = Helpers.CameraMain.GetComponents<BoxCollider>();

        foreach (BoxCollider collider in colliders) { collider.enabled = true; }

        var UICam = GameObject.FindWithTag("UI Camera").GetComponent<Camera>();
        if (UICam != null) UICam.enabled = true;
    }
}
