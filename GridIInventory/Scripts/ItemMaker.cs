using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ItemMaker : MonoBehaviour
{
    public class ItemName
    {
        ItemMaker PARENT;
        GameObject Obj;
        TMPro.TMP_InputField field;
        public string v
        {
            get
            {
                return field.text;
            }
            set
            {
                field.text = value;
            }
        }

        public ItemName(GameObject target, ItemMaker Pa)
        {
            Obj = target;
            PARENT = Pa;
            field = Obj.GetComponentInChildren<TMPro.TMP_InputField>();
        }
    }
    ItemName _itemName;

    public class ItemSize
    {
        ItemMaker PARENTS;

        public class slide_E
        {
            public float Limit = 3;
            public int nowValue
            {
                get
                {
                    return (int)(s.value * (Limit - 1)) + 1;
                }
                set
                {
                    s.value = value  / Limit;
                }
            }
            GameObject Obj;
            Slider _s;
            public Slider s
            {
                get
                {
                    return _s;
                }
                set
                {
                    _s = value;
                    s.onValueChanged.AddListener((float a)=> { int temp = nowValue; t.text = temp.ToString(); });
                }
            }
            public TMPro.TMP_Text t;

            public slide_E(GameObject Target,float Limit)
            {
                Obj = Target;
                this.Limit = Limit;
                if(Obj.GetComponentInChildren<Slider>()!=null) this.s = Obj.GetComponentInChildren<Slider>();
                if (Obj.GetComponentInChildren<TMPro.TMP_Text>() != null) this.t = s.GetComponentInChildren<TMPro.TMP_Text>();
                t.text = nowValue.ToString();
            }
        }

        GameObject Slider_Obj;
        slide_E X_Slider_Obj;
        slide_E Y_Slider_Obj;
        public Vector2 size
        {
            get
            {
                return new Vector2(float.Parse( X_Slider_Obj.t.text),float.Parse( Y_Slider_Obj.t.text));
            }
            set
            {
                X_Slider_Obj.nowValue = (int)value.x;
                Y_Slider_Obj.nowValue = (int)value.y;
            }
        }

        public ItemSize(GameObject Target,ItemMaker pa)
        {
            PARENTS = pa;
            Slider_Obj = Target;
            Slider[] temp = Target.GetComponentsInChildren<Slider>();
            if (temp.Length > 1)
            {
                X_Slider_Obj = new slide_E(temp[0].transform.parent. gameObject,3);
                Y_Slider_Obj = new slide_E(temp[1].transform.parent.gameObject,3);
            }
        }
    }
    ItemSize _itemsize;

    public class ItemCode
    {
        ItemMaker PARENTS;
        
        public string strItemCode
        {
            get
            {
                return input.text;
            }
            set
            {
                input.text = value;
            }
        }
        class ItemList
        {
            ItemMaker PARENTS;
            GameObject Target;
            Transform content;
            Button temp;

            Button[] ItemListButton;
            Item_Info_Obj[] data
            {
                set
                {
                    for (int i = content.childCount; i < value.Length; i++)
                    {
                        GameObject tempobj = Instantiate(temp.gameObject,content);
                    }
                    ItemListButton = content.GetComponentsInChildren<Button>();
                    for (int i = 0; i < value.Length; i++)
                    {
                        ItemListButton[i].onClick.RemoveAllListeners();
                        string a = value[i].Data.ItemCode;
                        ItemListButton[i].GetComponentInChildren<TMPro.TMP_Text>().text = a;
                        ItemListButton[i].onClick.AddListener(()=> 
                        {
                            ItemCall(a); 
                        });
                    }
                }
            }
            public void ItemCall(string target)
            {
                PARENTS._itemCode.input.text = target;
                ListOnOff();
            }
            public void ListOnOff()
            {
                //off
                if (Target.transform.parent.childCount - 1 > Target.transform.GetSiblingIndex())
                {
                    Target.transform.SetAsLastSibling();
                }
                //on
                else
                {
                    data = Resources.LoadAll<Item_Info_Obj>("Item_info_Data");
                    Target.transform.SetSiblingIndex(1);
                }
            }
            public ItemList(GameObject target, ItemMaker Pa)
            {
                PARENTS = Pa;
                Target = target;
                content = target.GetComponentInChildren<ContentSizeFitter>().transform;
                temp = content.GetComponentInChildren<Button>();
                data = Resources.LoadAll<Item_Info_Obj>("Item_info_Data");
            }
        }

        ItemList myList;

        TMPro.TMP_InputField _input;
       public  TMPro.TMP_InputField input
        {
            get
            {
                return _input;
            }
            set
            {
                _input = value;
                input.onValueChanged.RemoveAllListeners();
                input.onValueChanged.AddListener((string a)=> 
                {
                    TMPro.TMP_Text tempText = input.GetComponentsInChildren<TMPro.TMP_Text>()[1];
                    tempText.color = Color.red;
                    PARENTS. Data = Resources.Load<Item_Info_Obj>("Item_info_Data/"+input.text);
                });
            }
        }
        Button _ListCall;
        Button ListCall
        {
            get
            {
                return _ListCall;
            }
            set
            {
                _ListCall = value;
                ListCall.onClick.RemoveAllListeners();
                ListCall.onClick.AddListener(()=>{ myList.ListOnOff(); });
            }
        }
        Button _ReSet;
        Button ReSet
        {
            get
            {
                return _ReSet;
            }
            set
            {
                _ReSet = value;
                ReSet.onClick.RemoveAllListeners();
                ReSet.onClick.AddListener(()=> { InputReset(); });
            }
        }
        void InputReset()
        {
            input.readOnly = false;
            input.text = "";
        }

        public ItemCode(GameObject target,ItemMaker pa)
        {
            PARENTS = pa;
            input= target.GetComponentInChildren<TMPro.TMP_InputField>();
            myList = new ItemList(target.transform.parent.GetChild(target.transform.parent.GetChildCount()-1).gameObject, PARENTS);
            Button[] tempbutton =  target.GetComponentsInChildren<Button>();
            ListCall = tempbutton[0];
            ReSet = tempbutton[1];
            PARENTS.Data = null;
        }
    }
    ItemCode _itemCode;

    public class ItemEa
    {
        ItemMaker PARENT;
        GameObject Obj;
        ItemSize.slide_E MaxEaSlider;
        ItemSize.slide_E NowEaSlider;
        public void MaxEa_Controll(GameObject Target)
        {
            NowEaSlider = new ItemSize.slide_E(Target,MaxEaSlider.nowValue);
          
        }
        public int max
        {
            get
            {
                return MaxEaSlider.nowValue;
            }
            set
            {
                MaxEaSlider.nowValue =  value;
            }
        }
        public int now
        {
            get
            {
                return NowEaSlider.nowValue;
            }
            set
            {
                NowEaSlider.nowValue = value;
            }
        }
        public ItemEa(GameObject Target, ItemMaker Pa)
        {
            PARENT = Pa;
            Obj = Target;
            Slider[] temp = Obj.GetComponentsInChildren<Slider>();
            if (temp.Length > 1)
            {
                MaxEaSlider = new ItemSize.slide_E(temp[0].transform.parent.gameObject,100);
                MaxEa_Controll(temp[1].transform.parent.gameObject);

                MaxEaSlider.s.onValueChanged.AddListener((float a) => { MaxEa_Controll(temp[1].transform.parent.gameObject); });
            }
        }
    }
    ItemEa _itemEa;

    public GameObject ItemNameObj;
    GameObject itemName
    {
        set
        {
            _itemName = new ItemName(value,this);
        }
    }

    public GameObject itemSizeObj;
    GameObject itemsize
    {
        set
        {
            _itemsize = new ItemSize(value,this);
        }
    }

    public GameObject itemCodeObj;
    GameObject itemCode
    {
        set
        {
            _itemCode = new ItemCode(value,this);
        }
    }


    public GameObject itemEaObj;
    GameObject itemEa
    {
        set
        {
            _itemEa = new ItemEa(value,this);
        }
    }



    public Button ItemMakeButton;
    public Button ItemDataEdit;


    Item_Info_Obj _Data;
    public Item_Info_Obj Data
    {
        get
        {
            return _Data;
        }
        set
        {
            _Data = value;
            if (ItemMakeButton != null) { ItemMakeButton.enabled = false; ItemMakeButton.image.color = Color.gray; }
            if (ItemDataEdit != null) {ItemDataEdit.GetComponentInChildren<TMPro.TMP_Text>().text = "DataMake"; }
            if (_itemsize != null && Data != null)
            {
                _itemsize.size = Data.Data.ItemSize;
                _itemEa.max = Data.Data.maxEa;
                _itemName.v = Data.Data.ItemName;
                if (Data != null) _itemCode.input.GetComponentsInChildren<TMPro.TMP_Text>()[1].color = Color.blue;
                if (ItemMakeButton != null) { ItemMakeButton.enabled = true; ItemMakeButton.image.color = Color.white; }
                if (ItemDataEdit != null) { ItemDataEdit.GetComponentInChildren<TMPro.TMP_Text>().text = "DataEdit"; }
            }
        }
    }

    Item_Info DataEdit()
    {
        Item_Info value = new Item_Info();

        value.ItemName = _itemName.v;
        value.ItemSize =  new Vector2Int( (int)_itemsize.size.x,(int)_itemsize.size.y);
        value.ItemCode = _itemCode.strItemCode;
        value.maxEa = _itemEa.max;


        return value;
    }

    public void ItemDataEditMake()
    {
        if (Data != null)
        { 
            Data.Data = DataEdit();
            Data.SetDirty();
        }
        else
        {
            string tempItemCode = _itemCode.strItemCode;
            if (tempItemCode.Length > 0)
            {
                Item_Info_Obj tempData = ScriptableObjectUtility.CreateAsset<Item_Info_Obj>(Item_Info_Obj.Item_Info_Obj_Path, tempItemCode);
                tempData.Data = DataEdit();

                tempData.SetDirty();
            }
        }
    }

    public GameObject viewWindow;
    public void ItemMakeFunction()
    {
        GameObject temp = Item.ItemCreate(viewWindow.transform);
        if (Data != null) temp.GetComponent<Item>().myData.Info_Obj = Data;
        temp.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        temp.GetComponent<Item>().myData.Ea = _itemEa.now;
    }
    void Start()
    {
        if (itemSizeObj != null) itemsize = itemSizeObj;
        if (itemCodeObj != null) itemCode = itemCodeObj;
        if (ItemMakeButton != null)
        { 
            ItemMakeButton.onClick.RemoveAllListeners();
            ItemMakeButton.onClick.AddListener(()=> { ItemMakeFunction(); });
        }
        if (ItemDataEdit != null)
        {
            ItemDataEdit.onClick.RemoveAllListeners();
            ItemDataEdit.onClick.AddListener(() => { ItemDataEditMake(); });
        }
        if (itemEaObj != null) itemEa = itemEaObj;
        if (ItemNameObj != null) itemName = ItemNameObj;

    }
}
