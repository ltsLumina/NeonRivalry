TOP 10 Unity Code Optimization Tips; curated by me :)

1. Avoid FindObjectOfType:
Instead of using FindObjectOfType to locate a component at runtime, consider using serialized references or cached references in your scripts. This reduces the overhead of searching for components dynamically, resulting in better performance.

2. Minimize GameObject.Find:
GameObject.Find methods can be costly, especially when called frequently. Opt for alternative approaches such as assigning references through the Inspector or using object pooling techniques to reduce the need for frequent GameObject.Find calls. Additionally, avoid using SendMessage and BroadcastMessage as they are also expensive.

3. Use object pooling:
Object pooling helps avoid expensive object instantiation and destruction operations. By reusing objects instead of creating and destroying them repeatedly, you can significantly improve performance in scenarios like spawning projectiles or particles.

4. Prefer non-alloc versions of Unity physics methods:
Using non-alloc versions of methods in Unity, such as Physics.OverlapSphereNonAlloc, reduces memory allocations and garbage collection overhead. By providing a pre-allocated array for storing results, you can reuse the same array across frames, improving performance.

5. Avoid excessive use of Camera.main:
While Camera.main provides a convenient way to access the main camera in your scene, it is recommended to avoid excessive usage in performance-critical situations. Although accessing Camera.main has a small overhead, it is not as slow as some might believe, especially when used sparingly or in non-performance-sensitive scenarios.

6. Optimize Update calls:
Instead of using MonoBehaviour.Update for every script, consider using a centralized script with fewer Update calls that manages and delegates the necessary logic to other scripts. This reduces the overhead of multiple Update calls and improves performance.

7. Use object pooling for particle systems:
Particle systems can be performance-intensive. Implement object pooling techniques to reuse particle systems instead of instantiating and destroying them frequently.

8. Limit the use of string concatenation (e.g: $"this is a string with {stringConcatenation}"):
String concatenation operations can be expensive, especially when performed frequently. Use StringBuilder or string.Format instead of repeated concatenation to optimize string operations.

9. Optimize physics interactions:
Minimize the number of physics interactions and use appropriate physics layers and colliders to limit unnecessary calculations.
Use FixedUpdate instead of Update for physics-related operations to ensure consistent and optimized physics simulation.

10. Optimize resource loading:
Load and unload resources efficiently to minimize memory usage. Use asset bundles, resource pooling, or asset streaming techniques to manage resource loading and unloading dynamically.

By following these Unity code optimization tips, you can enhance the performance and efficiency of your Unity projects, resulting in smoother gameplay, reduced CPU and GPU overhead, and improved user experience. Remember to profile your application regularly to identify performance bottlenecks and fine-tune your optimizations accordingly.

!!! KEEP IN MIND !!!
Optimizing your code is a good practice, but it is not always necessary. If your game is running smoothly and you are not experiencing any performance issues, then you don’t need to optimize your code. "Premature optimization is the root of all evil" - Donald Knuth

P.S, you can make all objects reset their transform automatically upon creation by going into Edit > Preferences > Scene View > Create Objects at Origin. :)