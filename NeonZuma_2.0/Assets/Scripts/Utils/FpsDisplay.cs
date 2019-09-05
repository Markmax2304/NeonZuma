using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FpsDisplay : MonoBehaviour
{
    [SerializeField] private float updateFrecuency = 1f;

    private Text text;
    private WaitForSeconds waitForDelay;

    private void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(UpdateCounter());
    }

    private IEnumerator UpdateCounter()
    {
        waitForDelay = new WaitForSeconds(updateFrecuency);

        while (true)
        {
            var lastFrameCount = Time.frameCount;
            var lastTime = Time.realtimeSinceStartup;

            yield return waitForDelay;

            var timeDelta = Time.realtimeSinceStartup - lastTime;
            var frameDelta = Time.frameCount - lastFrameCount;

            text.text = string.Format("{0:0.} FPS", frameDelta / timeDelta);
        }
    }
}
