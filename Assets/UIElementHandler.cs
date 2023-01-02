using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementHandler : MonoBehaviour
{
    private List<UIElementSystem> elements = new List<UIElementSystem>();

    public void MoveElements(Vector3 newPosition){
        foreach (UIElementSystem i in elements){
            i.iconPosition += newPosition;
        }
    }

    public void AddElement(UIElementSystem element){
        elements.Add(element);
    }

    public void RemoveElement(UIElementSystem element){
        elements.Remove(element);
    }
}
