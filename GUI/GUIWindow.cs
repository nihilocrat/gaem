using UnityEngine;
using System.Collections;

public class GUIWindow : MonoBehaviour 
{
	public Transform windowChild;
	public bool dontDestroyOnClose = false;
	public bool dontAnimateOnStart = false;
	public AnimationClip openAnimation;
	public AnimationClip closeAnimation;

	void Awake()
	{
		// if you were too lazy to specify the window, assume it's the first window
		if(windowChild == null)
		{
            if (transform.childCount > 0)
            {
                windowChild = transform.GetChild(0);
            }
		}
	}
	
	public void Start()
	{
		if(!dontAnimateOnStart)
		{
			Open();
		}
	}

	public void Open()
	{
        /*
		var iEvent = iTweenEvent.GetEvent(windowChild.gameObject, "Open");
		if(iEvent != null)
		{
			iEvent.Play();
		}
		*/

        GetComponent<Canvas>().enabled = true;

        if (openAnimation != null)
		{
            windowChild.gameObject.SetActive(true);
			GetComponent<Animation>().Play(openAnimation.name);
		}
		else
        {
            // making it less data-driven for the sake of prefab simplicity
            // TODO: make it driven by unity animations instead
            iTween.Stop(windowChild.gameObject);

            // prevent loss of scale if Open()ing a window while it's already being Open()ed
            // scaling from 1 makes it less likely that a stopped itween will leave the window at zero
            windowChild.localScale = Vector3.one;

            
			iTween.ScaleFrom(windowChild.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.4f, "ignoretimescale", true));
		}
	}
	
	public void Close()
	{
		/*
		var iEvent = iTweenEvent.GetEvent(windowChild.gameObject, "Close");
		if(iEvent != null)
		{
			iEvent.Play();
			Destroy(gameObject, (float)iEvent.Values["time"]);
		}
		else
		{
			Destroy(gameObject);
		}
		*/

		if(closeAnimation)
		{
			GetComponent<Animation>().Play(closeAnimation.name);
		}
		else
		{
			// making it less data-driven for the sake of prefab simplicity
			// TODO: make it driven by unity animations instead
			//iTween.Stop(windowChild.gameObject);
			iTween.ScaleTo(windowChild.gameObject, Vector3.zero, 0.5f);
		}

        if (!dontDestroyOnClose)
        {
            Destroy(gameObject, 0.5f);
        }
        else
        {
            Invoke("Disable", closeAnimation.length);
        }
	}

    private void Disable()
    {
        GetComponent<Canvas>().enabled = false;
    }

	public static Vector3 ClampToCanvas(RectTransform canvas, RectTransform window, Vector3 screenPosition)
	{
		var vec2ScreenPosition = new Vector2(screenPosition.x, screenPosition.y);
		var vec2 = ClampToCanvas(canvas, window, vec2ScreenPosition);

		return new Vector3(vec2.x, vec2.y, 0f);
	}

	// CAVEAT: this function will not work on RectTransforms with elastic anchors (min != max)
	public static Vector2 ClampToCanvas(RectTransform canvas, RectTransform window, Vector2 screenPosition)
	{
		Vector2 worldPosition = screenPosition;
		Vector2 min = Vector2.zero;
		Vector2 max = canvas.sizeDelta;
		Vector2 size = window.sizeDelta;

		// scale everything by the canvas scale
		max.x *= canvas.localScale.x;
		max.y *= canvas.localScale.y;
		size.x *= canvas.localScale.x;
		size.y *= canvas.localScale.y;

		min.x += size.x * window.anchorMin.x;
		min.y += size.y * window.anchorMin.y;
		
		max.x -= size.x * (1f - window.anchorMax.x);
		max.y -= size.y * (1f - window.anchorMax.y);
		
		worldPosition.x = Mathf.Clamp(worldPosition.x, min.x, max.x);
		worldPosition.y = Mathf.Clamp(worldPosition.y, min.y, max.y);

		Debug.DrawLine(new Vector3(min.x, max.y, 0f), new Vector3(max.x, max.y, 0f), Color.red);
		Debug.DrawLine(new Vector3(min.x, min.y, 0f), new Vector3(max.x, min.y, 0f), Color.green);
		
		return worldPosition;
	}
}
