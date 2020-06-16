// Loop support for MP3 and OGG in MonoGame
// by Jesuszilla
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Audio;

public partial class Music
{
    /// <summary>
    /// The volume for the BGM track.
    /// </summary>
    [XmlIgnoreAttribute]
    public float Volume
    {
        get { return volume; }
        set
        {
            if (value > 1.0F || value < 0.0F)
                throw new ArgumentOutOfRangeException("volume", value, "Must be in range [0.0, 1.0]");
            else
            {
                volume = value;

                if (bgmInstance != null)
                    bgmInstance.Volume = volume;
            }
        }
    }
    private float volume = 0.5f;

    public float maxVolume = 1;

    /// <summary>
    /// Gets the value indicating whether or not the BGM is currently playing.
    /// </summary>
    [XmlIgnoreAttribute]
    public bool IsPlaying { get; private set; }

    /// <summary>
    /// Currently playing instance.
    /// </summary>
    [XmlIgnoreAttribute]

    private SoundEffectInstance bgmInstance;

    /// <summary>
    /// Loop start time.
    /// </summary>
    [XmlIgnoreAttribute]
    private TimeSpan loopStartTime;

    /// <summary>
    /// Loop time.
    /// </summary>
    [XmlIgnoreAttribute]
    private TimeSpan loopTime;

    /// <summary>
    /// If true, this BGM has loop points.
    /// </summary>
    [XmlIgnoreAttribute]
    public bool isLooped = false;

    [XmlIgnoreAttribute]
    byte[] introBytes;

    [XmlIgnoreAttribute]
    byte[] loopBytes;

    [XmlIgnoreAttribute]
    int sampleRate;

    [XmlIgnoreAttribute]
    AudioChannels channels;

    /// <summary>
    /// Parameterless constructor for XML.
    /// </summary>
    public Music(string filePath)
    {
        Initialize("Content/" + filePath + ".ogg");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Blugen.Sound.BGM"/> class.
    /// </summary>
    /// <param name="filePath">File path to the BGM.</param>
    public void Initialize(string filePath, int loopStart = 0, int loopEnd = 0)
    {
        isLooped = (loopEnd - loopStart) > 0;

        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
            // Not a valid MP3, try Vorbis
            try
            {
                using (NVorbis.VorbisReader vr = new NVorbis.VorbisReader(fs, false))
                {
                    sampleRate = vr.SampleRate;
                    channels = (AudioChannels)vr.Channels;
                    float[] buffer = new float[1024];
                    List<byte> decoded = new List<byte>();
                    while (vr.ReadSamples(buffer, 0, buffer.Length) > 0)
                    {
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            short temp = (short)(32767 * buffer[i]);
                            if (temp > 32767)
                            {
                                decoded.Add(0xFF);
                                decoded.Add(0x7F);
                            }
                            else if (temp < -32768)
                            {
                                decoded.Add(0x80);
                                decoded.Add(0x00);
                            }
                            decoded.Add((byte)temp);
                            decoded.Add((byte)(temp >> 8));
                        }
                    }
                    var db = decoded.ToArray();

                    if (isLooped)
                    {
                        // Regions
                        int loopStartByte = loopStart * (vr.Channels << 1);
                        int loopEndByte = loopEnd * (vr.Channels << 1);
                        int loopStartOffByte = loopStartByte - (loopStartByte % (vr.Channels << 1)); // Align to byte boundary
                        int loopEndOffByte = loopEndByte - (loopEndByte % (vr.Channels << 1)); // Align to byte boundary

                        // Region bytes
                        introBytes = db.Take(loopStartOffByte).ToArray();
                        loopBytes = db.Skip(loopStartOffByte).Take(loopEndOffByte - loopStartOffByte).ToArray();

                        loopStartTime = new TimeSpan((long)((loopStart * 1.0) / vr.SampleRate * 1E7));
                        loopTime = new TimeSpan((long)((loopEnd * 1.0 - loopStart) / vr.SampleRate * 1E7));
                    }
                    else
                        introBytes = db;
                }
            }
            catch (Exception e)
            {   // Let's see if it's an MP3 first.
                /*try
                {
                    fs.Position = 0;
                    using (MP3Stream mp3Dec = new MP3Stream(fs))
                    {
                        sampleRate = mp3Dec.Frequency;
                        channels = (AudioChannels)mp3Dec.ChannelCount;
                        mp3Dec.Position = 0;
                        byte[] buffer = new byte[4096];
                        List<byte> decoded = new List<byte>();
                        int bytesRead = 0;
                        while ((bytesRead = mp3Dec.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            decoded.AddRange(buffer.Take(bytesRead));
                        }
                        var db = decoded.ToArray();

                        if (isLooped)
                        {
                            // Regions
                            int loopStartByte = loopStart * (mp3Dec.ChannelCount << 1);
                            int loopEndByte = loopEnd * (mp3Dec.ChannelCount << 1);
                            int loopStartOffByte = loopStartByte - (loopStartByte % (mp3Dec.ChannelCount << 1)); // Align to byte boundary
                            int loopEndOffByte = loopEndByte - (loopEndByte % (mp3Dec.ChannelCount << 1)); // Align to byte boundary

                            // Region bytes
                            introBytes = db.Take(loopStartOffByte).ToArray();
                            loopBytes = db.Skip(loopStartOffByte).Take(loopEndOffByte - loopStartOffByte).ToArray();

                            loopStartTime = new TimeSpan((long)((loopStart * 1.0) / mp3Dec.Frequency * 1E7));
                            loopTime = new TimeSpan((long)((loopEnd * 1.0 - loopStart) / mp3Dec.Frequency * 1E7));
                        }
                        else
                            introBytes = db;
                    }
                }
                catch (Exception x)
                {

                }*/
            }

            if (introBytes != null)
            {
                bgmInstance = new DynamicSoundEffectInstance(sampleRate, channels);
                // Queue up everything needed for playing
                ((DynamicSoundEffectInstance)bgmInstance).SubmitBuffer(introBytes);
                if (loopBytes != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        ((DynamicSoundEffectInstance)bgmInstance).SubmitBuffer(loopBytes);
                    }
                }
                ((DynamicSoundEffectInstance)bgmInstance).BufferNeeded += BgmInstance_BufferNeeded;
            }
        }
    }

    private void BgmInstance_BufferNeeded(object sender, EventArgs e)
    {
        if (loopBytes != null)
            ((DynamicSoundEffectInstance)bgmInstance).SubmitBuffer(loopBytes);
        else
            ((DynamicSoundEffectInstance)bgmInstance).SubmitBuffer(introBytes);
    }

    /// <summary>
    /// Plays the BGM.
    /// </summary>
    public void Play()
    {
        if (bgmInstance != null)
        {
            bgmInstance.Volume = Volume;
            bgmInstance.Play();

            IsPlaying = true;
        }
    }

    /// <summary>
    /// Pauses the BGM playback.
    /// </summary>
    public void Pause()
    {
        if (bgmInstance != null)
        {
            bgmInstance.Pause();
            IsPlaying = false;
        }
    }

    /// <summary>
    /// Immediately stops the BGM playback and resets its position to the beginning.
    /// </summary>
    public void Stop()
    {
        if (bgmInstance != null)
        {
            bgmInstance.Stop(true);
            
            IsPlaying = false;
        }
    }
}