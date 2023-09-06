#region
using System;
using UnityEngine;
#endregion

/// <summary>
///     A custom class that extends the functionality of the RigidbodyConstraints2D class.
///     Now allows you to freeze and unfreeze individual constraints.
///     <remarks>
///         Recommend to import this class as a static member to avoid having to type the full name of the class
///         every time you want to use it. Is is also possible to freeze multiple constraints at once using the bitwise OR "|" operator.
///     </remarks>
///     <example> rigidBody2D.FreezeConstraints(RigidbodyConstraints2DExtended.Constraints.FreezeX); </example>
/// </summary>
public static partial class RigidbodyConstraints2DExtended
{
    public static void FreezeConstraints
        (this Rigidbody2D rigidbody, Constraints constraints) =>
        rigidbody.constraints |= (RigidbodyConstraints2D)constraints;

    public static void UnfreezeConstraints
        (this Rigidbody2D rigidbody, Constraints constraints) =>
        rigidbody.constraints &= ~(RigidbodyConstraints2D)constraints;

    public static void FreezeAllConstraints
        (this Rigidbody2D rigidbody) => rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

    public static void UnfreezeAllConstraints
        (this Rigidbody2D rigidbody) => rigidbody.constraints = RigidbodyConstraints2D.None;
}

public static partial class RigidbodyConstraints2DExtended
{
    [Flags]
    public enum Constraints
    {
        None = 0,
        FreezeX = 1        << 0,
        FreezeY = 1        << 1,
        FreezeRotation = 1 << 2,
        FreezePosition = FreezeX   | FreezeY,
        FreezeAll = FreezePosition | FreezeRotation,
    }
}