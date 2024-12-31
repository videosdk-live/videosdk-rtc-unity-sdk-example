## How to install the VideoSDK package? 

1. Open Unity’s Package Manager by selecting from the top bar:
   **Window -> Package Manager**.

2. Click the **+** button in the top left corner and select **Add package from git URL...**

3. Paste the following URL and click **Add**:

   ```jsx
   https://github.com/videosdk-live/videosdk-rtc-unity-sdk.git
   ```
4. Add the `com.unity.nuget.newtonsoft-json` package by following the instructions provided [here](https://github.com/applejag/Newtonsoft.Json-for-Unity/wiki/Install-official-via-UPM
).

## Android Setup

- Add the repository to `settingsTemplate.gradle` file in your project.

```jsx
dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.PREFER_SETTINGS)
    repositories {
        **ARTIFACTORYREPOSITORY**
        google()
        mavenCentral()
        jcenter()
         maven {
            url = uri("https://maven.aliyun.com/repository/jcenter")
        }
        flatDir {
            dirs "${project(':unityLibrary').projectDir}/libs"
        }
    }
}
```

- Install our Android SDK in `mainTemplate.gradle`

```jsx
dependencies {
     //...
    implementation 'live.videosdk:rtc-android-sdk:0.1.37'
**DEPS**}
```

- If your project has set `android.useAndroidX=true`, 
then set `android.enableJetifier=true` in the `gradleTemplate.properties` file to migrate your project to AndroidX and avoid duplicate class conflict.


```jsx
//...
**ADDITIONAL_PROPERTIES**
android.enableJetifier=true
android.useAndroidX=true
android.suppressUnsupportedCompileSdk=34
```

## iOS Setup

- To run it on iOS, build the project from Unity for iOS.

- After building the project for iOS, open the Xcode project and navigate to the Unity-iPhone target.

- Under Frameworks, Libraries and Embedded Content of the General tab, add the VideoSDK and related frameworks.

