# Multi Camera Events

## Contact
If you have any questions or feedback, please contact me at reslav.hollos@gmail.com.  
You can also join the [Discord server](https://discord.gg/uFEDDpS8ad)  

## Description
The purpose of this asset is to solve issues with camera events that arise when using
dual-camera systems (where one camera renders to a texture and another camera renders that texture),
which seems to produce incorrect event behaviour.

Concretely this solves the events for the asset [UPixelator](https://assetstore.unity.com/packages/slug/243562)
which uses the above described technique to achieve pixelation with subpixel offset.

## How to test
Drag and drop `/Abiogenesis3d/UPixelator/Prefabs/UPixelator` into the scene under the `./Example/Scenes` folder.
Each cube is rendered by a separate camera, hover them and see the changes made by scripts that depend on mouse events.

## How it works
Unity mouse events on cameras are skipped with `cam.eventMask = 0` because they are incorrect when rendering to a texture.
Correct mouse events are then recreated with custom raycasting and emitted with SendMessageUpwards on collided gameObjects.

Recreating events makes it easy to add additional control like:
- Being blocked by UI
- Per camera raycast distance
- Per camera raycast layer

## Recreated Events
- OnMouseDown
- OnMouseDrag
- OnMouseEnter
- OnMouseExit
- OnMouseOver
- OnMouseUp
- OnMouseUpAsButton

If you find a case where behavior differs from non dual-camera system let me know.
