using UnityEngine;
using System.Collections;

public class GUIWorldToScreen : MonoBehaviour
{	
	public Camera cam;
	public bool clampToEdge = false;
	
	private Vector3 worldPosition;
	private Vector3 offset;
	private Vector3[] childOffsets;
	
	void Start () {
		worldPosition = transform.position;
		offset = transform.localPosition;

		if(cam == null)
		{
			cam = Camera.main;
		}

		if(transform.childCount > 0)
		{
			childOffsets = new Vector3[transform.childCount];
			for(int i=0; i<transform.childCount; i++)
			{
				childOffsets[i] = transform.GetChild(i).localPosition;
			}
		}
	}
	
	public void AttachTo(Transform newParent, Vector3 newOffset)
	{
		transform.parent = newParent;
		offset = newOffset;
	}
	
	void LateUpdate ()
	{	
		// TODO : only call this in cases where the object would be visible, to improve performance
		if(transform.parent != null)
		{
			Vector3 newPosition;
			Vector3 camVec = transform.parent.position - cam.transform.position;
			//newPosition = cam.WorldToScreenPoint(transform.parent.position + offset);
			//newPosition.x /= Screen.width;
			//newPosition.y /= Screen.height;
			newPosition = cam.WorldToViewportPoint(transform.parent.position + offset);
			
			if(clampToEdge)
			{
				if(Vector3.Dot(cam.transform.forward, camVec) > 0)
				{
					if(newPosition.x > newPosition.y)
					{
						newPosition.x = 1.0f;
					}
					else
					{
						newPosition.y = 1.0f;
					}
				}
				
				newPosition.x = Mathf.Clamp(newPosition.x, 0.0f, 1.0f);
				newPosition.y = Mathf.Clamp(newPosition.y, 0.0f, 1.0f);
			}
			//else if(Vector3.Dot(cam.transform.forward, camVec) > 0)
			else if(newPosition.z <= 0.0f)
			{
				newPosition.x = 9999.9f;
				newPosition.y = 9999.9f;
			}
			
			transform.position = newPosition;
			if(transform.childCount > 0)
			{
				for(int i=0; i<childOffsets.Length; i++)
				{
					transform.GetChild(i).position = newPosition + childOffsets[i];
				}
			}
		}
	}
}
