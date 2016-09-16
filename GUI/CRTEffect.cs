using UnityEngine;
using System.Collections;

public class CRTEffect : MonoBehaviour
{
	public float scaling = 1f;

	void Awake ()
	{
		float aspect = (float)Screen.width / (float)Screen.height;
		transform.localScale = new Vector3(aspect, 1f, 1f) * 2f;

		var tex = GetComponent<Renderer>().material.mainTexture;
		var scale = new Vector2(Screen.width * (1f / tex.width), Screen.height * (1f / tex.height));
		GetComponent<Renderer>().material.mainTextureScale = scale * scaling;
	}
}
