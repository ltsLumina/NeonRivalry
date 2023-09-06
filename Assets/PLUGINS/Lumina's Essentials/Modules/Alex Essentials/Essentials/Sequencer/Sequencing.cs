#region
using UnityEngine;
#endregion

namespace Lumina.Essentials.Sequencer
{
/// <summary>
///     Provides utilities for creating and processing sequences of actions.
///     This class serves to provide a simple way to create and process sequences of actions.
/// </summary>
public static class Sequencing
{
    /// <summary>
    ///     Creates a new <see cref="Sequence" /> instance.
    /// </summary>
    /// <param name="host">The <see cref="MonoBehaviour" /> on which the sequence is hosted.</param>
    /// <returns>A new <see cref="Sequence" /> instance.</returns>
    public static Sequence CreateSequence(MonoBehaviour host) => new (host);
}
}
