using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollView : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public GameObject itemPrefab;

    public RectOffset padding = new();
    public float spacing = 0;

    public bool vertical = true;
    public bool reverseArrangemnent = false;

    List<GameObject> items = new List<GameObject>();
    Vector2 itemSize;

    int visibleItemCount; // 화면에 표시되는 아이템 개수
    public int VisibleItemCount { get { return visibleItemCount; } }
    int totalItemCount = 100; // 총 아이템 개수
    int topIndex = 0; // 현재 최상단 아이템 인덱스
    Vector2 lastScrollPos;
    Vector2 limitScrollPos;

    public Action<GameObject, int> OnUpdateItem = null;
    bool bInitialized = false;

    private void Start()
    {
        // Setup(100);
    }

    public void Init()
    {
        if (bInitialized)
            return;

        if (itemPrefab == null)
            return;

        RectTransform itemRect = itemPrefab.GetComponent<RectTransform>();
        itemSize.x = itemRect.rect.width;
        itemSize.y = itemRect.rect.height;

        items.Clear();
        for (int i = 0; i < content.childCount; i++)
        {
            GameObject item = content.GetChild(i).gameObject;
            items.Add(item);
        }

        // vertical = scrollRect.vertical;
        visibleItemCount = vertical ?
            Mathf.CeilToInt((scrollRect.viewport.rect.height - (padding.top + padding.bottom) + spacing) / (itemSize.y + spacing)) + 1 :
            Mathf.CeilToInt((scrollRect.viewport.rect.width - (padding.left + padding.right) + spacing) / (itemSize.x + spacing)) + 1;
        InitializeItems();
        scrollRect.onValueChanged.AddListener(OnScroll);

        bInitialized = true;
    }
    public void Setup(int _itemCount)
    {
        if (!bInitialized)
            Init();

        totalItemCount = _itemCount;
        topIndex = 0;
        SetContentSizeDelta();

        lastScrollPos = Vector2.zero;
        limitScrollPos = vertical ?
           new Vector2(lastScrollPos.x, scrollRect.content.rect.height - scrollRect.GetComponent<RectTransform>().rect.height) :
           new Vector2(scrollRect.content.rect.width - scrollRect.GetComponent<RectTransform>().rect.width, lastScrollPos.y);
    }

    public void InitializeItems()
    {
        for (int i = 0; i < visibleItemCount; i++)
        {
            GameObject item = null;
            if (i >= items.Count)
            {
                item = Instantiate(itemPrefab, content);
                items.Add(item);
            }
            else
                item = items[i];
            UpdateItem(item, i, false);
        }
    }
    public void SetContentSizeDelta()
    {
        content.sizeDelta = vertical ?
            new Vector2(content.sizeDelta.x, totalItemCount * itemSize.y) :
            new Vector2(totalItemCount * itemSize.x, content.sizeDelta.y);
    }
    public void MoveContent(int _topIndex = 0)
    {
        if (_topIndex < 0)
            return;

        topIndex = _topIndex;
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            int index = topIndex + i;
            UpdateItem(item, index);
        }

        Vector2 anchoredPosition = Vector2.zero;
        if (vertical) anchoredPosition.y = (!reverseArrangemnent) ? topIndex * (itemSize.y + spacing) : -topIndex * (itemSize.y + spacing);
        content.anchoredPosition = anchoredPosition;
        lastScrollPos = anchoredPosition;
    }
    void UpdateItem(GameObject item, int index, bool invokeEvent = true)
    {
        RectTransform itemRect = item.GetComponent<RectTransform>();
        if (!reverseArrangemnent)
        {
            itemRect.anchoredPosition = vertical ?
              new Vector2(itemRect.anchoredPosition.x, -(padding.top + (index * itemSize.y) + (index * spacing))) :
              new Vector2(padding.left + (index * itemSize.x) + (index * spacing), itemRect.anchoredPosition.y);
        }
        else
        {
            itemRect.anchoredPosition = vertical ?
              new Vector2(itemRect.anchoredPosition.x, padding.bottom + (index * itemSize.y) + (index * spacing)) :
              new Vector2(-(padding.right + (index * itemSize.x) + (index * spacing)), itemRect.anchoredPosition.y);
        }

        if (invokeEvent)
            OnUpdateItem?.Invoke(item, index);
    }


    #region EventHandler
    void OnScroll(Vector2 pos)
    {
        if (scrollRect.velocity.x == 0 && scrollRect.velocity.y == 0)
            return;

        if (vertical)
        {
            Vector2 currentScrollPos = scrollRect.content.anchoredPosition;
            float deltaY = currentScrollPos.y - lastScrollPos.y;
            int direction = deltaY > 0 ? 1 : -1;
            if (direction > 0) OnScrollUp();
            else if (direction < 0) OnScrollDown();
        }
    }
    void OnScrollUp()
    {
        Vector2 currentScrollPos = scrollRect.content.anchoredPosition;
        float deltaY = !reverseArrangemnent ? currentScrollPos.y - lastScrollPos.y : lastScrollPos.y - currentScrollPos.y;
        if (!reverseArrangemnent)
        {
            if (currentScrollPos.y + deltaY >= limitScrollPos.y)
                return;

            float addValue = topIndex == 0 ? padding.top : spacing;
            if (Mathf.Abs(deltaY) >= itemSize.y + addValue)
            {
                int moveCount = (int)(deltaY / (itemSize.y + addValue));
                for (int i = 0; i < moveCount; i++)
                {
                    int newIndex = topIndex + i + visibleItemCount;
                    if (newIndex >= 0 && newIndex < totalItemCount)
                    {
                        GameObject item = items[i];
                        items.RemoveAt(i);
                        items.Insert(items.Count, item);
                        UpdateItem(item, newIndex);
                    }
                }
                topIndex += moveCount;
                lastScrollPos += Vector2.up * ((itemSize.y + addValue) * moveCount);
                // Debug.Log("OnScrollUp : " + lastScrollPos);
            }
        }
        else
        {
            if (currentScrollPos.y + deltaY >= 0)
                return;

            float addValue = topIndex == 0 ? padding.bottom : spacing;
            if (Mathf.Abs(deltaY) >= 0)
            {
                int moveCount = (int)(-deltaY / (itemSize.y + addValue)) + 1;
                for (int i = 0; i < moveCount; i++)
                {
                    int newIndex = topIndex - i - 1;
                    if (newIndex >= 0 && newIndex < totalItemCount)
                    {
                        int itemIdx = items.Count - 1;
                        GameObject item = items[itemIdx];
                        items.RemoveAt(itemIdx);
                        items.Insert(0, item);
                        UpdateItem(item, newIndex);
                    }
                }
                topIndex -= moveCount;
                addValue = topIndex == 0 ? padding.bottom : spacing;
                lastScrollPos += Vector2.up * ((itemSize.y + addValue) * moveCount);
                // Debug.Log("OnScrollUp : " + lastScrollPos);
            }
        }
    }
    void OnScrollDown()
    {
        Vector2 currentScrollPos = scrollRect.content.anchoredPosition;
        float deltaY = !reverseArrangemnent ? currentScrollPos.y - lastScrollPos.y : lastScrollPos.y - currentScrollPos.y;
        if (!reverseArrangemnent)
        {
            if (currentScrollPos.y + deltaY <= 0)
                return;

            float addValue = topIndex == 0 ? padding.bottom : spacing;
            if (Mathf.Abs(deltaY) >= 0)
            {
                int moveCount = (int)(-deltaY / (itemSize.y + addValue)) + 1;
                for (int i = 0; i < moveCount; i++)
                {
                    int newIndex = topIndex - i - 1;
                    if (newIndex >= 0 && newIndex < totalItemCount)
                    {
                        int itemIdx = items.Count - 1;
                        GameObject item = items[itemIdx];
                        items.RemoveAt(itemIdx);
                        items.Insert(0, item);
                        UpdateItem(item, newIndex);
                    }
                }
                topIndex -= moveCount;
                addValue = topIndex == 0 ? padding.bottom : spacing;
                lastScrollPos -= Vector2.up * ((itemSize.y + addValue) * moveCount);
                // Debug.Log("OnScrollDown : " + lastScrollPos);
            }
        }
        else
        {
            if (currentScrollPos.y - deltaY <= -limitScrollPos.y)
                deltaY = currentScrollPos.y + limitScrollPos.y;
            if (deltaY < 0)
                return;

            float addValue = topIndex == 0 ? padding.top : spacing;
            if (Mathf.Abs(deltaY) >= itemSize.y + addValue)
            {
                int moveCount = (int)(deltaY / (itemSize.y + addValue));
                for (int i = 0; i < moveCount; i++)
                {
                    int newIndex = topIndex + i + visibleItemCount;
                    if (newIndex >= 0 && newIndex < totalItemCount)
                    {
                        GameObject item = items[i];
                        items.RemoveAt(i);
                        items.Insert(items.Count, item);
                        UpdateItem(item, newIndex);
                    }
                }
                topIndex += moveCount;
                lastScrollPos -= Vector2.up * ((itemSize.y + addValue) * moveCount);
                // Debug.Log("OnScrollDown : " + lastScrollPos);
            }
        }
    }
    #endregion

    public List<GameObject> Items { get { return items; } }
    public int TopIndex { get { return topIndex; } }
}