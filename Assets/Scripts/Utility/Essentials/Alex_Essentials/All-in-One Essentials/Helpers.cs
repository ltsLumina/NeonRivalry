using UnityEngine;

namespace Essentials
{
    public static class Helpers
    {
        /// <summary>
        ///     Returns the main camera so that you don't have to use Camera.main every time.
        ///     i.e, instead of writing 'Camera.main' or caching it as a variable, you can just write 'Helpers.Camera'.
        ///     Optionally, you can then import the namespace 'using static Helpers;' to use it as 'Camera' instead.
        /// </summary>
        static Camera camera;
        public static Camera Camera
        {
            get {
                if (camera == null) camera = Camera.main;
                return camera;
            }
        }
    }
}
