using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    const int instantiateLimitCount = 20;
    int currentInstanticate = 0;
    // 원본 프리팹
    [Header("Origin Object")]
    [SerializeField] List<GameObject> OriPoolingObject = new List<GameObject>();

    // 오브젝트 리스트
    [Header("Pooling Object")]
    [SerializeField] Transform InstantiateTR;
    [SerializeField] Dictionary<string, Stack<GameObject>> DicPoolingObjectList = new Dictionary<string, Stack<GameObject>>();
    Dictionary<string, int> instantiateCounts = new();

    const string ProjectilePath = "Projectile";

    [ContextMenu("ResourcesLoadAllAndAddOriginPoolingObject")]
    public void ResourcesLoadAllAndAddOriginPoolingObject()
    {
        //OriPoolingObject.Clear();
        //GameObject[] arrObjs = Resources.LoadAll<GameObject>("Prefabs");
        //for (int i = 0; i < arrObjs.Length; i++)
        //{
        //    OriPoolingObject.Add(arrObjs[i]);
        //}
        OriPoolingObject.Clear();
    }

    IEnumerator Start()
    {
        //ResourcesLoadAllAndAddOriginPoolingObject();

        //float fTime = 0f;
        //while (fTime < 3f)
        //{
        //    fTime += Time.deltaTime * Time.timeScale;
        //    yield return null;
        //}

        //if (DicPoolingObjectList == null)
        //    DicPoolingObjectList = new Dictionary<string, Stack<GameObject>>();
        //DicPoolingObjectList.Clear();

        //GameObject newObj = null;
        //string key = string.Empty;
        //int idx = 0;
        //for (int i = 0; i < InstantiateTR.childCount; i++)
        //{
        //    newObj = InstantiateTR.GetChild(i).gameObject;
        //    if (newObj == null) continue;
        //    if (newObj.gameObject.activeSelf)
        //        newObj.gameObject.SetActive(false);

        //    key = newObj.name;
        //    if (DicPoolingObjectList.ContainsKey(key) == false)
        //        DicPoolingObjectList.Add(key, new Stack<GameObject>());
        //    DicPoolingObjectList[key].Push(newObj);
        //    yield return null;
        //}

        //for (int i = 0; i < OriPoolingObject.Count; i++)
        //{
        //    newObj = null;
        //    key = OriPoolingObject[i].gameObject.name;
        //    if (key.Equals("HitEffect") || key.Equals("AttackTrailEffect") || key.Equals("UIDamageText"))
        //    {
        //        int createCount = 20;
        //        idx = 0;
        //        for (int k = 0; k < createCount; k++)
        //        {
        //            newObj = Instantiate(OriPoolingObject[i]);
        //            newObj.name = key;
        //            newObj.gameObject.SetActive(false);
        //            newObj.transform.SetParent(InstantiateTR);
        //            if (DicPoolingObjectList.ContainsKey(key) == false)
        //                DicPoolingObjectList.Add(key, new Stack<GameObject>());
        //            DicPoolingObjectList[key].Push(newObj);

        //            if (idx++ % 5 == 0)
        //            {
        //                yield return null;
        //            }
        //        }
        //        yield return null;
        //    }
        yield return null;
        //}
    }

    public GameObject InstantiateObj<T>(string path, string key)
    {
        GameObject originObj = OriPoolingObject.Find(x => x.gameObject.name.Equals(key));
        if (originObj == null)
        {
            string append = GameUtils.GameUtils.Append("Prefabs/", path, "/", key);
            originObj = Resources.Load<GameObject>(append);
            if (originObj != null)
                OriPoolingObject.Add(originObj);
        }

        if (originObj == null)
            return null;

        GameObject newObj = Instantiate(originObj);
        if (newObj != null)
        {
            if (!DicPoolingObjectList.ContainsKey(key))
                DicPoolingObjectList.Add(key, new Stack<GameObject>());

            if (DicPoolingObjectList[key] == null)
                DicPoolingObjectList[key] = new Stack<GameObject>();

            newObj.gameObject.SetActive(false);
            return newObj;
        }

        return null;
    }

    //public GameObject GetObject(string key)
    //{
    //    GameObject obj = null;
    //    if (!DicPoolingObjectList.ContainsKey(key))
    //        DicPoolingObjectList.Add(key, new Stack<GameObject>());

    //    if (DicPoolingObjectList[key] == null)
    //        DicPoolingObjectList[key] = new Stack<GameObject>();

    //    // Debug.Log(key);
    //    if (DicPoolingObjectList.ContainsKey(key) && DicPoolingObjectList[key].Count > 0)
    //        obj = DicPoolingObjectList[key].Pop();

    //    if (obj == null)
    //    {
    //        if (!instantiateCounts.ContainsKey(key))
    //            instantiateCounts.Add(key, 0);

    //        if (instantiateCounts[key] < instantiateLimitCount)
    //        {
    //            obj = InstantiateObj<GameObject>(key);
    //            instantiateCounts[key] += 1;
    //        }
    //    }

    //    if (obj != null)
    //    {
    //        obj.SetActive(true);
    //        //obj.transform.SetParent(null);
    //    }
    //    return obj;
    //}

    public T GetObject<T>(string path, string key = null) where T : Component
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        GameObject obj = null;
        if (!DicPoolingObjectList.ContainsKey(key))
            DicPoolingObjectList.Add(key, new Stack<GameObject>());

        if (DicPoolingObjectList[key] == null)
            DicPoolingObjectList[key] = new Stack<GameObject>();

        // Debug.Log(key);
        if (DicPoolingObjectList.ContainsKey(key) && DicPoolingObjectList[key].Count > 0)
            obj = DicPoolingObjectList[key].Pop();

        if (obj == null)
        {
            obj = InstantiateObj<GameObject>(path, key);
        }

        if (obj != null)
        {
            obj.SetActive(true);
            //obj.transform.SetParent(null);
            return obj.GetComponent<T>();
        }

        return null;
    }
    
    public void Release()
    {
        foreach (var item in DicPoolingObjectList)
        {
            GameObject[] arrObjects = item.Value.ToArray();
            for (int i = 0; i < arrObjects.Length; i++)
            {
                Destroy(arrObjects[i]);
            }
            item.Value.Clear();
        }
        DicPoolingObjectList.Clear();
    }


    public void ReturnObject(string key, GameObject gameObject)
    {
        if (gameObject == null)
            return;

        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        if (DicPoolingObjectList.ContainsKey(key))
            DicPoolingObjectList[key].Push(gameObject);
        //gameObject.transform.SetParent(InstantiateTR);
    }
}
