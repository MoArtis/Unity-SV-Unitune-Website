Order: 5
---

As mentioned before Unitune relies on an Open-Source C/C++ module library called [Libopenmpt](https://lib.openmpt.org/libopenmpt/).

Thus, Unitune includes [C# binding classes](xref:api-SV.Unitune.Libopenmpt.Libopenmpt) to the native library, 
a [module abstraction class](xref:api-SV.Unitune.Libopenmpt.Module) and a [utility class](xref:api-SV.Unitune.Libopenmpt.LibopenmptUtility) to ease the interactions with Libopenmpt.

While interacting with **Libopenmpt** manually is risky (like many other native plugins) due to the unmanaged memory management,
it will give you more control over the generation and usage of the Audio data. 
To illustrate that, here is an example of **Libopenmpt** is used to generate wav files within Unity:

```csharp
using System.IO;
using System.Text;
using SV.Unitune;
using SV.Unitune.Libopenmpt;
using UnityEngine;

public class CreateWavFileFromUnituneAsset : MonoBehaviour
{
    [SerializeField]
    private UnituneAsset unituneAsset;

    [SerializeField]
    private string filename = "test.wav";

    private void Start()
    {
        CreateWavFile();
    }

    private void CreateWavFile()
    {
        if (!LibopenmptUtility.TryCreateModuleFromMemory(unituneAsset.Data, out var mod))
            return;

        var stream = new FileStream(filename, FileMode.Create);
        var writer = new BinaryWriter(stream, Encoding.UTF8, false);

        const int sampleRate = 44100;

        // When using Libopenmpt, it's important to understand the distinction between frame count and sample count.
        // A sample is one value of data.
        // A frame in this context is the samples of every output channel at a specific point in time.
        // In mono Sample and Frame are identical as there is only one channel to work with.
        var frameCount = LibopenmptUtility.GetFrameCount(mod, sampleRate);

        // But since we will read the data in stereo the sample count is the frame count multiplied by 2.
        var sampleCount = frameCount * 2;

        // We prepare a buffer to store the audio data into.
        // Here we make sure that the buffer is big enough to read the entire audio data in one call.
        // But we could totally read the data over multiple frames with a much smaller buffer.
        var samples = new float[sampleCount];

        // Now we can render the audio data and store it into our buffer.
        // There are multiple versions of that "ModuleRead" method. Check out the API for more information.
        Libopenmpt.ModuleReadInterleavedFloatStereo(mod, sampleRate, frameCount, samples);

        // Wav files require a proper header to work.
        WriteWavHeader(writer, sampleRate, 2, 2, sampleCount);

        // We then convert the normalized audio samples ranging from -1 to 1 into the expected shorts (2 bytes).
        foreach (var sample in samples)
            writer.Write((short)(sample * 32767.0f));

        // And done!
        // With the default filename, the wave file should be located outside the project's asset folder.
        writer.Close();
        writer.Dispose();
    }

    private void WriteWavHeader(BinaryWriter writer, int sampleRate, int byteCount, int channelCount, int sampleCount)
    {
        writer.Write("RIFF".ToCharArray());
        writer.Write((uint)44);
        writer.Write("WAVE".ToCharArray());
        writer.Write("fmt ".ToCharArray());
        writer.Write((uint)16);
        writer.Write((ushort)1);
        writer.Write((ushort)channelCount);
        writer.Write((uint)sampleRate);
        writer.Write((uint)(sampleRate * channelCount * byteCount));
        writer.Write((ushort)(channelCount * byteCount));
        writer.Write((ushort)(byteCount == 2 ? 16 : 8));
        writer.Write("data".ToCharArray());
        writer.Write((uint)sampleCount * byteCount);
    }
}
```

Now that you have a better idea of how to work with **Libopenmpt**, we can move on to the **Libopenmpt extension interfaces** which allow much more complex manipulation on the loaded module.
Such as changing the pitch, modifying the tempo, muting an instrument, altering the volume of a specific pattern channel and much more.

Using these **"extension interfaces"** is a bit more complex than using **Libopenmpt** directly. 
So let's go through a simple example to get the point across:

```csharp
using System;
using SV.Unitune;
using SV.Unitune.Libopenmpt;
using UnityEngine;

public class UseExtensionInterface : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private UnituneAsset unituneAsset;

    [SerializeField, Range(0.01f, 4f)]
    private float tempoFactor = 2f;

    private ModuleExtInterfaceInteractive _interfaceInteractive;
    private IntPtr _modExt;

    private void Start()
    {
        // First, we query an extended module to get access to the module extended interfaces.
        // LibopenmptUtility provides a very thin layer of abstraction to avoid common pitfalls.
        // The plugin equivalent of that method is LibopenmptExt.ModuleExtCreateFromMemory().
        // What we obtain is a pointer from the unmanaged side of the plugin which cannot be used directly.
        if (!LibopenmptUtility.TryCreateModuleExtFromMemory(unituneAsset.Data, out _modExt))
            return;

        // With the ModuleExt pointer, we can now query a module interface to manipulate it directly.
        // As the full process is pretty cumbersome, LibopenmptUtility is providing an abstraction for this as well.
        if (!LibopenmptUtility.TryGetModuleExtInterface(_modExt, out _interfaceInteractive))
            return;

        // To avoid having the tempo factor being applied with a delay, we also apply it on start.
        if (tempoFactor is > 0f and <= 4f)
            _interfaceInteractive.SetTempoFactor(_modExt, tempoFactor);
        
        // The last step is to create the AudioClip which requires the frame count derived from duration.
        var moduleName = LibopenmptUtility.GetModuleMetadata(_modExt, ModuleMetadataType.title);
        const int sampleRate = 44100;
        const int channelCount = 2;
        
        // As we are modifying the tempo, the frame count and thus the AudioClip duration will not match.
        // So the song will eventually be clipped or silent depending on the tempo factor used.
        // But it's not very important for this simple demonstration.
        var frameCount = LibopenmptUtility.GetFrameCount(_modExt, sampleRate);

        // To be able to turn on and off the sample channel while the clip is playing, we have to stream the audio clip.
        // Note: AudioClip streaming doesn't work with WebGL builds. Make sure to test this in the Editor or a desktop build.
        var clip = AudioClip.Create(moduleName, frameCount, channelCount, sampleRate, true, PCMReaderCallback);

        // This callback deals with writing into the audio buffer of that clip.
        void PCMReaderCallback(float[] data)
        {
            Libopenmpt.ModuleReadInterleavedFloatStereo(_modExt, sampleRate, data.Length / channelCount, data);
        }

        // Finally, we assign the clip to the audio source and play it.
        audioSource.clip = clip;
        audioSource.Play();
    }

    // In the update loop, we apply the tempo factor using the Module Extension and Extension Interface fields.
    // The effect is not immediately applied due to the audio buffer size,
    // but you should eventually hear the music pattern being played slower or faster.
    private void Update()
    {
        if (_modExt == IntPtr.Zero || tempoFactor is <= 0f or > 4f)
            return;

        _interfaceInteractive.SetTempoFactor(_modExt, tempoFactor);
    }
}
```

Don't hesitate to check out the [API](xref:api-SV.Unitune) and samples included with Unitune for more information.