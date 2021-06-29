using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class Item_Data
{


    public Item_Info_Obj Info_Obj;
    public Item_Info Info;
    public int Ea;
    public void Initialize()
    {
        if (Info_Obj == null) Info_Obj = Item_Info_Obj.ItemObjectCheck("Templet");
        if( Info==null) Info = Info_Obj.Data;
    }
}

public class Item : MouseEvent
{
    public Item_Data myData = new Item_Data();
    public float GridSize = ItemSystem.ItemGridSizeGet;

    BoxCollider2D col;
    GridGroup[] GridGroups;
 
    [SerializeField]
     GridGroup _TargetGridGroup;
    public GridGroup TargetGridGroup
    {
        get
        {
            return _TargetGridGroup;
        }
        set
        {
                if (TargetGridGroup != value)
                {
                if(TargetGridGroup!=null)   foreach (Grid e in TargetGridGroup.myGrid) e.Active = false;

                    _TargetGridGroup = value;
                if (TargetGridGroup != null)
                    GridCheck = TargetGridGroup.myGrid;
                else
                    GridCheck = null;
                }
        }
    }

    public Grid[] GridCheck;
    public int myPosition = -1;
    public int LastPosition
    {
        get
        {
            if (myPosition > -1 && TargetGridGroup!=null)
            {
                return myPosition+(myData.Info_Obj.Data.ItemSize.x - 1) + (myData.Info_Obj.Data.ItemSize.y - 1) * TargetGridGroup.InventorySize.x;
            }
            return -1;
        }
    }
    void Initialize()
    {
        Vector2 temp_Size = new Vector2(myData.Info_Obj.Data.ItemSize.x * GridSize, myData.Info_Obj.Data.ItemSize.y * GridSize);
        GetComponent<RectTransform>().sizeDelta = temp_Size;

        if (GetComponent<BoxCollider2D>() == null) gameObject.AddComponent<BoxCollider2D>();
        col = GetComponent<BoxCollider2D>();
        col.size = temp_Size;

        transform.name = myData.Info_Obj.Data.ItemName;

        Click += (PointerEventData a) => {
            if (myState == STATE.DEFAULT) changeState(STATE.GET);
            else if (myState == STATE.GET) ItemInput();
        };
    }
    public enum STATE
    { 
        DEFAULT,GET,DROP,DESTROY
    }

    public STATE myState = STATE.DEFAULT;

    public void changeState(STATE a)
    {
        if (a == myState) return;
        myState = a;
        switch (myState)
        {
            case STATE.DEFAULT:
                if (TargetGridGroup != null)
                {
                    if (TargetGridGroup.GetComponent<GridLayoutGroup>() == null) 
                        GetComponent<RectTransform>().sizeDelta = new Vector2(TargetGridGroup.Inventory_Data.InventorySIze.x * ItemSystem.ItemGridSizeGet, TargetGridGroup.Inventory_Data.InventorySIze.y*ItemSystem.ItemGridSizeGet);
                    TargetGridGroup.ItemCheckInitialize();
                }
                break;
            case STATE.GET:
                GetComponent<RectTransform>().sizeDelta = new Vector2(myData.Info_Obj.Data.ItemSize.x*ItemSystem.ItemGridSizeGet, myData.Info_Obj.Data.ItemSize.y * ItemSystem.ItemGridSizeGet);
                GridGroups = FindObjectsOfType<GridGroup>();
                ItemOut();
                transform.SetParent(FindObjectOfType<Canvas>().transform.Find("ItemField"));
                
                TargetGridGroup = null;
                break;
            case STATE.DROP:
                break;
            case STATE.DESTROY:
                ItemOut();
                Destroy(gameObject);
                break;
        }
    }
    void StateProcess()
    {
        switch (myState)
        {
            case STATE.DEFAULT:
                break;
            case STATE.GET:
                transform.position = Input.mousePosition;
                ItemUpCheck();
                break;
            case STATE.DROP:
                break;
        }
    }
    bool RectPositionCheck(RectTransform e,Vector2 leftUp,Vector2 rightDown)
    {
        if (leftUp.x < e.GetComponent<RectTransform>().position.x
              && rightDown.x > e.GetComponent<RectTransform>().position.x
              && leftUp.y > e.GetComponent<RectTransform>().position.y
              && rightDown.y < e.GetComponent<RectTransform>().position.y)
            return true;

        return false;
    }

