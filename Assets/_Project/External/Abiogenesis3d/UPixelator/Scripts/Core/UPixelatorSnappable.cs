using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abiogenesis3d
{
    [ExecuteInEditMode]
    public class UPixelatorSnappable : MonoBehaviour
    {
        [HideInInspector] public bool isCamera;

        [Header("Position")]
        // NOTE: this snaps to the grid defined by the pixel size
        public bool snapPosition = true;
        public bool snapWithParent = true;

        [Header("Rotation")]
        public bool snapRotation;
        // WIP
        [HideInInspector]
        public bool rotateToCamForward;
        public bool isLocalRotation;
        [Tooltip("Setting to 0 allows for custom values input")]
        [Range(0, 8)] public int divisions360 = 5;
        public Vector3 snapRotationAngles;
        // public Vector3 snapRotationOffset;
        // WIP
        [HideInInspector]
        public Vector3 camRotationOffset;

        [Header("Scale")]
        public bool snapLocalScale;
        [Range(0, 1)] public float snapScaleValue = 0.05f;

        Vector3 storedPosition;
        [HideInInspector] public Vector3 lastSnappedPosition;
        [HideInInspector] public Vector3 skippedSnappedPosition;

        Quaternion storedRotation;
        Vector3 storedLocalScale;

        public Vector3 initialPosition;

        // NOTE: needed because exiting play mode resets values before onEndCameraRendering calls reset so it resets to default 0 position
        bool storePositionDirty;
        bool storeRotationDirty;
        bool storeLocalScaleDirty;

        [Header("Experimental")]
        [Tooltip("Waits for more than a pixel size position diff to snap, use for moving objects not followed by the camera")]
        // public bool stabilizeDiagonal = false;

        [Header("RectTrasform only")]
        public bool smoothSnapUI = true;

        // NOTE: default to true because snapping should be accompanied with restoring otherwise it can lead to issues
        [NonSerialized] bool log = false;
        [NonSerialized] bool verboseLog = false;

        [HideInInspector] public List<UPixelatorSnappable> nested = new List<UPixelatorSnappable>();

        bool positionMutated;

        // NOTE: don't use transform.position when there's rectTransform, it does not serialize properly when
        //  reopening the project... what seems to serialize properly is rectTransform.anchoredPosition3D
        [HideInInspector] public RectTransform rectTransform;

        public Canvas canvas;
        [HideInInspector] public UPixelatorSnappableCanvasOverlay overlaySnappable;

        public void ValidatePositionBeforeRestore()
        {
            positionMutated = transform.position != lastSnappedPosition;
            var diff = transform.position - lastSnappedPosition;
            if (positionMutated) Log("Position <color=red>mutated</color>, diff (" + diff.x + ", " + diff.y + ", " + diff.z + ")");
        }

        void OnValidate()
        {
            if (divisions360 > 0)
                snapRotationAngles = Vector3.one * 360 / Mathf.Pow(2, divisions360);
        }

        // void Update()
        // {
        //     if (positionMutated)
        //     {
        //         Log("Position mutated last frame, skipping restore, " + name);
        //         positionMutated = false;
        //         storePositionDirty = false;
        //         return;
        //     }
        //     if (storePositionDirty)
        //     {
        //         Debug.Log("Position not unsnapped, restoring" + name);
        //         RestorePosition();
        //     }
        // }

        void OnEnable()
        {
            var uPixelator = FindObjectOfType<UPixelator>();
            if (uPixelator) uPixelator.isSnappablesDirty = true;

            rectTransform = GetComponent<RectTransform>();

            canvas = GetComponentInParent<Canvas>();
            if (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                if (!overlaySnappable)
                {
                    overlaySnappable = GetComponent<UPixelatorSnappableCanvasOverlay>();
                    if (!overlaySnappable)
                        overlaySnappable = gameObject.AddComponent<UPixelatorSnappableCanvasOverlay>();
                }
                overlaySnappable.enabled = true;
            }
            else
            {
                if (overlaySnappable) overlaySnappable.enabled = false;
            }
        }

        void Log(string str)
        {
            if (!log) return;
            // TODO: simplify Log calls, remove name as it's appended here
            Debug.Log(str + $" <color=#ffffcc>{name}</color>", gameObject);
        }

        public void StorePosition()
        {
            if (storePositionDirty) Log("Position <color=red>already stored</color>, waiting for restore");
            else
            {
                storedPosition = transform.position;
                storePositionDirty = true;
            }
        }

        public void RestorePosition()
        {
            if (positionMutated)
            {
                Log("Position <color=red>mutated</color>, skipping restore");
                positionMutated = false;
                storePositionDirty = false;
                return;
            }

            if (storePositionDirty) transform.position = storedPosition;
            else Log("Position <color=red>not stored</color>, skipping restore");
            storePositionDirty = false;
            if (verboseLog) Log("Restored position");
        }

        public void StoreRotation()
        {
            if (storeRotationDirty) Log("Rotation <color=red>already stored</color>, waiting for restore");
            else
            {
                if (isLocalRotation) storedRotation = transform.localRotation;
                else storedRotation = transform.rotation;
                storeRotationDirty = true;
            }
        }
        public void RestoreRotation()
        {
            if (storeRotationDirty) {
                if (isLocalRotation) transform.localRotation = storedRotation;
                else transform.rotation = storedRotation;
            }
            else Log("Rotation <color=red>not stored</color>, skipping restore");
            storeRotationDirty = false;
        }


        public void StoreLocalScale()
        {
            if (storeLocalScaleDirty) Log("Scale <color=red>already stored</color>, waiting for restore");
            else
            {
                storedLocalScale = transform.localScale;
                storeLocalScaleDirty = true;
            }
        }

        public void RestoreLocalScale()
        {
            if (storeLocalScaleDirty) transform.localScale = storedLocalScale;
            else Log("Scale <color=red>not stored</color>, skipping restore");
            storeLocalScaleDirty = false;
        }

        public void SnapLocalRotation()
        {
            Vector3 localEulerAngles = transform.localEulerAngles;
            localEulerAngles.x = Snap(transform.localEulerAngles.x, snapRotationAngles.x);
            localEulerAngles.y = Snap(transform.localEulerAngles.y, snapRotationAngles.y);
            localEulerAngles.z = Snap(transform.localEulerAngles.z, snapRotationAngles.z);
            transform.localEulerAngles = localEulerAngles;
        }

        public void SnapRotation(Quaternion camRotation)
        {
            // var r = camRotation * camRotationOffset;
            // WIP
            Vector3 axis = new Vector3(1, 0, 0);
            Vector3 rotatedAxis = camRotation * axis;

            Quaternion inverseRotation = Quaternion.Inverse(Quaternion.AngleAxis(camRotationOffset.x, rotatedAxis));
            Quaternion finalRotation = inverseRotation * camRotation;

            camRotation = finalRotation;

            Quaternion rotation = rotateToCamForward ? Quaternion.Inverse(camRotation) * transform.rotation : transform.rotation;
            Vector3 eulerAngles = rotation.eulerAngles;

            eulerAngles.x = Snap(eulerAngles.x, snapRotationAngles.x);
            eulerAngles.y = Snap(eulerAngles.y, snapRotationAngles.y);
            eulerAngles.z = Snap(eulerAngles.z, snapRotationAngles.z);

            transform.rotation = rotateToCamForward ? camRotation * Quaternion.Euler(eulerAngles) : Quaternion.Euler(eulerAngles);
        }

        public void SnapLocalScale(float snapValue)
        {
            transform.localScale = Snap(transform.localScale, snapValue);
        }

        public static Vector3 Snap(Vector3 vector, float divisor)
        {
            vector.x = Snap(vector.x, divisor);
            vector.y = Snap(vector.y, divisor);
            vector.z = Snap(vector.z, divisor);
            return vector;
        }

        public static float Snap(float number, float divisor)
        {
            if (divisor == 0) return number;
            return Mathf.Round(number / divisor) * divisor;
        }

        public static Vector2 ScreenSpaceDifference(Vector3 position1, Vector3 position2, Quaternion cameraRotation)
        {
            return cameraRotation * (position2 - position1);
        }

        public Vector3 SnapPosition(Quaternion camRotation, float snapSize, Vector3 initialPosition)
        {
            // if (rotateToCamForward) initialPosition = default;
            // TODO: mark all snap functions as static, add Transform target
            Transform target = transform;

            // by the time this code executes, parents have already snapped and affected this position so reset it
            if (!snapWithParent) target.position = storedPosition;
            if (rectTransform && canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                if (smoothSnapUI) target.position = storedPosition;
                else
                {
                    var parentSnappable = transform.GetComponentInParent<UPixelatorSnappable>();
                    if (!parentSnappable)
                    {
                        var uPixelator = FindObjectOfType<UPixelator>();
                        snapSize *= uPixelator.pixelMultiplier;
                    }
                }
            }

            Vector3 unrotatedPosition = Quaternion.Inverse(camRotation) * (target.position - initialPosition);
            Vector3 snappedPosition = Snap(unrotatedPosition, snapSize);
            // NOTE: don't snap z, makes the shadows flicker less
            snappedPosition.z = unrotatedPosition.z;

            Vector3 snapDiff = unrotatedPosition - snappedPosition;
            Vector3 rotatedVector = camRotation * snappedPosition + initialPosition;

            // var screenSpaceDiff = ScreenSpaceDifference(lastSnappedPosition, rotatedVector, camRotation);
            // stabilizeDiagonal = UPixelator.a;
            // if (isCamera || !stabilizeDiagonal)
            //     target.position = rotatedVector;
            // else
            // {
            //     // if diagonal movement
            //     if (screenSpaceDiff.x != 0 && screenSpaceDiff.y != 0)
            //     {
            //         target.position = rotatedVector;
            //     }
            //     // if (screenSpaceDiff.magnitude >= snapSize * 1.4)
            //     else
            //     {
            //         // if linear movement not over threshold, store it
            //         if (screenSpaceDiff.magnitude <= snapSize)
            //         {
            //             skippedSnappedPosition = rotatedVector;
            //             target.position = lastSnappedPosition;
            //         }
            //         else
            //         {
            //             target.position = skippedSnappedPosition;
            //             skippedSnappedPosition = rotatedVector;
            //         }
            //     }
            // }

            target.position = rotatedVector;
            lastSnappedPosition = target.position;

            if (verboseLog) Log("Snapped position");
            return snapDiff;
        }
    }
}
