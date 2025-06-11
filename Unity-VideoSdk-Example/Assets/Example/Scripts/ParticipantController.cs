using live.videosdk;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParticipantController : MonoBehaviour
{
    [SerializeField] public VideoSurface participant { get; private set; }
    [SerializeField] private Button remoteMic, remoteCam, removeUser;
    [SerializeField] private Sprite micOn, micOff, camOn, camOff;
    [SerializeField] private TextMeshProUGUI userIdtext;
    public void SetParticipant(VideoSurface participant)
    {
        this.participant = participant;
        userIdtext.text = this.participant.IsLocal ? "You" : this.participant.ParticipantId;
    }
    private void Start()
    {
        if (participant.IsLocal)
        {
            remoteMic.gameObject.SetActive(false);
            remoteCam.gameObject.SetActive(false);
            removeUser.gameObject.SetActive(false);
        }
        participant.OnStreamEnableCallback += OnRemoteStreamEnable;
        participant.OnStreamDisableCallback += OnRemoteStreamDisable;

        SetRemoteToggle();

    }

    private void SetRemoteToggle()
    {
        remoteCamToggle = participant.CamEnabled;
        remoteMicToggle = participant.MicEnabled;
    }

    private void OnRemoteStreamDisable(StreamKind kind)
    {
        Debug.Log($"OnRemoteStreamDisable {kind}");
        remoteCamToggle = participant.CamEnabled;
        remoteMicToggle = participant.MicEnabled;
    }

    private void OnRemoteStreamEnable(StreamKind kind)
    {
        Debug.Log($"OnRemoteStreamEnable {kind}");
        remoteCamToggle = participant.CamEnabled;
        remoteMicToggle = participant.MicEnabled;
    }

    private bool _remoteMicToggle = true;
    private bool remoteMicToggle
    {
        get => _remoteMicToggle;
        set
        {
            remoteMic.image.sprite = value ? micOn : micOff;
            _remoteMicToggle = value;
        }
    }
    private bool _remoteCamToggle = true;
    private bool remoteCamToggle
    {
        get => _remoteCamToggle;
        set
        {
            Debug.Log($"set _camToggle {value}");
            remoteCam.image.sprite = value ? camOn : camOff;
            _remoteCamToggle = value;
        }
    }

    // Assign on button
    public void RemoteMicToggle()
    {
        bool status = !participant.MicEnabled;
        participant.SetAudio(status);
    }
    // Assign on button
    public void RemoteWebCamToggle()
    {
        bool status = !participant.CamEnabled;
        participant.SetVideo(status);
    }
    // Assign on button
    public void RemoveRemoteParticipant()
    {
        participant.Remove();
    }

}
