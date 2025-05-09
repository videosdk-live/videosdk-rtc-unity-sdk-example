using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeviceCloneController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deviceNameText;
    [SerializeField] private GameObject checkBox;
    public Button button;

    public void SetData(string deviceName)
    {
        deviceNameText.text = deviceName;
    }

    public void SelectDevice(bool isSelect)
    {
        checkBox.SetActive(isSelect);
    }
}
