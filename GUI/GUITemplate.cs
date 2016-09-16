using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUITemplate : MonoBehaviour
{
	public Transform[] bindings;
	public GameObject eventReceiver;

	private Dictionary<string, Component> bindingCache = new Dictionary<string, Component>();

	public delegate bool BindingHandlerFunction(Component binding, object data);

	// Add an entry to this list to support a new binding + data type pair
	// you also need to update the "handlers" list with a function that fits the BindingHandlerFunction delegate
	// it's best to put it in GUITemplateHandlers.cs, but there's no restriction 
	private static Dictionary<System.Type, System.Type> bindingTypes = new Dictionary<System.Type, System.Type>()
	{
		{typeof(GUITemplate), typeof(Hashtable)},
		{typeof(GUITemplateList), typeof(List<Hashtable>)},
		{typeof(Text), typeof(string)},
		{typeof(TextMesh), typeof(string)},
		{typeof(GUIFillBar), typeof(float)},
		{typeof(RawImage), typeof(Texture2D)},
		{typeof(Image), typeof(Sprite)},
		{typeof(WorldToScreen), typeof(Transform)},
	};

	private static List<BindingHandlerFunction> handlers = new List<BindingHandlerFunction>()
	{
		{GUITemplateHandlers.HandleTemplate},
		{GUITemplateHandlers.HandleTemplateList},
		{GUITemplateHandlers.HandleText},
		{GUITemplateHandlers.HandleTextMesh},
		{GUITemplateHandlers.HandleGUIFillBar},
		{GUITemplateHandlers.HandleRawImage},
		{GUITemplateHandlers.HandleImage},
		{GUITemplateHandlers.HandleWorldToScreen},
	};

	public static void RegisterBindingType(System.Type targetType, System.Type sourceType, BindingHandlerFunction handlerFunction)
	{
		// TODO : intelligently handle duplicate target types and handler funcs
		bindingTypes[targetType] = sourceType;
		handlers.Add(handlerFunction);
	}

	public Component GetBindingTarget(string key)
	{
		if(bindingCache.ContainsKey(key))
		{
			return bindingCache[key];
		}

		return null;
	}

	public void SetEventReceiver(GameObject receiver)
	{
		eventReceiver = receiver;

		// set the receiver in all of my child templates
		foreach(Component b in bindingCache.Values)
		{
			var template = b as GUITemplate;
			if(template != null)
			{
				template.SetEventReceiver(receiver);
			}

			// TODO : refactor GUITemplateList so that we don't have to do this twice
			var templateList = b as GUITemplateList;
			if(templateList != null)
			{
				templateList.SetEventReceiver(receiver);
			}
		}
	}
	
	void Awake()
	{
		foreach(Transform b in bindings)
		{
			AddBinding(b);
		}
	}
	
	public void AddBinding(Transform b)
	{
		if(b != null)
		{
			var binding = GetBestBinding(b);
			bindingCache[b.name] = binding;
		}
		else
		{
			Debug.LogWarning("Skipping null binding on GUITemplate: " + gameObject.name);
		}
	}

	public void OnUpdateGUI(Hashtable data)
	{
		// CAVEAT : this doesn't complain if there's a binding that hasn't been given any data
		foreach(string key in data.Keys)
		{
			OnUpdateGUI(key, data[key]);
		}
	}

	// convenience function to update a single binding
	public void OnUpdateGUI(string key, object input)
	{
		string propertyName = null;

		// see if this is a property-setter binding
		// if so, treat the text before the "." as the binding object
		if(key.Contains("."))
		{
			string[] keyparts = key.Split('.');
			if(keyparts.Length > 1)
			{
				key = keyparts[0];
				propertyName = keyparts[1];
			}
		}
		
		//int index = bindings.FindIndex(x => x.name == key);
		if(!bindingCache.ContainsKey(key))
		{
			Debug.LogWarning(string.Format("Cached binding miss for GUITemplate '{0}', key '{1}', searching for one now...", gameObject.name, key));

			Transform newBinding = Utils.FindInHierarchy(transform, key);
			
			if(newBinding != null)
			{
				AddBinding(newBinding);
			}
			else
			{
				Debug.LogError(string.Format("Can't find a binding for GUITemplate '{0}', key '{1}'", gameObject.name, key));
				return;
			}
		}

		// TODO: type-check the input to catch client errors early

		if(!string.IsNullOrEmpty(propertyName))
		{
			// function call
			if(propertyName.EndsWith("()"))
			{
				string methodName = propertyName.Substring(0, propertyName.Length-2);
				Utils.CallMethod(bindingCache[key], methodName, input);
			}
			// member assignment
			else
			{
				// example key-value pair: {"MyText.enabled", false}
				UpdateProperty(bindingCache[key], propertyName, input);
			}
		}
		else
		{
			UpdateBinding(bindingCache[key], input);
		}
	}

	public void UpdateProperty(Component b, string propertyName, object input)
	{
		// NOTE : don't catch any exceptions, client code should know when it sends parameters
		Utils.SetPropertyValue(b, propertyName, input);
	}

	public void UpdateBinding(Component b, object input)
	{
		for(int i=0; i < handlers.Count; i++)
		{
			BindingHandlerFunction handler = handlers[i];

			if(handler(b, input))
			{
				// handled!
				return;
			}

			// otherwise, keep trying
		}

		Debug.LogError(string.Format("Couldn't figure out what to do with data: binding: '{0}', got '{1}' with datatype '{2}', expected '{3}'", b.gameObject.name, input.ToString(), input.GetType().ToString(), ""));//bindingTypes[input.GetType()]));
	}

	private Component GetBestBinding(Transform bindingObject)
	{
		foreach(System.Type bindingType in bindingTypes.Keys)
		{
			Component c = bindingObject.GetComponent(bindingType);
			if(c != null)
			{
				return c;
			}
		}

		return null;
	}
}
