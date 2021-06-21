using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void MouseEventAction(PointerEventData eventData);

public class MouseEvent : MonoBehaviour,
    IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
    IPointerEnterHandler, IPointerExitHandler,
    IDragHandler, IEndDragHandler, IDropHandler
{
    public event MouseEventAction Click;
    public event MouseEventAction Down;
    public event MouseEventAction Move;
    public event MouseEventAction Up;
    public event MouseEventAction Over;
    public event MouseEventAction Enter;
    public event MouseEventAction Exit;
    public event MouseEventAction Drag;
    public event MouseEventAction EndDrag;
    public event MouseEventAction Drop;
    public void OnPointerClick(PointerEventData eventData)
    {
        Click?.Invoke(eventData);
    }
    public void OnMouseOver(PointerEventData eventData)
    {
        Over?.Invoke(eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Down?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Enter?.Invoke(eventData);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Exit?.Invoke(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Up?.Invoke(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Drag?.Invoke(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag?.Invoke(eventData);
    }
    public void OnDrop(PointerEventData eventData)
    {
        Drop?.Invoke(eventData);
    }
}
