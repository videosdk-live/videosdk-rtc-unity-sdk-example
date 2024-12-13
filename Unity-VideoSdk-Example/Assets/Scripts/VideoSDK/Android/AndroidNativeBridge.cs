using UnityEngine;

public class AndroidNativeBridge : MonoBehaviour
{
    private AndroidJavaObject _nativeBridgePlugin;

    void Start()
    {
        // Load the plugin
        _nativeBridgePlugin = new AndroidJavaObject("com.artoon.nativeandroidbridgelib.NativePlugin");

        // Register Unity callback
        _nativeBridgePlugin.CallStatic("registerCallback", new UnityCallback());
    }

    // Call native method
    public void GetWelcomeMessage()
    {
        string message = _nativeBridgePlugin.CallStatic<string>("getWelcomeMessage");
        Debug.Log("Message from Android: " + message);
    }

    // Callback class to receive messages from Android
    private class UnityCallback : AndroidJavaProxy
    {
        public UnityCallback() : base("com.artoon.nativeandroidbridgelib.NativePlugin$UnityCallback") { }

        public void onMessage(string message)
        {
            Debug.Log("Received message from Android: " + message);
        }
    }
}
