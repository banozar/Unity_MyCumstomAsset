using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GridGroup : MonoBehaviour
{
    public Grid[] myGrid;

    public Inventory_Data_Obj Inventory_Data;
    public Grid_Info[] ItemCheck;
    public Item[] itemList;

    public Item item
    {
        get
        {
            foreach (Item e in itemList)
            {
                if (e.myPosition > -1) return e;
            }
            return null;
        }
    }
   
    public Vector2Int InventorySize;

    public void ItemUsing()
    {
        this.item.changeState(Item.STATE.DESTROY);
    }


    private void Start()
    {
        myGrid = transform.GetComponentsInChildren<Grid>();
        if (Inventory_Data.myItem.Length < 1)
        {
            Inventory_Data.myItem = new Grid_Info[myGrid.Length];
            for (int i = 0; i < Inventory_Data.myItem.Length; i++) Inventory_Data.myItem[i] = new Grid_Info();
        }
        ItemCheck = new Grid_Info[Inventory_Data.myItem.Length];
        for (int i = 0; i < ItemCheck.Length; i++) ItemCheck[i] = new Grid_Info();

        itemList = new Item[Inventory_Data.myItem.Length];

        ItemCheckInitialize();
        InventoryInitiailize();
    }
    public void ItemCheckInitialize()
    {
        Inventory_Data_Obj data =  Inventory_Data;
        ItemCheck = new Grid_Info[data.myItem.Length];
        for (int i = 0; i < ItemCheck.Length; i++)
        {
            ItemCheck[i] = new Grid_Info();
            ItemCheck[i].GridPosition = -1;
        }

        for (int i = 0; i < data.myItem.Length; i++)
        {
            if (data.myItem[i].GridPosition > -1 && data.myItem[i].HaveItem != null)
                for (int j = 0; j < data.myItem[i].HaveItem.Info.ItemSize.x; j++)
                    for (int y = 0; y < data.myItem[i].HaveItem.Info.ItemSize.y; y++)
                    {
                        int Index = i + j + (y * InventorySize.x);
                        ItemCheck[Index] = data.myItem[i];
                    }
        }
    }

    public enum ITEMCHECKTYPE
    { 
        NORMAL, OVERLAP, CHANGE
    }
    public int[] tempCheck;
    public Dictionary<ITEMCHECKTYPE,int> ItemPositionCheck(Item item)
    {
        Dictionary<ITEMCHECKTYPE, int> value = new Dictionary<ITEMCHECKTYPE, int>();

        value[ITEMCHECKTYPE.NORMAL] = item.myPosition;
        value[ITEMCHECKTYPE.OVERLAP] = -1;
        value[ITEMCHECKTYPE.CHANGE] = -1;
        tempCheck =  new int[item.myData.Info.Size];

        if (item.myData.Info.ItemSize.x + (item.myData.Info.ItemSize.y - 1) * InventorySize.x > InventorySize.x * InventorySize.y) value[ITEMCHECKTYPE.NORMAL] = -1;
        else
        {
            for (int i = 0; i < item.myData.Info.ItemSize.x; i++)
                for (int j = 0; j < item.myData.Info.ItemSize.y; j++)
                {
                    int tempIndex = item.myPosition + i + (j * InventorySize.x);
                    if (ItemCheck[tempIndex].GridPosition != -1) value[ITEMCHECKTYPE.NORMAL] = -1;

                    tempCheck[i + (j * item.myData.Info.ItemSize.x)] = -1;
                    if (ItemCheck[tempIndex].HaveItem != null)
                    {
                        tempCheck[i + (j * item.myData.Info.ItemSize.x)] = tempIndex;
                    }
                }
        }


        if (value[ITEMCHECKTYPE.NORMAL] == -1)
        {
            for (int i = 0; i < tempCheck.Length; i++)
            {
                if (tempCheck[i] != -1)
                {
                    if (ItemCheck[tempCheck[i]].GridPosition != -1)
                    {
                        if (itemList[ItemCheck[tempCheck[i]].GridPosition].myData.Info.ItemCode == item.myData.Info.ItemCode && itemList[ItemCheck[tempCheck[i]].GridPosition].myData.Info_Obj.Data.maxEa > itemList[ItemCheck[tempCheck[i]].GridPosition].myData.Ea)
                        {
                            if (itemList[ItemCheck[tempCheck[i]].GridPosition].myData.Ea + item.myData.Ea <= item.myData.Info_Obj.Data.maxEa)
                            {
                                value[ITEMCHECKTYPE.OVERLAP] = ItemCheck[tempCheck[i]].GridPosition;
                                value[ITEMCHECKTYPE.CHANGE] = -1;
                                return value;
                            }
                            else
                            {
                                value[ITEMCHECKTYPE.OVERLAP] = ItemCheck[tempCheck[i]].GridPosition;
                                value[ITEMCHECKTYPE.CHANGE] = ItemCheck[tempCheck[i]].GridPosition;
                                return value;
                            }
                        }
                    }

                    if (value[ITEMCHECKTYPE.CHANGE] == -1) value[ITEMCHECKTYPE.CHANGE] = ItemCheck[tempCheck[i]].GridPosition;
                    if (ItemCheck[tempCheck[i]].GridPosition != -1 && value[ITEMCHECKTYPE.CHANGE] != ItemCheck[tempCheck[i]].GridPosition)
                    {
                        value[ITEMCHECKTYPE.CHANGE] = -1;
                        break;
                    }
                }
            }
        }
        

        return value;
    }

    Vector2[] InventoryPosition;
    void InventoryInitiailize()
    {
              InventoryPosition = ItemAreaPosition();


        for (int i = 0; i < Inventory_Data.myItem.Length; i++)
        {
            if (Inventory_Data.myItem[i].GridPosition >= 0 && Inventory_Data.myItem[i].HaveItem != null)
            {
                if (Inventory_Data.myItem[i].HaveItem.Info != null && Inventory_Data.myItem[i].HaveItem.Info_Obj != null)
                {
                    GameObject tempItem = Item.ItemCreate(transform.parent.Find("ItemArea"));
                    tempItem.GetComponent<Item>().myData = Inventory_Data.myItem[i].HaveItem;

                    //Item Data Set
                    tempItem.GetComponent<Item>().myPosition = i;
                    tempItem.GetComponent<Item>().TargetGridGroup = this;


                    //Item Position Set
                    int LastIndex = i + (Inventory_Data.myItem[i].HaveItem.Info_Obj.Data.ItemSize.x + (Inventory_Data.myItem[i].HaveItem.Info_Obj.Data.ItemSize.y - 1) * InventorySize.x) - 1;

                    Vector2 tempPosition = (InventoryPosition[i] + InventoryPosition[LastIndex]) / 2;
                    tempItem.GetComponent<RectTransform>().anchoredPosition = tempPosition;

                    itemList[i] = tempItem.GetComponent<Item>();
                }               
            }
        }
    }

    Vector2[] ItemAreaPosition()
    {
        Vector2[] InventoryPosition;
        InventoryPosition = new Vector2[InventorySize.x * InventorySize.y];
        Vector2 basic_Position = Vector2.zero;
        basic_Position = new Vector2(InventorySize.x * ItemSystem.ItemGridSizeGet / 2, -InventorySize.y * ItemSystem.ItemGridSizeGet / 2);

        for (int j = InventoryPosition.Length - 1; j >= 0; j--)
        {
            int tempNum = j - (InventoryPosition.Length - 1);
            float x_size = 0;
            float y_size = 0;
            if (gameObject.GetComponent<GridLayoutGroup>() != null) 
            {
                x_size = basic_Position.x + tempNum % InventorySize.x * ItemSystem.ItemGridSizeGet - ItemSystem.ItemGridSizeGet / 2;
                y_size = basic_Position.y - tempNum / InventorySize.x * ItemSystem.ItemGridSizeGet + ItemSystem.ItemGridSizeGet / 2;
            }
            InventoryPosition[j] = new Vector2( x_size ,  y_size);
        }
        return InventoryPosition;
    }

    public void ItemIN(Item item)
    {
        this.Inventory_Data.myItem[item.myPosition].GridPosition = item.myPosition;
        this.Inventory_Data.myItem[item.myPosition].HaveItem = item.myData;
        Inventory_Data.SetDirty();
    }

}
