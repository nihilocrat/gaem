using UnityEngine;
using System.Collections;

public class WorldToScreen : MonoBehaviour
{
	public Camera activeCamera;
	public Vector3 worldPosition;
	public Vector3 worldOffset;
	public Transform worldParent;
	public bool destroyIfParentDestroyed = false;

	// TODO : make this toggleable?
	private const bool screenSpaceCamera = true;
	
	public void AttachTo(Transform target, Vector3 offset)
	{
		worldParent = target;
		worldOffset = offset;
	}

	void Start()
	{
		// update immediately on spawn so I'm in the correct position
		Update();
	}

    public static Vector3 WorldToScreenPoint(Camera activeCamera, Vector3 worldPosition)
    {
        var screenPosition = activeCamera.WorldToScreenPoint(worldPosition);

        // don't render this if it's off-screen
        if (screenPosition.z <= 0.0f)
        {
            screenPosition.x = 9999.9f;
            screenPosition.y = 9999.9f;
        }

        // NOTE : no need to do anything with splitscreen

        if (screenSpaceCamera)
        {
            // splitscreen support
            screenPosition.x -= activeCamera.pixelRect.x;
            screenPosition.y -= activeCamera.pixelRect.y;

            screenPosition.x -= activeCamera.pixelWidth / 2;
            screenPosition.y -= activeCamera.pixelHeight / 2;

            screenPosition.z = 0f;
        }
        
        return screenPosition;
    }

    void Update()
	{
		if(worldParent != null)
		{
			worldPosition = worldParent.position;
		}
		else if(destroyIfParentDestroyed)
		{
			Destroy(gameObject);
			return;
		}

		if(activeCamera == null)
		{
			activeCamera = Camera.main;
		}

        var screenPosition = WorldToScreenPoint(activeCamera, worldPosition + worldOffset);
        
		if(screenSpaceCamera)
		{
			transform.localPosition = screenPosition;
		}
		else
		{
			transform.position = screenPosition;
		}

		// something keeps fucking with the scale
		//transform.localScale = Vector3.one;
		//transform.localRotation = Quaternion.identity;
	}
}
