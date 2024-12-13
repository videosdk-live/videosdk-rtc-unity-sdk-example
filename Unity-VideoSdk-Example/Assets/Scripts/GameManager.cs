using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live.videosdk;
using UnityEngine.Android;
using TMPro;
public class GameManager : MonoBehaviour
{
    private bool micToggle;
    private bool camToggle;

    [SerializeField] GameObject _videoSurfacePrefab;
    [SerializeField] Transform _parent;
    [SerializeField] GameObject _meetControlls;
    [SerializeField] GameObject _meetCreateActivity;
    [SerializeField] GameObject _meetJoinActivity;

    private VideoSurface _localParticipant;
    private Meeting videosdk;
    private readonly string _token = "YOUR_TOKEN";
    
    [SerializeField] TMP_Text _meetIdTxt;
    [SerializeField] TMP_InputField _meetIdInputField;

    private List<VideoSurface> _participantList = new List<VideoSurface>();

    private void Awake()
    {
        _meetControlls.SetActive(false);
        _meetCreateActivity.SetActive(false);
        _meetJoinActivity.SetActive(false);
        RequestForPermission(Permission.Camera);
    }
    void Start()
    {
        videosdk = Meeting.GetMeetingObject();

        videosdk.OnCreateMeetingIdCallback += OnCreateMeet;
        videosdk.OnParticipantJoinedCallback += OnParticipantJoined;
        videosdk.OnParticipantLeftCallback += OnParticipantLeft;
        videosdk.OnCreateMeetingIdFailedCallback += OnCreateMeetFailed;
        videosdk.OnErrorCallback += OnError;
        _meetCreateActivity.SetActive(true);
        _meetJoinActivity.SetActive(true);
    }

    private void OnError(Error error)
    {
        Debug.LogError($"Error-Code: {error.Code} message: {error.Message}");
    }

    private void OnParticipantJoined(IParticipant obj)
    {   
        Debug.Log($"On Pariticpant Joined: " + obj.ToString());
        VideoSurface participant = Instantiate(_videoSurfacePrefab, _parent.transform).GetComponentInChildren<VideoSurface>();
        participant.SetVideoSurfaceType(VideoSurfaceType.RawImage);//For raw Image
        participant.SetParticipant(obj);
        participant.SetEnable(true);
        _participantList.Add(participant);
        if (obj.IsLocal)
        {
            _localParticipant = participant;

             _localParticipant.OnStreamEnableCallback += OnStreamEnable;
             _localParticipant.OnStreamDisableCallback += OnStreamDisable;


            _meetIdTxt.text = _meetIdInputField.text;
            _meetIdInputField.text = string.Empty;
            //_meetIdInputField.Select();
            _meetCreateActivity.SetActive(false);
            _meetJoinActivity.SetActive(false);
            _meetControlls.SetActive(true);

        }
    }

    private void OnStreamDisable(string kind)
    {
        Debug.Log($"OnStreamDisable {kind}");
        camToggle = _localParticipant.CamEnabled;
        micToggle = _localParticipant.MicEnabled;
    }

    private void OnStreamEnable(string kind)
    {
        Debug.Log($"OnStreamEnable {kind}");
        camToggle = _localParticipant.CamEnabled;
        micToggle = _localParticipant.MicEnabled;
    }

    private void OnParticipantLeft(IParticipant obj)
    {
        Debug.Log($"On Pariticpant Left: " + obj.ToString());

        if (obj.IsLocal)
        {
            OnLeave();
        }
        else
        {
            VideoSurface participant = null;
            for (int i = 0; i < _participantList.Count; i++)
            {
                if(obj.ParticipantId== _participantList[i].Id)
                {
                    participant = _participantList[i];
                    break;
                }
                
            }
            if(participant!=null)
            {
                _participantList.Remove(participant);
                Destroy(participant.transform.parent.gameObject);
            }
        }
    }

    private void OnLeave()
    {
        _meetCreateActivity.SetActive(true);
        _meetJoinActivity.SetActive(true);
        _meetControlls.SetActive(false);
        camToggle = true;
        micToggle = true;
        for (int i = 0; i < _participantList.Count; i++)
        {
            Destroy(_participantList[i].transform.parent.gameObject);
        }
        _participantList.Clear();
        _meetIdTxt.text = "VideoSDK Unity Demo";
    }

    private void OnCreateMeet(string meetId)
    {
        _meetIdTxt.text = meetId;
        videosdk.Join(_token, meetId, "User", true, true);
    }

    public void CreateMeeting()
    {
        Debug.Log("User Request for Create meet-ID");
        _meetCreateActivity.SetActive(false);
        _meetJoinActivity.SetActive(false);
        videosdk.CreateMeetingId(_token);
    }

    private void OnCreateMeetFailed(string obj)
    {
        _meetCreateActivity.SetActive(true);
        _meetJoinActivity.SetActive(true);
        Debug.LogError(obj);
    }

    public void JoinMeet()
    {
        Debug.Log("User Request for join meet");
        if (string.IsNullOrEmpty(_meetIdInputField.text)) return;
       
        videosdk.Join(_token, _meetIdInputField.text,"User", true, true);
    }

    public void CamToggle()
    {
        camToggle = !camToggle;
        Debug.Log("Cam Toggle " + camToggle);
        _localParticipant?.SetVideo(camToggle);
    }
    public void AudioToggle()
    {
        micToggle = !micToggle;
        Debug.Log("Mic Toggle " + micToggle);
        _localParticipant?.SetAudio(micToggle);
    }

    public void LeaveMeeting()
    {
        videosdk?.Leave();
    }


    private void OnPermissionGranted(string permissionName)
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone) && Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            return;
        }
        RequestForPermission(Permission.Microphone);

    }

    private void OnPermissionDenied(string permissionName)
    {
        Debug.LogError($"VideoSDK can't Initialize {permissionName} Denied");

    }

    private void OnPermissionDeniedAndDontAskAgain(string permissionName)
    {
        Debug.LogError($"VideoSDK can't Initialize {permissionName} Denied And DontAskAgain");
    }


    private void RequestForPermission(string permission)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Permission.HasUserAuthorizedPermission(Permission.Microphone) && Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                // The user authorized use of the microphone.
                OnPermissionGranted("");
            }
            else
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += OnPermissionDenied;
                callbacks.PermissionGranted += OnPermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += OnPermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(permission, callbacks);
            }
        }

    }


}
