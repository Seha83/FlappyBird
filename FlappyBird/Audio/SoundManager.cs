using System.Runtime.InteropServices;
using FlappyBird.Services;

namespace FlappyBird.Audio;

/// <summary>
/// <see cref="ISoundService"/> arayÃ¼zÃ¼nÃ¼n Windows Multimedia (<c>winmm.dll</c> <c>waveOut</c>) tabanlÄ±,
/// 4 kanallÄ± (polifonik) ve sÄ±fÄ±r gecikmeli uygulamasÄ±.
/// <c>SoundPlayer</c>'Ä±n tek kanal kÄ±sÄ±tlamasÄ±nÄ± ve <c>MemoryStream</c> kesilmelerini ortadan kaldÄ±rÄ±r.
/// </summary>
public sealed class SoundManager : ISoundService, IDisposable
{
    private const int VoiceCount = 4;

    private readonly Dictionary<SoundEffect, byte[]> _pcmBuffers = [];
    private readonly WaveVoice[] _voices;
    private readonly object _lock = new();
    private int _nextVoiceIndex;
    private bool _disposed;

    public SoundManager()
    {
        // 1) YumuÅŸak kanat Ã§Ä±rpma sesi
        Register(SoundEffect.Flap, WaveGenerator.FlapWhoosh());

        // 2) Klasik iki notalÄ± puan Ã§anÄ± (B5 -> E6)
        Register(SoundEffect.Score, WaveGenerator.Concat(
            WaveGenerator.ChimeNote(987.77f, 0.075f, 0.25f, 4.5f),
            WaveGenerator.ChimeNote(1318.51f, 0.22f, 0.27f, 5.5f)));

        // 3) CÄ±zÄ±rtÄ±sÄ±z tok Ã§arpma sesi
        Register(SoundEffect.Hit, WaveGenerator.ImpactHit());

        // 4) Madalya kazanma arpeji (C5 -> E5 -> G5 -> C6)
        Register(SoundEffect.Medal, WaveGenerator.Concat(
            WaveGenerator.ChimeNote(523.25f, 0.08f, 0.24f, 4.0f),
            WaveGenerator.ChimeNote(659.25f, 0.08f, 0.24f, 4.0f),
            WaveGenerator.ChimeNote(783.99f, 0.08f, 0.25f, 4.0f),
            WaveGenerator.ChimeNote(1046.50f, 0.28f, 0.28f, 4.8f)));

        _voices = new WaveVoice[VoiceCount];
        for (int i = 0; i < VoiceCount; i++)
        {
            _voices[i] = new WaveVoice(WaveGenerator.SampleRate);
        }
    }

    public bool IsMuted { get; set; }

    public void Play(SoundEffect effect)
    {
        if (IsMuted || _disposed || !_pcmBuffers.TryGetValue(effect, out byte[]? pcm))
        {
            return;
        }

        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            WaveVoice voice = _voices[_nextVoiceIndex];
            _nextVoiceIndex = (_nextVoiceIndex + 1) % _voices.Length;
            voice.Play(pcm);
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            foreach (WaveVoice voice in _voices)
            {
                voice.Dispose();
            }
        }
    }

    private void Register(SoundEffect effect, float[] samples)
    {
        _pcmBuffers[effect] = WaveGenerator.ToPcm16(samples);
    }

    /// <summary>
    /// Tek bir <c>HWAVEOUT</c> ses kanalÄ±nÄ± ve unmanaged ses tamponunu yÃ¶netir.
    /// </summary>
    private sealed class WaveVoice : IDisposable
    {
        private const int WaveMapper = -1;
        private const ushort WaveFormatPcm = 1;

        private IntPtr _hWaveOut;
        private IntPtr _headerPtr;
        private IntPtr _dataPtr;
        private bool _isPrepared;

        public WaveVoice(int sampleRate)
        {
            var format = new WaveFormatEx
            {
                wFormatTag = WaveFormatPcm,
                nChannels = 1,
                nSamplesPerSec = sampleRate,
                wBitsPerSample = 16,
                nBlockAlign = 2,
                nAvgBytesPerSec = sampleRate * 2,
                cbSize = 0
            };

            if (waveOutOpen(out _hWaveOut, WaveMapper, ref format, IntPtr.Zero, IntPtr.Zero, 0) != 0)
            {
                _hWaveOut = IntPtr.Zero;
            }
        }

        public void Play(byte[] pcmData)
        {
            if (_hWaveOut == IntPtr.Zero)
            {
                return;
            }

            try
            {
                CleanupBuffer();

                _dataPtr = Marshal.AllocHGlobal(pcmData.Length);
                Marshal.Copy(pcmData, 0, _dataPtr, pcmData.Length);

                var header = new WaveHeader
                {
                    lpData = _dataPtr,
                    dwBufferLength = (uint)pcmData.Length,
                    dwFlags = 0,
                    dwLoops = 0
                };

                int headerSize = Marshal.SizeOf<WaveHeader>();
                _headerPtr = Marshal.AllocHGlobal(headerSize);
                Marshal.StructureToPtr(header, _headerPtr, false);

                if (waveOutPrepareHeader(_hWaveOut, _headerPtr, headerSize) == 0)
                {
                    _isPrepared = true;
                    waveOutWrite(_hWaveOut, _headerPtr, headerSize);
                }
            }
            catch
            {
                // Ses donanÄ±mÄ± hatasÄ±nda oyun akÄ±ÅŸÄ± bozulmamalÄ±
            }
        }

        public void Dispose()
        {
            if (_hWaveOut == IntPtr.Zero)
            {
                return;
            }

            CleanupBuffer();
            waveOutClose(_hWaveOut);
            _hWaveOut = IntPtr.Zero;
        }

        private void CleanupBuffer()
        {
            if (_hWaveOut != IntPtr.Zero && _isPrepared)
            {
                waveOutReset(_hWaveOut);
                if (_headerPtr != IntPtr.Zero)
                {
                    waveOutUnprepareHeader(_hWaveOut, _headerPtr, Marshal.SizeOf<WaveHeader>());
                }

                _isPrepared = false;
            }

            if (_headerPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_headerPtr);
                _headerPtr = IntPtr.Zero;
            }

            if (_dataPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_dataPtr);
                _dataPtr = IntPtr.Zero;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WaveFormatEx
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public int nSamplesPerSec;
            public int nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WaveHeader
        {
            public IntPtr lpData;
            public uint dwBufferLength;
            public uint dwBytesRecorded;
            public IntPtr dwUser;
            public uint dwFlags;
            public uint dwLoops;
            public IntPtr lpNext;
            public IntPtr reserved;
        }

        [DllImport("winmm.dll")]
        private static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, ref WaveFormatEx lpFormat,
            IntPtr dwCallback, IntPtr dwInstance, int dwFlags);

        [DllImport("winmm.dll")]
        private static extern int waveOutPrepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

        [DllImport("winmm.dll")]
        private static extern int waveOutWrite(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

        [DllImport("winmm.dll")]
        private static extern int waveOutUnprepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

        [DllImport("winmm.dll")]
        private static extern int waveOutReset(IntPtr hWaveOut);

        [DllImport("winmm.dll")]
        private static extern int waveOutClose(IntPtr hWaveOut);
    }
}
