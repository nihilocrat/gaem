using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUITemplateList : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject eventReceiver;

    // TODO: does this really need to exist?
    public int Count
    {
        get
        {
            return itemTemplates.Count;
        }
    }

    public List<GUITemplate> ItemTemplates
    {
        get
        {
            return itemTemplates;
        }
    }

    private List<GUITemplate> itemTemplates = new List<GUITemplate>();
    private UnityEngine.UI.Button firstButton;

    public void OnUpdateGUI(List<Hashtable> dataList)
    {
        InitInstances(dataList.Count);

        for (int i = 0; i < itemTemplates.Count; i++)
        {
            GUITemplate template = itemTemplates[i];

            if (i < dataList.Count)
            {
                template.gameObject.SetActive(true);
                Hashtable data = dataList[i];

                template.OnUpdateGUI(data);
            }
            else
            {
                // there's no data, so disable it
                template.gameObject.SetActive(false);
            }
        }
    }

    // call this when entering a new menu with the same TemplateList
    // HACK : this code smells a little bit, does it belong here? Is there another way?
    public void OnMenuChanged()
    {
        if (firstButton != null)
        {
            // this will force the GUI to focus on the main
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject, null);
        }
    }

    public void SelectAt(int index)
    {
        var buttonObj = GetButtonAt(index);
        if (buttonObj)
        {
            EventSystem.current.SetSelectedGameObject(buttonObj, null);
        }
    }

    public GameObject GetButtonAt(int index)
    {
        var button = itemTemplates[index].GetComponentInChildren<UnityEngine.UI.Button>();

        if (button)
        {
            return button.gameObject;
        }

        return null;
    }

    public void InitInstances(int count)
    {
        if (itemTemplates.Count >= count)
        {
            return;
        }
        else
        {
            itemTemplates.Capacity = count;
        }

        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(itemPrefab, transform.position, transform.rotation) as GameObject;
            obj.name = i.ToString();

            // need to set parent and reset scale to prevent weird scaling artifacts
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;

            var template = obj.GetComponent<GUITemplate>();
            if (template != null)
            {
                itemTemplates.Add(template);
            }
            else
            {
                Debug.LogError("No GUITemplate component on itemPrefab for GUITemplateList: " + gameObject.name);
            }

            var button = obj.GetComponent<Button>();

            if (button == null)
            {
                button = obj.GetComponentInChildren<Button>();
            }

            if (button != null && eventReceiver != null)
            {
                // sneaky weirdness required here!
                // see: http://answers.unity3d.com/questions/791573/46-ui-how-to-apply-onclick-handler-for-button-gene.html
                int buttonNumber = i;
                var e = new MenuButtonEvent(gameObject.name, buttonNumber);
                button.onClick.AddListener(() => { eventReceiver.SendMessage("OnButtonClicked", e); });
                if (i == 0)
                {
                    firstButton = button;
                }
            }

            obj.SetActive(true);
        }

        OnMenuChanged();
    }

    public void SetEventReceiver(GameObject receiver)
    {
        eventReceiver = receiver;

        // set the receiver in all of my child templates
        for (int i = 0; i < itemTemplates.Count; i++)
        {
            itemTemplates[i].SetEventReceiver(receiver);
        }
    }
}

public class MenuButtonEvent
{
    public MenuButtonEvent(string menuName, int buttonNumber)
    {
        this.menuName = menuName;
        this.buttonNumber = buttonNumber;
    }

    public override string ToString()
    {
        return string.Format("[MenuButtonEvent] Menu: '{0}', Button: {1}", menuName, buttonNumber);
    }

    public Transform menu;
    public string menuName = null;
    public int buttonNumber = -1;
    //public GUIMenuOption option;
}
