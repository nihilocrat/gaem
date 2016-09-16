using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GUIControl : MonoBehaviour
{
	public Transform recipient;
	public string message;
	public string argument;
	public bool executeSubmitEvent = false;
	
	public void OnGUISubmit()
	{
		if(executeSubmitEvent)
		{
			ExecuteEvents.Execute( recipient.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler );
		}
		else
		{
			SendGUIMessage(this.message, this.argument);
		}
	}

	public void OnGUICancel()
	{
		// Cancel messages are always forwarded
		SendGUIMessage("OnGUICancel", null);
	}

	void SendGUIMessage(string message, string argument)
	{
        // avoid infinite loops
        if(recipient == transform && (message == "OnGUISubmit" || message == "OnGUICancel"))
        {
            Debug.LogError("GUIControl tried to send an 'OnGUI' message to itself! It would cause an infinite loop, ignoring it!");
            return;
        }

		if(recipient == null)
		{
			recipient = Director.Instance.transform;
		}
		
		if(string.IsNullOrEmpty(argument))
		{
			recipient.gameObject.SendMessage(message);
		}
		else
		{
			recipient.gameObject.SendMessage(message, argument);
		}
	}
}
