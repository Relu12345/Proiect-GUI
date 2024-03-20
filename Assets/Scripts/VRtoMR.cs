using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class VRtoMR : MonoBehaviour
{
    // Duration over which the opacity will change
    public float fadeDuration = 3.0f;

    // Reference to the Passthrough layer
    private OVRPassthroughLayer passthroughLayer;

    void Start()
    {
        // Get the Passthrough layer component
        passthroughLayer = GetComponent<OVRPassthroughLayer>();
    }

    // Call this method when the button is pressed
    public void StartFadeToMixedReality()
    {
        StartCoroutine(FadeToMixedReality());
    }

    IEnumerator FadeToMixedReality()
    {
        float currentTime = 0f;
        while (currentTime < fadeDuration)
        {
            // Calculate the current opacity
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);

            // Set the current opacity to the Passthrough layer
            passthroughLayer.textureOpacity = alpha;

            // Increment the time
            currentTime += Time.deltaTime;
            yield return null;
        }
        // Ensure the opacity is set to 1 at the end
        passthroughLayer.textureOpacity = 1f;
    }
}