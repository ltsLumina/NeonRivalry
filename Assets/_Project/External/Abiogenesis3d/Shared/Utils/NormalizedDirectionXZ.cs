using UnityEngine;

namespace Abiogenesis3d
{
    public partial class Utils
    {
        public static Vector3 NormalizedDirectionXZ(Vector3 direction)
        {
            direction.y = 0;
            direction.Normalize();
            return direction;
        }
    }
}
