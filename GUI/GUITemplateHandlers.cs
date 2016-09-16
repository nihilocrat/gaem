using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class GUITemplateHandlers
{
	public static bool HandleTemplate(Component b, object input)
	{
		var template = b as GUITemplate;
		var data = input as Hashtable;
		if(template != null)// && data != null)
		{
			if(data != null)
			{
				template.gameObject.SetActive(true);
				template.OnUpdateGUI(data);
			}
			else
			{
				template.gameObject.SetActive(false);
			}
			return true;
		}
		
		return false;
	}

	public static bool HandleTemplateList(Component b, object input)
	{
		var templatelist = b as GUITemplateList;
		var dataList = input as List<Hashtable>;
		if(templatelist != null && dataList != null)
		{
			templatelist.OnUpdateGUI(dataList);
			return true;
		}

		return false;
	}

	public static bool HandleText(Component b, object input)
	{
		string text = input as string;
		var textMesh = b as Text;
		if(textMesh != null && text != null)
		{
			textMesh.text = text;
			return true;
		}

		return false;
	}
	
	public static bool HandleGUIFillBar(Component b, object input)
	{
		try
		{
			float floatVal = (float)input;
			var bar = b as GUIFillBar;
			if(bar != null)
			{
				bar.value = floatVal;
				return true;
			}
		}
		catch
		{
			// do nothing
		}

		return false;
	}

	public static bool HandleRawImage(Component b, object input)
	{
		var rawImage = b as RawImage;
		var tex = input as Texture2D;
		if(rawImage != null)
		{
			rawImage.enabled = (tex != null);
			rawImage.texture = tex;
			return true;
		}

		return false;
	}
	
	public static bool HandleImage(Component b, object input)
	{
		var image = b as Image;
		var sprite = input as Sprite;
		if(image != null)
		{
			image.enabled = (sprite != null);
			image.sprite = sprite;
			return true;
		}

		return false;
	}
	
	public static bool HandleWorldToScreen(Component b, object input)
	{
		var world2screen = b as WorldToScreen;
		var parent = input as Transform;
		if(world2screen != null && parent != null)
		{
			//world2screen.gameObject.SetActive((parent != null));
			world2screen.worldParent = parent;
			return true;
		}

		return false;
	}
	
	public static bool HandleTextMesh(Component b, object input)
	{
		var textmesh = b as TextMesh;
		var data = input as string;
		if(textmesh != null && data != null)
		{
			textmesh.text = data;
			return true;
		}
		
		return false;
	}
}
