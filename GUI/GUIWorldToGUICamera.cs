using UnityEngine;
using System.Collections;

public class GUIWorldToGUICamera : MonoBehaviour
{
	public Transform target;
	public Camera cam;
	public bool clampToEdge = false;
	
	private Vector3 worldPosition;
	private Vector3 offset;
	private Vector3[] childOffsets;
	
	void Start ()
	{
		if(target == null)
		{
			target = transform;
		}

		worldPosition = target.position;
		offset = transform.localPosition;
		
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
		target = newParent;
		offset = newOffset;
	}
	
	void LateUpdate ()
	{
		// TODO : only call this in cases where the object would be visible, to improve performance
		if(target != null)
		{
			Vector3 newPosition;
			Vector3 camVec = target.position - cam.transform.position;
			//newPosition = cam.WorldToScreenPoint(target.position);// + offset);
			//newPosition.x /= Screen.width;
			//newPosition.y /= Screen.height;
			newPosition = cam.WorldToViewportPoint(target.position);// + offset);

			var ratio = Screen.width / Screen.height;

			newPosition -= new Vector3(0.5f,0.5f,0f);
			//newPosition.y += 20f;

			//newPosition.x *= ratio;
			//newPosition *= 2f * 2f;
			//newPosition *= 0.01f;//2f * cam.orthographicSize;

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
