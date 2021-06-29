using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Grid_Info
{
    public int GridPosition = -1;
    public Item_Data HaveItem = null;
}
public class Grid : MonoBehaviour
{
    public int GridNumber = 0;
    bool _Active = false;
    public bool Active
    {
        get
        {
            return _Active;
        }
        set
        {
            _Active = value;

            if (value) GetComponent<Image>().color = Color.gray;
            else GetComponent<Image>().color = Color.white;
        }
    }
}
