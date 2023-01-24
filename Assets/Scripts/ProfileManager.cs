using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{

    public GameObject shopDetails;
    public GameObject profiles;
    public GameObject userBack;

    // Start is called before the first frame update
    void Start()
    {
        shopDetails.SetActive(false);
        profiles.SetActive(true);
        userBack.SetActive(false);
    }


    public void OnAdminButtonClick()
    {
        profiles.SetActive(false);
        shopDetails.SetActive(true);
        userBack.SetActive(false);
    }

    public void OnUserButtonClick()
    {
        profiles.SetActive(false);
        userBack.SetActive(true);
       
    }

    public void OnBackButtonClick()
    {

        shopDetails.SetActive(false);
        profiles.SetActive(true);
        userBack.SetActive(false);

    }

}
