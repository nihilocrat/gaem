using UnityEngine;
using System.Collections;

public class GUITriggeredSender : MonoBehaviour
{
	//public string triggerMessage;
	public Transform recipient;
	public string message;
	public string argument;
	
	public void OnClicked()
	{
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
