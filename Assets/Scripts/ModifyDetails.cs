using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModifyDetails : MonoBehaviour
{
    public TextMeshPro shopName;
    public TextMeshPro shopDiscount;
    public TextMeshPro shopfeatured;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void changeShopDetails(string Name, string Discount, string featured)
    {
        shopName.text = Name;
        shopDiscount.text = Discount + " %";
        shopfeatured.text = featured;
    }
}
