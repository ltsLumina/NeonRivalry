using TransitionsPlus;
using UnityEngine;

namespace TransitionsPlusDemos {

    public class StartTransition : MonoBehaviour {

        public Texture2D picture;

        public TransitionProfile starTransitionProfile;

        public void StartFadeTransition() {
            TransitionAnimator.Start(
                TransitionType.Fade,     // transition type
                duration: 2f,            // transition duration in seconds
                noiseIntensity: 0.2f     // intensity of noise
                );
        }


        public void StartStarCartoonTransition() {
            TransitionAnimator.Start(TransitionType.Shape, shapeTexture: Resources.Load<Texture2D>("Textures/StartSDF"), splits: 1, keepAspectRatio: true, rotationMultiplier: 2f);
        }

        public void StartStarCartoon2Transition() {
            TransitionAnimator.Start(starTransitionProfile);
        }

        public void StartWipeTransition() {
            TransitionAnimator.Start(TransitionType.Wipe, noiseIntensity: 0.1f, rotation: -15f);
        }

        public void StartCrossWipeTransition() {
            TransitionAnimator.Start(TransitionType.CrossWipe, rotationMultiplier: 5f);
        }

        public void StartDoubleWipeTransition() {
            TransitionAnimator.Start(TransitionType.DoubleWipe);
        }

        public void StartMosaicTransition() {
            TransitionAnimator.Start(TransitionType.Mosaic, duration: 3, cellsDivisions: 6, spread: 8, texture: picture);
        }

        public void StartDissolveTransition() {
            TransitionAnimator.Start(TransitionType.Dissolve, cellsDivisions: 128);
        }

        public void StartBurnTransition() {
            TransitionAnimator.Start(TransitionType.Burn, color: Color.red);
        }

        public void StartBurnSquareTransition() {
            TransitionAnimator.Start(TransitionType.BurnSquare, contrast: 500f);
        }

        public void StartCircularWipeTransition() {
            TransitionAnimator.Start(TransitionType.CircularWipe, contrast: 10f, noiseIntensity: 0.2f, toonGradientIntensity: 16);
        }

        public void StartSeaWavesTransition() {
            TransitionAnimator.Start(TransitionType.SeaWaves, rotationMultiplier: 0.5f, splits: 4);
        }

        public void StartSplashTransition() {
            TransitionAnimator.Start(TransitionType.Splash);
        }

        public void StartTilesTransition() {
            TransitionAnimator.Start(TransitionType.Tiles, cellsDivisions: 8, rotationMultiplier: 2f, contrast: 50f, noiseIntensity: 0);
        }

        public void StartSmearTransition() {
            TransitionAnimator.Start(TransitionType.Smear);
        }

        public void StartSlideTransition() {
            TransitionAnimator.Start(TransitionType.Slide);
        }

        public void StartDoubleSlideTransition() {
            TransitionAnimator.Start(TransitionType.DoubleSlide, rotation: 90);
        }

        public void StartCubeRotationTransition() {
            TransitionAnimator.Start(TransitionType.Cube, texture: picture);
        }

    }
}