    void ItemUpCheck()
    {
        //Grid Group Set
        foreach (GridGroup e in GridGroups)
        {
            Vector2 teori = e.GetComponent<RectTransform>().position;
            Vector2 tetemp_ItmeSize = new Vector2(e.InventorySize.x * (GridSize / 2), e.InventorySize.y * (GridSize / 2));

            Vector2 teleftUp = new Vector2(teori.x - tetemp_ItmeSize.x, teori.y + tetemp_ItmeSize.y);
            Vector2 terightDown = new Vector2(teori.x + tetemp_ItmeSize.x, teori.y - tetemp_ItmeSize.y);

            if (RectPositionCheck(GetComponent<RectTransform>(), teleftUp, terightDown))
            {
                TargetGridGroup = e;
                break;
            }
            else TargetGridGroup = null;
        }
        if (TargetGridGroup == null) return;

        Vector2 ori;
        Vector2 temp_ItmeSize;

        Vector2 leftUp;
        Vector2 rightDown;

        if (TargetGridGroup.GetComponent<GridLayoutGroup>() != null)
        {
            ori = GetComponent<RectTransform>().position;
            temp_ItmeSize = new Vector2(myData.Info_Obj.Data.ItemSize.x * (GridSize / 2), myData.Info_Obj.Data.ItemSize.y * (GridSize / 2));

            leftUp = new Vector2(ori.x - temp_ItmeSize.x, ori.y + temp_ItmeSize.y);
            rightDown = new Vector2(ori.x + temp_ItmeSize.x, ori.y - temp_ItmeSize.y);
            foreach (Grid e in TargetGridGroup.myGrid)
            {
                if (RectPositionCheck(e.GetComponent<RectTransform>(), leftUp, rightDown)) e.Active = true;
                else e.Active = false;
            }
        }
        else
        {
            foreach (Grid e in TargetGridGroup.myGrid)
            {
                ori = e.GetComponent<RectTransform>().position;
                temp_ItmeSize = new Vector2(TargetGridGroup.InventorySize.x * GridSize /2, TargetGridGroup.InventorySize.y * GridSize / 2);
                leftUp = new Vector2(ori.x - temp_ItmeSize.x, ori.y + temp_ItmeSize.y);
                rightDown = new Vector2(ori.x + temp_ItmeSize.x, ori.y - temp_ItmeSize.y);
                if (RectPositionCheck(GetComponent<RectTransform>(), leftUp, rightDown)) e.Active = true;
                else e.Active = false;
            }
        }
       
    }
    void ItemOut()
    {
        if (TargetGridGroup != null)
        {
            if (myPosition >= 0)
            {
                TargetGridGroup.itemList[myPosition] = null;

                TargetGridGroup.Inventory_Data.myItem[myPosition] = new Grid_Info();
                TargetGridGroup.Inventory_Data.myItem[myPosition].GridPosition = -1;
                TargetGridGroup.Inventory_Data.myItem[myPosition].HaveItem = null;

                TargetGridGroup.Inventory_Data.SetDirty();
            }

            TargetGridGroup.ItemCheckInitialize();
            myPosition = -1;
        }
    }

    void ItemInput()
    {
        if (TargetGridGroup != null)
        {
            List<Grid> tempGrid = new List<Grid>();
            foreach (Grid e in TargetGridGroup.myGrid)
                if (e.Active) tempGrid.Add(e);

            if (tempGrid.Count >= myData.Info_Obj.Data.Size)
            {
                int number = -1;
                foreach (Grid e in tempGrid)
                {
                    if(number<0)    number = e.GridNumber;
                    if (number > e.GridNumber) number = e.GridNumber;
                }
                if (number < 0) return;

                myPosition = number;
                Dictionary<GridGroup.ITEMCHECKTYPE, int> checkGroup = TargetGridGroup.ItemPositionCheck(this);

                if (checkGroup[GridGroup.ITEMCHECKTYPE.OVERLAP] > -1&& checkGroup[GridGroup.ITEMCHECKTYPE.CHANGE] <0)
                {
                    myPosition = -1;
                    TargetGridGroup.Inventory_Data.myItem[checkGroup[GridGroup.ITEMCHECKTYPE.OVERLAP]].HaveItem.Ea += myData.Ea;
                    TargetGridGroup.Inventory_Data.SetDirty();
                    
                    changeState(STATE.DESTROY);
                    return;
                }
                if (checkGroup[GridGroup.ITEMCHECKTYPE.NORMAL]>-1)
                {
                    TargetGridGroup.ItemIN(this);

                    Vector2 firstPos = TargetGridGroup.myGrid[myPosition].GetComponent<RectTransform>().position;
                    Vector2 lastPos = TargetGridGroup.myGrid[LastPosition].GetComponent<RectTransform>().position;
                    Vector2 pos = (firstPos + lastPos) / 2;


                    transform.SetParent(TargetGridGroup.transform.parent.Find("ItemArea"));
                    GetComponent<RectTransform>().position = pos;

                    TargetGridGroup.itemList[myPosition] = this;
                    changeState(STATE.DEFAULT);
                    return;
                }
                if (checkGroup[GridGroup.ITEMCHECKTYPE.CHANGE]>-1)
                {
                    if (checkGroup[GridGroup.ITEMCHECKTYPE.CHANGE] == checkGroup[GridGroup.ITEMCHECKTYPE.OVERLAP])
                    {
                        int temEa = TargetGridGroup.itemList[checkGroup[GridGroup.ITEMCHECKTYPE.OVERLAP]].myData.Ea + myData.Ea - myData.Info_Obj.Data.maxEa;
                        TargetGridGroup.itemList[checkGroup[GridGroup.ITEMCHECKTYPE.OVERLAP]].myData.Ea = myData.Info_Obj.Data.maxEa;
                        this.myData.Ea = temEa;

                        TargetGridGroup.Inventory_Data.SetDirty();
                        return;
                    }
                    else
                    {
                        TargetGridGroup.itemList[checkGroup[GridGroup.ITEMCHECKTYPE.CHANGE]].changeState(STATE.GET);

                        TargetGridGroup.ItemIN(this);

                        Vector2 firstPos = TargetGridGroup.myGrid[myPosition].GetComponent<RectTransform>().position;
                        Vector2 lastPos = TargetGridGroup.myGrid[LastPosition].GetComponent<RectTransform>().position;
                        Vector2 pos = (firstPos + lastPos) / 2;

                        transform.SetParent(TargetGridGroup.transform.parent.Find("ItemArea"));
                        GetComponent<RectTransform>().position = pos;

                        TargetGridGroup.itemList[myPosition] = this;
                        changeState(STATE.DEFAULT);
                        return;
                    }
                }
            }
        }
    }
    
    public static GameObject ItemCreate(Transform a)
    {
        GameObject value = new GameObject();
        value.transform.SetParent(a);
        value.AddComponent<RectTransform>();
        value.AddComponent<Item>();
        value.AddComponent<Image>();

        return value;
    }
    private void Start()
    {
        myData.Initialize();
        Initialize();
    }
    private void Update()
    {
        StateProcess();
    }
}
