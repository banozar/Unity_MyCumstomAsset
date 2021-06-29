using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory_Data_Obj", menuName = "Scriptable Object/Inventory_Data_Obj", order = int.MaxValue)]
public class Inventory_Data_Obj : ScriptableObject
{
    public Vector2Int InventorySIze = Vector2Int.one;
    public Grid_Info[] myItem;
}
