using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Item_Info
{
    public string ItemCode = "";
    public string ItemName = "";
    public Vector2Int ItemSize = Vector2Int.one;
    public int maxEa = 1;

    public int Size
    {
        get { return ItemSize.x * ItemSize.y; }
    }
}
[CreateAssetMenu(fileName = "Item_Info_Obj", menuName = "Scriptable Object/Item_Info_Obj", order = int.MaxValue)]
public class Item_Info_Obj : ScriptableObject
{
    public Item_Info Data;
    public static string Item_Info_Obj_Path = "Assets/Resources/Item_info_Data";
    public static Item_Info_Obj ItemObjectCheck(string path)
    {
        Item_Info_Obj value = null;

        string DirectPath = "Assets/Resources/Item_info_Data";
        if (!Directory.Exists(DirectPath)) Directory.CreateDirectory(Item_Info_Obj_Path);
        if (File.Exists(DirectPath + "/" + path + ".asset")) value = Resources.Load<Item_Info_Obj>("Item_info_Data/" + path);

        return value;
    }
}
