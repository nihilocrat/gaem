using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(GUIMenuController))]
public class GUIMenuScroller : MonoBehaviour
{
	public ScrollRect scroll;
	public int maxVisibleItems = 3;
	public int pixelsPerItem = 100;

	private int currentItemOffset = 0;

	public void SetScrollRectSize()
	{
		int siblingCount = scroll.content.transform.childCount;

		scroll.content.sizeDelta = new Vector2(scroll.content.sizeDelta.x, siblingCount * pixelsPerItem);
	}

	public void CenterToItem(RectTransform obj)
	{
		SetScrollRectSize();

		if(obj == null)
		{
			Debug.LogError("Tried to center GUIMenuScroller on a null object");
			return;
		}

		// HACK TODO : this needs a lot of tweaking
		int currentIndex = obj.GetSiblingIndex();
		int siblingCount = scroll.content.transform.childCount;

		float normalizePosition = 1f;

		int visibleIndex = currentIndex - currentItemOffset;

		if(siblingCount > maxVisibleItems && (visibleIndex >= maxVisibleItems || visibleIndex < 0))
		{
			normalizePosition = (float)currentIndex / (float)siblingCount;

			// HACK : multiplier below is a magic number, probably varies based on the size of scroll.content
			//normalizePosition *= 0.01f * siblingCount; //0.25f;
			normalizePosition -= ((float)maxVisibleItems-1) / (float)siblingCount;
			normalizePosition = Mathf.Clamp01(1 - normalizePosition);

			int minOffset = 0;
			int maxOffset = siblingCount - maxVisibleItems;
			currentItemOffset = Mathf.Clamp(currentIndex - (maxVisibleItems+1), minOffset, maxOffset);
		}

		scroll.verticalNormalizedPosition = normalizePosition;
    }

}