![iOS Integration Step 1](https://cdn.videosdk.live/website-resources/docs-resources/unity_ios_integration1.png)
![iOS Integration Step 2](https://cdn.videosdk.live/website-resources/docs-resources/unity_ios_integration2.png)

# Integration

## Initializing the SDK

```csharp
Meeting meeting = Meeting.GetMeetingObject();
```

## Creating a Meeting ID

### Create a Meeting ID

Use the `CreateId` method with a token to generate a meeting ID. Refer to the [Token Generation Guide](https://docs.videosdk.live/android/guide/video-and-audio-calling-api-sdk/authentication-and-token) for details.

```csharp
public void CreateMeeting()
{
   meeting.CreateMeetingId("YOUR_TOKEN");
}
```

### Handle Callbacks

#### Success Callback

- Once the meeting ID is successfully created, it will be received through the `OnCreateMeetingIdCallback`.

```csharp
private void OnCreateMeetingId(string meetingId)
{
   Debug.Log($"MeetingId: " + meetingId);
}

void Start()
{
    Meeting meeting = Meeting.GetMeetingObject();
    meeting.OnCreateMeetingIdCallback += OnCreateMeetingId;
}
```

#### Failure Callback

- If there's an issue and the meeting ID is not created, you will receive the `OnCreateMeetingIdFailedCallback` with the corresponding error message.

```csharp
private void OnCreateMeetingIdFailed(string message)
{
   Debug.Log($"Exception Message: " + message);
}

void Start()
{
    Meeting meeting = Meeting.GetMeetingObject();
    meeting.OnCreateMeetingIdFailedCallback += OnCreateMeetingIdFailed;
}
```

## Joining a Meeting

To join a meeting, use the `Join` method with the following parameters:

### Parameters:

| Parameter     | Type     | Required | Description                                        |
|---------------|----------|----------|----------------------------------------------------|
| `token`       | `string` | Yes      | Authentication token.                             |
| `meetingId`   | `string` | Yes      | Unique meeting ID.                                |
| `name`        | `string` | Yes      | Participant's display name.                       |
| `micEnabled`  | `bool`   | Yes      | Whether the microphone is enabled on join.        |
| `camEnabled`  | `bool`   | Yes      | Whether the camera is enabled on join.            |
| `participantId` | `string` | No     | Unique participant ID (optional).                 |

### Example:

```csharp
meeting.Join("YOUR_TOKEN", "MEETING_ID", "PARTICIPANT_NAME", true, true);
```

## Managing Participants

### Participant Callbacks

#### 1. OnParticipantJoinedCallback

- This event is triggered when a local participant (yourself) or a new remote participant joins the meeting.

**Parameters:**
- `IParticipant` containing:
  - `participantId`: The ID of the participant who joined.
  - `name`: The name of the participant who joined.
  - `isLocal`: Boolean indicating if the participant is local.

#### 2. OnParticipantLeftCallback

- This event is triggered when a local participant (yourself) or a new remote participant leaves the meeting.

**Parameters:**
- `IParticipant` containing:
  - `participantId`: The ID of the participant who left.
  - `name`: The name of the participant who left.
  - `isLocal`: Boolean indicating if the participant is local.

```csharp
private void OnParticipantJoined(IParticipant participant)
{
    Debug.Log($"On Participant Joined: " + participant.ToString());
}

private void OnParticipantLeft(IParticipant participant)
{
    Debug.Log($"On Participant Left: " + participant.ToString());
}

void Start()
{
    Meeting meeting = Meeting.GetMeetingObject();
    meeting.OnParticipantJoinedCallback += OnParticipantJoined;
    meeting.OnParticipantLeftCallback += OnParticipantLeft;
}
```

## Rendering Video

**Step 1** : Create a `RawImage` named `ParticipantView`.

**Step 2**:  Attach the `VideoSurface` script to your object. This can be done either at runtime or directly in the editor. In the below example, the script is being attached at runtime.

- `VideoSurface` is a MonoBehaviour script, which is responsible for render frames. Through this, you don't have to worry about manually drawing the video;. `VideoSurface` will take care of it.

**Step 3**: Call `SetParticipant()` with `IParticipant`  parameter. It will create new participant and attached it with `VideoSurface`

**Step 4**: Call `SetEnable()` with `true` parameter to register callbacks.

**Step 5**:  You can configure the render surface type using the `SetVideoSurfaceType()` method.
  - If you are using a `RawImage`, set the surface type to `VideoSurfaceType.RawImage`.
  - If you are using a Render Texture, set the surface type to `VideoSurfaceType.Renderer`.

```csharp
private void OnParticipantJoined(IParticipant participant)
{
  Debug.Log($"On Participant Joined: " + participant.ToString());
  GameObject go = GameObject.Find("ParticipantView");

  if (participantView == null)
  {
      participantView = go.AddComponent<VideoSurface>();
  }

  participantView.SetParticipant(participant);
  participantView.SetEnable(true);
  participantView.SetVideoSurfaceType(VideoSurfaceType.RawImage); // For Raw Image
  // participantView.SetVideoSurfaceType(VideoSurfaceType.Renderer); // For Render Texture
}
```

## Managing Streams

### Callbacks

#### OnStreamEnabledCallback
- This callback is triggered whenever a participant's video or audio stream is enabled.

#### OnStreamDisabledCallback
- This callback is triggered whenever a participant's video, audio stream is disabled.

```csharp
private void OnParticipantJoined(IParticipant participant)
{
    Debug.Log($"On Participant Joined: " + participant.ToString());

    participant.OnStreamEnabledCallback += (kind) => {
        Debug.Log($"On Stream Enable: " + kind);
    };

    participant.OnStreamDisableCallback += (kind) => {
        Debug.Log($"On Stream Disable: " + kind);
    };
}
```

## Get the State of a Participant's Media

```csharp
 private VideoSurface _localParticipant;
 
 private void OnParticipantJoined(IParticipant participant)
 {
    GameObject go = GameObject.Find("ParticipantView");

    if (participantView == null)
    {
        participantView = go.AddComponent<VideoSurface>();
    }
    
   participantView.SetParticipant(pariticipant);
   participantView.SetEnable(true);

    if (participantView.IsLocal)
    {
        _localParticipant = participantView;
    }
 }

 public void GetCamState()
 {
    _localParticipant.CamEnabled  // Returns true if the camera is enabled
 }
 
 public void GetMicState()
 {
   _localParticipant.MicEnabled  // Returns true if the microphone is enabled
 }
```

## Enabling/Disabling Local Participant's Camera and Microphone

### Enable/Disable Camera

```csharp
private VideoSurface _localParticipant;

private void OnParticipantJoined(IParticipant participant)
{
  GameObject go = GameObject.Find("ParticipantView");

    if (participantView == null)
    {
        participantView = go.AddComponent<VideoSurface>();
    }
    
   participantView.SetParticipant(pariticipant);
   participantView.SetEnable(true);

    if (participantView.IsLocal)
    {
        _localParticipant = participantView;
    }
}

public void CamEnable()
{
    _localParticipant?.SetVideo(true);
}

public void CamDisable()
{
    _localParticipant?.SetVideo(false);
}
```

### Enable/Disable Microphone

```csharp
private void OnParticipantJoined(IParticipant participant)
{
  GameObject go = GameObject.Find("ParticipantView");

    if (participantView == null)
    {
        participantView = go.AddComponent<VideoSurface>();
    }
    
   participantView.SetParticipant(pariticipant);
   participantView.SetEnable(true);

    if (participantView.IsLocal)
    {
        _localParticipant = participantView;
    }
}

public void MicEnable()
{
    _localParticipant?.SetAudio(true);
}

public void MicDisable()
{
    _localParticipant?.SetAudio(false);
}
```
