using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIFillBar : MonoBehaviour
{
	public Image barImage;


	private float _value = 1.0f;
	//[Range(0f, 1f)]
	public float value
	{
		get
		{
			return _value;
		}
		set
		{	
			_value = Mathf.Clamp(value, 0f, 1f);
			
			if(barImage.type == Image.Type.Filled)
			{
				barImage.fillAmount = _value;
			}
			else
			{
				barRect.sizeDelta = new Vector2(rect.rect.width * _value, 0f);//rect.rect.height);
			}
		}
	}

	private RectTransform rect;
	private RectTransform barRect;

	void Awake()
	{
		rect = GetComponent<RectTransform>();
		barRect = barImage.gameObject.GetComponent<RectTransform>();

		//parentRect = rect.parent.GetComponent<RectTransform>();
	}
}
