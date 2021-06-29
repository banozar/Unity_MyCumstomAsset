using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public class GRIDGROUP
    {
        GameObject _Obj;
        public GameObject Obj
        {
            get
            {
                return _Obj;
            }
            set
            {
                _Obj = value;
                
                if(Obj.GetComponent<GridGroup>()==null) Obj.AddComponent<GridGroup>();
                Obj.GetComponent<GridGroup>().InventorySize = new Vector2Int((int)PARENT.Data.InventorySIze.x, (int)PARENT.Data.InventorySIze.y);
                Obj.GetComponent<GridGroup>().Inventory_Data = PARENT.Data;
                switch (this.PARENT.myType)
                {
                    case INVENTORYTYPE.INVENTORY:
                        Obj.AddComponent<GridLayoutGroup>();
                        GridLayoutSet(Obj.GetComponent<GridLayoutGroup>());
                        InventoryGirdGroupInitialize();
                        break;
                    case INVENTORYTYPE.EQIPMENT:
                        InventoryGirdGroupInitialize();
                       Vector2 tempGridSize = new Vector2 (PARENT.Data.InventorySIze.x * ItemSystem.ItemGridSizeGet, PARENT.Data.InventorySIze.y*ItemSystem.ItemGridSizeGet);
                        for (int i = 0; i < Obj.transform.childCount; i++)
                        {
                            Obj.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = tempGridSize;
                        }
                        
                        break;
                }
                
            }
        }
        Inventory PARENT;

        void GridLayoutSet(GridLayoutGroup target)
        {
            target.cellSize = new Vector2(ItemSystem.ItemGridSizeGet, ItemSystem.ItemGridSizeGet);
            target.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            target.constraintCount = PARENT.Data.InventorySIze.x;
            target.childAlignment = TextAnchor.MiddleCenter;
        }
        void InventoryGirdGroupInitialize()
        {
            for (int i = 0; i < Obj.transform.childCount; i++) Destroy(Obj.transform.GetChild(i).gameObject);
            for (int i = 0; i < PARENT.Data.InventorySIze.y; i++)
                for (int j = 0; j < PARENT.Data.InventorySIze.x; j++)
                {
                    GameObject temp = RectObjMake();
                    temp.name = i + "/" + j;
                    temp.AddComponent<Image>();
                    temp.GetComponent<Image>().sprite = Resources.Load<Sprite>(ItemSystem.GridImagePath);
                    temp.GetComponent<Image>().type = Image.Type.Sliced;
                    temp.transform.SetParent(Obj.transform);
                    
                    temp.AddComponent<Grid>();
                    temp.GetComponent<Grid>().GridNumber = (j % PARENT.Data.InventorySIze.x) + (i * PARENT.Data.InventorySIze.x);
                    temp.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
        }
        public GRIDGROUP(Inventory Pa)
        {
            this.PARENT = Pa;
        }
    }


    public enum INVENTORYTYPE
    { 
        INVENTORY,EQIPMENT
    }
    
    public Inventory_Data_Obj Data;
    public GameObject GridArea;
    public GameObject ItemArea;
    public INVENTORYTYPE myType = INVENTORYTYPE.INVENTORY;
    GRIDGROUP myGridGroup;

   public static GameObject RectObjMake()
    {
        GameObject temp = new GameObject();
        temp.AddComponent<RectTransform>();
        return temp;
    }

    void GridInitailize()
    {
        myGridGroup = new GRIDGROUP(this);

        if (transform.FindChild("GridGroup") == null)
        {
            GameObject temp = RectObjMake();
            temp.name = "GridGroup";
            temp.transform.SetParent(transform);
            temp.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        myGridGroup.Obj = transform.FindChild("GridGroup").gameObject;
    }

    void ItemAreaInitialize()
    {
        if (transform.FindChild("ItemArea") == null)
        {
            GameObject temp = RectObjMake();
            temp.transform.SetParent(transform);
            temp.transform.localPosition = Vector3.zero;
            temp.name = "ItemArea";
        }
        ItemArea = transform.FindChild("ItemArea").gameObject;
    }
    void InventoryInitailize()
    {
        //Grid Set
        if (Data != null)
        {
            GridInitailize();
            ItemAreaInitialize();
        }
    }

    void Start()
    {
        InventoryInitailize();
    }
}
