using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace AR_Fukuoka
{
    public class SampleScript : MonoBehaviour
    {
        //GeospatialAPI
        public AREarthManager EarthManager;
        //GeospatialAPI ARCore
        public VpsInitializer Initializer;
        public Text OutputText;
        public double HeadingThreshold = 25;
        public double HorizontalThreshold = 20;
        public TMP_InputField nameObject;
        public TMP_InputField discountObject;
        public TMP_InputField featuredObject;

        public TextMeshProUGUI log;
        public double Latitude;
        public double Longitude;
        public double Altitude;
        public double Heading;
        public ARAnchorManager AnchorManager;
        public GameObject ContentPrefab;

        GameObject displayObject;
        GeospatialPose pose;
        string shopName="", discount="" , featured = "";

        // Start is called before the first frame update
        void Start()
        {
            
        }

        public void updateCoordinates()
        {
           // log.text = "seting empty";
         //   displayObject = null;
           // Latitude = Convert.ToDouble(LatitudeObject.text.ToString());
          //  Longitude = Convert.ToDouble(LongitudeObject.text.ToString());
          //  Altitude = Convert.ToDouble(AltitudeObject.text.ToString());

          //  Debug.Log("Latitude value is :" + Latitude + "Longitude value is :" + Longitude + "altitude value is :" + Altitude);
        }

        // Update is called once per frame
        void Update()
        {
            //string status = "";
            OutputText.text = Initializer.IsReady + ", " + EarthManager.EarthTrackingState + ", ";

            pose = EarthManager.CameraGeospatialPose;


            // ShowTrackingInfo(status, pose);
            ShowTrackingInfo(pose);

        }

        void ShowTrackingInfo(GeospatialPose pose)
        {
            OutputText.text = string.Format(
                "Latitude/Longitude: {0}°, {1}°\n" +
                "Horizontal Accuracy: {2}m\n" +
                "Altitude: {3}m\n",
                "Vertical Accuracy: {4}m\n" +
                "Heading: {5}°\n" +
                "Heading Accuracy: {6}°\n"
                //"{7} \n"
                ,
                pose.Latitude.ToString("F6"),  //{0}   // f6  means 6 numbers after the decimal points
                pose.Longitude.ToString("F6"), //{1}
                pose.HorizontalAccuracy.ToString("F6"), //{2}
                pose.Altitude.ToString("F2"),  //{3}
                pose.VerticalAccuracy.ToString("F2"),  //{4}
                pose.EunRotation.ToString("F1"),   //{5}
                pose.OrientationYawAccuracy.ToString("F1")   //{6}
                //status //{7}
            );
        }

        public void retrieveDetails(bool isAdmin)
        {
            if (isAdmin == true)
            {
                Latitude = Convert.ToDouble(pose.Latitude.ToString("F6"));
                Longitude = Convert.ToDouble(pose.Longitude.ToString("F6"));
                Altitude = Convert.ToDouble(pose.Altitude.ToString("F2"));
                shopName = nameObject.text;
                discount = discountObject.text;
                featured = featuredObject.text;
            }
            
        }

        public void addDetails(bool isAdmin)
        {
            log.text = "Null  object";
            displayObject = null;
            Debug.Log("add called");
            //retrieveDetails(true);
           // StartCoroutine(Upload());
            //if (/*!Initializer.IsReady || */EarthManager.EarthTrackingState != TrackingState.Tracking)
            //{
            //    return;
            //}

           

            if (pose.OrientationYawAccuracy > HeadingThreshold ||
                 pose.HorizontalAccuracy > HorizontalThreshold)
            {
                //status = "Low Tracking Accuracy";
            }
            else
            {
                //status = "High Tracking Accuracy";
                retrieveDetails(isAdmin);

                if (displayObject == null)
                {
                    //Height of the phone - 1.5 m to be approximately the height of the ground
                    //Altitude = pose.Altitude - 1.5f;
                    //Angle correction (Anchor generation function assumes South=0)
                    Quaternion quaternion = Quaternion.AngleAxis(180f - (float)Heading, Vector3.up);
                    ARGeospatialAnchor anchor = AnchorManager.AddAnchor(Latitude, Longitude, Altitude, quaternion);
                    //For terrain get the details from API
                    //ARGeospatialAnchor anchor =  AnchorManager.ResolveAnchorOnTerrain(Latitude, Longitude, 0, quaternion);

                    if (anchor != null)
                    {
                        log.text = isAdmin.ToString() + " , " + Latitude + " , " + Longitude + " , " + Altitude + " , " + shopName + " , " + discount + " , " + featured;
                        displayObject = Instantiate(ContentPrefab, anchor.transform);
                        log.text = displayObject ? "there" : "gone";
                        displayObject.GetComponent<ModifyDetails>().changeShopDetails(shopName , discount , featured);
                        log.text = "object instantiated";
                        if (isAdmin == true)
                        {
                            StartCoroutine(Upload());
                        }
                        

                    }
                }
            }

            // send the details to api

        }



        IEnumerator Upload()
        {
            //WWWForm form = new WWWForm();
            //form.AddField("shopName", "qw23");


            //UnityWebRequest www = UnityWebRequest.Post("https://geospatialapi.azurewebsites.net/api/GeoSpatial", form);
            //www.chunkedTransfer = false;
            //www.uploadHandler.contentType = "application/json";

            //yield return www.SendWebRequest();

            //if (www.result != UnityWebRequest.Result.Success)
            //{
            //    Debug.Log(www.error);
            //}
            //else
            //{
            //    string results = www.downloadHandler.text;
            //    Debug.Log(results);
            //}

            //www.Dispose();

            var user = new GeoSpatialResponse();
            user.shopName = shopName;
            user.latitude = Latitude.ToString();
            user.longitude = Longitude.ToString();
            user.altitude = Altitude.ToString();
            user.discount = discount;
            user.featured = featured;

            string json = JsonUtility.ToJson(user);

            var req = new UnityWebRequest("https://geospatialapi.azurewebsites.net/api/GeoSpatial", "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return req.SendWebRequest();

            if (req.isNetworkError)
            {
                Debug.Log("Error While Sending: " + req.error);
            }
            else
            {
                Debug.Log("Received: " + req.downloadHandler.text);
            }
        }


        public void showDetails()
        {
            StartCoroutine(Retrieve());
        }


        IEnumerator Retrieve()
        {

            log.text = "Retrieving";
            UnityWebRequest www = UnityWebRequest.Get("https://geospatialapi.azurewebsites.net/api/GeoSpatial");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                log.text = www.error;
            }
            else
            {
                string results = www.downloadHandler.text;
                string jsonString = fixJson(results);
                Debug.Log("Json response : " + jsonString);
                log.text = results;
                //GeoSpatialResponse[] yourClass;
                //yourClass = JsonHelper.FromJson<GeoSpatialResponse>(jsonString); 

                //Debug.Log(yourClass[0].shopName);
                //foreach (var i in yourClass)
                //{
                //    Debug.Log(i.shopName);
                //}
                List<GeoSpatialResponse> deserializedArray = JsonConvert.DeserializeObject<List<GeoSpatialResponse>>(results);
                foreach (var a in deserializedArray)
                {
                    log.text = "loop";
                    Latitude = Convert.ToDouble(a.latitude);
                    Longitude = Convert.ToDouble(a.longitude);
                    Altitude = Convert.ToDouble(a.altitude);
                    shopName = a.shopName;
                    discount = a.discount;
                    featured = a.featured;
                    log.text = shopName + " , " + discount;
                    addDetails(false);
                }
            }

            www.Dispose();

        }

        string fixJson(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }


    }
    //public static class JsonHelper
    //{
    //    public static T[] FromJson<T>(string json)
    //    {
    //        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
    //        return wrapper.Items;
    //    }

    //    public static string ToJson<T>(T[] array)
    //    {
    //        Wrapper<T> wrapper = new Wrapper<T>();
    //        wrapper.Items = array;
    //        return JsonUtility.ToJson(wrapper);
    //    }

    //    public static string ToJson<T>(T[] array, bool prettyPrint)
    //    {
    //        Wrapper<T> wrapper = new Wrapper<T>();
    //        wrapper.Items = array;
    //        return JsonUtility.ToJson(wrapper, prettyPrint);
    //    }

    //    [Serializable]
    //    private class Wrapper<T>
    //    {
    //        public T[] Items;
    //    }
    //}

    public class GeoSpatialResponse
    {
        public int shopId;
        public string shopName;
        public string latitude;
        public string longitude;
        public string altitude;
        public string discount;
        public string featured;
    }

    
}


