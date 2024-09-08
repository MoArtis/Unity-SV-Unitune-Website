Title: Use UnituneSource
Order: 2
---

The **UnituneSource** is a convenient wrapper around the main library used by the plugin: **Libopenmpt**.

By using this component, you avoid having to deal with unmanaged resources, the **Libopenmpt** API and the audio clip conversion process.
All of this will be automatically done for you.

As mentioned, the main feature of **Unitune** is generating an **AudioClip** from a module file at runtime, enabling their usage on WebGL.
Consequently, a **UnituneSource** requires an **AudioSource** on the same GameObject and its settings are all related to how the audio clip is generated. 
Here is a quick rundown of these settings:

<?# Figure Src="/img/documentation/create-unitune-asset-imported-module.webp" Class="text-center" /?>

- **Unitune Asset** is referencing the module file that needs to be generated. Without one, no audio clip will be generated. 
- **Create Audio Clip On Enable** defines whether the AudioClip should be generated when the Component is enabled. It is always assumed true if the AudioSource is set to play on Awake. 
- **Use Streaming** defines whether the AudioClip should be streamed or loaded up front into memory. This setting is not available on WebGL.
- **SubSong** allows you to select a specific subsong to be played. The "Every subsong" option just combines all of them into one AudioClip. This setting is disabled if the UnituneAsset's module has only one 1 subsong.
- **Use Output Sample Rate** allows to force the sample rate to be equal to the current audio output device sample rate (usually 48000hz). Enabling this will disable the "Sample Rate" setting.
- **Sample Rate** allows to finely control the generated AudioClip sample rate.
- **Use Output Channels** allows to force the channel count to be equal to the current audio output device channel count (usually stereo). Enabling this will disable the "Channels" setting.
- **Channels** allows to set the output channel count.
- **Use Normalized Time** defines whether to use normalized values (ranging from 0 to 1) for the From and Duration settings.
- **From** sets the starting position of the selected subsong in seconds or normalized time.
- **Duration** sets the duration of selected subsong in seconds or normalized time.
- **Stereo Separation** controls how the audio will be spread between the output channels. The value ranges from 0 (0% - No separation, like in Mono) to 2 (200% - Doubling the module's channel separation). The default and recommended value is 1 (100%).

And finally, if a module asset is set, a preview section allows you to test and hear your settings.

If the **UnituneAsset** setting is set, the **AudioSource** is enabled and set to play on awake then you can play the build. 
Your module file should be rendered into an **AudioClip** and played by the **AudioSource** component.

All These settings are public fields that can be modified from other scripts. 
It's important to note that changes done to these settings will not affect the previously generated **AudioClip**.
In this case, the **AudioClip** needs to be manually generated and played which can be done like so:

```csharp
using SV.Unitune;
using UnityEngine;

public class PlayOnDemand : MonoBehaviour
{
    [SerializeField]
    private UnituneSource unituneSource;

    [SerializeField]
    private UnituneAsset[] unituneAssets;
    
    private void Start()
    {
        // Pick a random UnituneAsset to play.
        var assetIndex = Random.Range(0, unituneAssets.Length);
        unituneSource.unituneAsset = unituneAssets[assetIndex];
        
        // Randomize the starting time of the music
        // and make sure the duration corresponds to the remaining time.
        unituneSource.useNormalizedTime = true;
        unituneSource.from = Random.Range(0f, 0.5f);
        unituneSource.duration = 1f - unituneSource.from;

        unituneSource.Play();
    }
}
```

And that's pretty much it! **Unitune** thrives to be a simple-to-use but powerful plugin.
That being said, if you are interested in more complex use cases involving interacting with **Libopenmpt** more directly, head over to the [Use the low-level API](xref:Use-the-low-level-API) section.
