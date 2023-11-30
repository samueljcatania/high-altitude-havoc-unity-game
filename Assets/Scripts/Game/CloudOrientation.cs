using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game
{
    public class CloudOrientation : MonoBehaviour
    {
        public Transform stealthBomber;
        private Volume cloudVolume;
        private VolumetricClouds _volumetricClouds;

        public float currentWindAngle;
        public float targetWindAngle;
        private const float Offset = 270f;
        public float interpolationSpeed = 1f;
        private Coroutine windChangeCoroutine;

        private void Start()
        {
            gameObject.GetComponent<Volume>().profile.TryGet(out _volumetricClouds);
            _volumetricClouds.verticalErosionWindSpeed.value = 0f;
            _volumetricClouds.verticalShapeWindSpeed.value = 0f;

            //var targetWindAngle = (Offset + stealthBomber.eulerAngles.y) % 360f;
        }

        // private void Update()
        // {
        //     var currentOrientation = _volumetricClouds.orientation;
        //     var targetOrientation = new WindOrientationParameter((Offset + stealthBomber.eulerAngles.y) % 360f,
        //         WindParameter.WindOverrideMode.Global, true);
        //
        //     if (windChangeCoroutine != null)
        //         StopCoroutine(windChangeCoroutine);
        //     windChangeCoroutine = StartCoroutine(ChangeWindDirection(currentOrientation, targetOrientation));
        // }
        //
        // IEnumerator ChangeWindDirection(WindOrientationParameter from, WindOrientationParameter to)
        // {
        //     float duration = 1f; // Duration of the transition in seconds
        //     float elapsed = 0f;
        //
        //     while (elapsed < duration)
        //     {
        //         elapsed += Time.deltaTime;
        //         float frac = elapsed / duration;
        //         _volumetricClouds.orientation.Interp(from.value, to.value, frac);
        //         yield return null;
        //     }
        // }
    }
}