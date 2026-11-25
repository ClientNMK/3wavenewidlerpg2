using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum ResourceType
{
    Atlas,
}

public enum AtlasType
{
    Bg = 1,
    Icon,
    Icon_Item,
    Icon_Skill,
    UI,
    Banner,
    UIBg,
}

public class SpriteResource
{
    public string spriteName;
    public Sprite sprite;
    public SpriteResource() { }
    public SpriteResource(string _spriteName, Sprite _sprite)
    {
        this.spriteName = _spriteName;
        this.sprite = _sprite;
    }
}

public class ResourceManager : Singleton<ResourceManager>
{
    Dictionary<string, GameObject> resources = new();

    Dictionary<AtlasType, SpriteAtlas> dicSpriteAtlas = new();
    Dictionary<AtlasType, List<SpriteResource>> spriteResources = new();

    public SpriteAtlas GetAtlas(AtlasType _atlasType)
    {
        if (dicSpriteAtlas.ContainsKey(_atlasType))
            return dicSpriteAtlas[_atlasType];

        SpriteAtlas spriteAtlas = null;
        if (_atlasType == AtlasType.Bg)
            spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/Bg_Atlas");
        else if (_atlasType == AtlasType.Icon)
            spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/iconAtlas");
        else if (_atlasType == AtlasType.Icon_Item)
            spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/iconAtlas_item");
        else if (_atlasType == AtlasType.Icon_Skill)
            spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/iconAtlas_skill");
        else if (_atlasType == AtlasType.UI)
            spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/uiAtlas");
        else if (_atlasType == AtlasType.Banner)
            spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/bannerAtlas");
        else if (_atlasType == AtlasType.UIBg)
            spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/uiBgAtlas");

        if (spriteAtlas != null)
            dicSpriteAtlas.Add(_atlasType, spriteAtlas);

        return spriteAtlas;
    }

    public Sprite GetSprite(AtlasType _atlasType, string _spriteName)
    {
        if (spriteResources.ContainsKey(_atlasType) == false)
            spriteResources.Add(_atlasType, new List<SpriteResource>());

        Sprite sprite = null;
        SpriteResource resource = spriteResources[_atlasType].Find(x => x.spriteName.Equals(_spriteName));
        if (resource == null)
        {
            SpriteAtlas atlas = GetAtlas(_atlasType);
            if (atlas != null)
                sprite = atlas.GetSprite(_spriteName);

            if (sprite != null)
            {
                resource = new SpriteResource(_spriteName, sprite);
                spriteResources[_atlasType].Add(resource);
            }
                

            return sprite;
        }
        else
            return resource.sprite;
    }

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject obj = null;
        resources.TryGetValue(path, out obj);
        if (obj != null)
            return obj;

        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.Log($"Filed to load prefab : {path}");
            return null;
        }

        obj = Instantiate(prefab, parent);
        resources.Add(path, obj);
        return obj;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }

    //public T GetResource<T>(string _key) where T : Component
    //{
    //    //if (obj == null)
    //    //    return default(T);

    //    //T t = obj.GetComponent<T>();
    //    //if (t == null)
    //    //    t = obj.AddComponent<T>();

    //    // return t;
    //}

    IEnumerator Load()
    {

        yield return null;
    }
}
