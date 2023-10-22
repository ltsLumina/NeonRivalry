#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FLZ.Utils
{
    public abstract class Time
    {
        public static bool RateLimiter(int frequency)
        {
            return UnityEngine.Time.frameCount % frequency == 0;
        }
    }
    
#if UNITY_EDITOR
    public struct TimeSinceEditor
    { 
        float time; 
	
        public static implicit operator float(TimeSinceEditor ts) 
        { 
            return (float)EditorApplication.timeSinceStartup - ts.time; 
        }

        public static implicit operator TimeSinceEditor(float ts) 
        { 
            return new TimeSinceEditor { time = (float)EditorApplication.timeSinceStartup - ts }; 
        } 
    }
#endif
    
    public struct TimeSince 
    { 
        float time; 
	
        public static implicit operator float(TimeSince ts) 
        { 
            return UnityEngine.Time.time - ts.time; 
        }

        public static implicit operator TimeSince(float ts) 
        { 
            return new TimeSince { time = UnityEngine.Time.time - ts }; 
        } 
    }
}