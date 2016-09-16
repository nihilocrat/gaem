using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DelayedScaler : MonoBehaviour
{
	public float matchSpeed = 10.0f;
	public float delayBeforeMatch = 1.0f;
	public Transform parentImage;
	public Transform delayedImage;

	private float lastChange = 0f;
	private Vector2 direction = Vector2.zero;
	private bool matchingScale = false;
	private bool countdown = false;

	public void Init()
	{
		// match the bar I'm following, so that at startup I won't countdown
		Stop();
	}

	public void Stop()
	{
		delayedImage.localScale = parentImage.localScale;
		matchingScale = false;
		countdown = false;
		direction = Vector2.zero;
	}

	void Update()
	{
		if(matchingScale)
		{
            // TODO: to make this super-generic, sign shouldn't rely on X, but figure out if sizedelta is growing or shrinking
			//var diff = parentImage.sizeDelta.sqrMagnitude - delayedImage.sizeDelta.sqrMagnitude;
			//var sign = Mathf.Sign(diff);
            var sign = Mathf.Sign(direction.x);
            
			Vector3 step = direction * Time.deltaTime * matchSpeed;
			
			if((sign > 0f && (delayedImage.localScale + step).x >= parentImage.localScale.x) ||
			   (sign < 0f && (delayedImage.localScale + step).x <= parentImage.localScale.x))
			{
				Stop();
			}
			else
			{
				delayedImage.localScale += step;
			}
		}
		else if(!countdown && parentImage.localScale != delayedImage.localScale)
		{
			lastChange = Time.time;
			countdown = true;
        }
		else if(countdown && Time.time - lastChange > delayBeforeMatch)
		{
			matchingScale = true;

			direction = (parentImage.localScale - delayedImage.localScale).normalized;
		}
	}
}
