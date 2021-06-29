using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemSystem : MonoBehaviour
{
    public float ItemGridSize = 50;
    public static float ItemGridSizeGet = 100;
      static string _GridImageDirPath = "Item_Image/";
     static string _GridImagePath = "Grid";
    public static string GridImagePath
    {
        get
        {
            return _GridImageDirPath + _GridImagePath;
        }
    }
}
