using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplitscreenCanvasScaler : MonoBehaviour
{
    public CanvasScaler scaler;
    public Splitscreen splitscreen;

    private bool scaled = false;
    private Vector2 originalResolution;

    void Start()
    {
        originalResolution = scaler.referenceResolution;
    }

	void Update()
    {
        if (splitscreen.enabled && !scaled)
        {
            scaler.referenceResolution = originalResolution * 2f;
            scaled = true;
        }
        else if(!splitscreen.enabled && scaled)
        {
            scaler.referenceResolution = originalResolution;
            enabled = false;
        }
	}
}
