using System.Collections;
using System.Collections.Generic;
using System.IO;
using EasyMobile;
using UnityEngine;
using UnityEngine.UIElements;
using CameraType = EasyMobile.CameraType;

public class ReadQRCode : MonoBehaviour
{
    void startScanning()
    {
        CameraType cameraType = CameraType.Rear;
        bool result = Media.Camera.IsCameraAvailable(cameraType);
        
        Media.Camera.TakePicture(cameraType, TakePictureCallback);
    }
    
    private void TakePictureCallback(string error, MediaResult result)
    {
        if (!string.IsNullOrEmpty(error))
        { 
            // This means users failed to take picture,
            // you should show the error to them.
        }
        else
        {
            // This means the picture has been taken successfully.
            // Now can get the image uri and load it manually.
            string uri = result.Uri;

            // Or you can use the built-in method to 
            // load the taken picture whenever needed. 
        }
    }
}