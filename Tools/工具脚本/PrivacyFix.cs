using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyFix : MonoBehaviour
{
    private RectTransform contentTrans;

    private void Awake()
    {
        contentTrans = GetComponent<RectTransform>();
    }

    private UICamera[] mNGUIEventSystems;

    public UICamera[] NGUIEventSystems
    {
        get
        {
            if (mNGUIEventSystems == null)
            {
                mNGUIEventSystems = GameObject.FindObjectsOfType<UICamera>();
            }
            return mNGUIEventSystems;
        }
    }

    private void OnEnable()
    {
        SetNGUIEventSystemActive(false);
    }

    private void OnDisable()
    {
        SetNGUIEventSystemActive(true);
    }

    private void SetNGUIEventSystemActive(bool isEnabled)
    {
        foreach (var eventSystem in NGUIEventSystems)
        {
            if (eventSystem == null)
                continue;
            eventSystem.enabled = isEnabled;
            Debug.Log($"{eventSystem.name},State:{isEnabled}");
        }
    }

    private Vector2 touchStartPos;
    private Vector2 lastTouchPos;
    private float minSwipeDistance = 0f; // 最小滑动距离  

    void Update()
    {
        // 确保有触摸输入  
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    lastTouchPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    lastTouchPos = touch.position;
                    DetectSwipe(touch.deltaPosition.y);
                    break;
                case TouchPhase.Ended:
                    break;
            }
        }
    }

    void DetectSwipe(float moveValue)
    {
        float distance = moveValue;
        if (Mathf.Abs(distance) >= minSwipeDistance)
        {
            // var rectTrans = contentTrans as RectTransform;
            //Debug.Log(distance > 0 ? "向上滑动" : "向下滑动");
            Vector2 anchoredPosition = contentTrans.anchoredPosition;
            anchoredPosition = Vector2.Lerp(new Vector2(anchoredPosition.x, anchoredPosition.y),
                new Vector2(anchoredPosition.x, anchoredPosition.y + distance), Time.deltaTime);
            contentTrans.anchoredPosition = anchoredPosition;
            // var vector2 = rectTrans.anchoredPosition;
            // vector2 =
            //     new Vector2(vector2.x, vector2.y + distance*2);
            // rectTrans.anchoredPosition = vector2;
            // touchStartPos = lastTouchPos;
        }
    }
}