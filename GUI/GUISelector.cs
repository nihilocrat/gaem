using UnityEngine;
using System.Collections;

public class GUISelector : MonoBehaviour
{
    public GUITemplate template;
    public object[] values;
    public string[] valueNames;

    private int currentValueIndex = 0;

    public object Value
    {
        get
        {
            return values[currentValueIndex];
        }
    }

	void Start () {
	
	}
	
    void UpdateGUI()
    {
        string valueName = valueNames[currentValueIndex];

        template.OnUpdateGUI("Value", valueName);
    }
}
