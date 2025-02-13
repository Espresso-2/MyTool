using UnityEngine;
using UnityEngine.EventSystems;

public class Input_SlideOnImage : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 m_delta;
    [Range(0.15f, 1f)]
    public float m_scale;

    //public bool b_isStop;

    //public Vector2 m_scrollV2;

    //public bool isDraging;

    public void OnDrag(PointerEventData eventData)
    {
        m_delta = eventData.delta * m_scale;
        //b_isStop = eventData.IsScrolling();
        //m_scrollV2 = eventData.scrollDelta;
        //isDraging = eventData.dragging;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_delta = Vector2.zero;
    }


    private Vector3 m_mousePos;

    private void Update()
    {

        //当按住不松手且不移动的时候,m_delta 不会等于 zero , 即仍会旋转镜头
        if (Input.mousePosition == m_mousePos)
        {
            m_delta = Vector3.zero;
        }
        m_mousePos = Input.mousePosition;
    }
}
