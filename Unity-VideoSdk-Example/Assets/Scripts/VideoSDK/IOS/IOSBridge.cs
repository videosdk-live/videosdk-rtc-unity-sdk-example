using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_IOS
public class IOSBridge : MonoBehaviour
{
    // Import Objective-C methods
    [DllImport("__Internal")]
    private static extern void sendMessageToUnity(string message);

    [DllImport("__Internal")]
    private static extern string receiveMessageFromUnity();

    // Method to call Objective-C
    public void SendMessageToiOS(string message)
    {
        Debug.Log("Send Message :" + message);
        sendMessageToUnity(message);
    }

    public string GetMessageFromiOS()
    {
        Debug.Log("Receive Message :"+ receiveMessageFromUnity());
        return receiveMessageFromUnity();
    }

    public void SendMessage()
    {
        SendMessageToiOS("Yup It's working");
    }
}
#endif
