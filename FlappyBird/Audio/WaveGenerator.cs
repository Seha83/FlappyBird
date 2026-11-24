namespace FlappyBird.Audio;

/// <summary>
/// Harici ses dosyasÄ±na ihtiyaÃ§ duymadan, kod ile 44.1 kHz 16-bit PCM ses Ã¶rnekleri Ã¼retir.
/// Kulak tÄ±rmalayan sert kare dalgalar veya ham beyaz gÃ¼rÃ¼ltÃ¼ yerine yumuÅŸak zarflÄ±
/// (Hann / Ã¼stel sÃ¶nÃ¼mlÃ¼) harmonik sinÃ¼s dalgalarÄ± kullanÄ±r.
/// </summary>
public static class WaveGenerator
{
    public const int SampleRate = 44100;

    /// <summary>
    /// Kanat Ã§Ä±rpma iÃ§in yumuÅŸak Hann zarflÄ± frekans kaydÄ±rmasÄ± ("whoosh/flap" tÄ±nÄ±sÄ±).
    /// </summary>
    public static float[] FlapWhoosh(float startHz = 185f, float endHz = 355f, float seconds = 0.08f, float volume = 0.28f)
    {
        int count = (int)(SampleRate * seconds);
        var samples = new float[count];
        double phase = 0;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / count;
            // YumuÅŸak eÄŸrisel frekans artÄ±ÅŸÄ±
            float curved = t * t;
            float freq = startHz + (endHz - startHz) * curved;
            phase += 2 * Math.PI * freq / SampleRate;

            // Hann penceresi: baÅŸlangÄ±Ã§ ve bitiÅŸte sÄ±fÄ±r tÄ±klama (zero click)
            float envelope = 0.5f * (1f - MathF.Cos(2f * MathF.PI * t));
            float fundamental = (float)Math.Sin(phase);
            float overtone = (float)Math.Sin(phase * 2.0) * 0.18f;

            samples[i] = (fundamental + overtone) * envelope * volume;
        }

        return samples;
    }

    /// <summary>
    /// Puan ve madalya sesleri iÃ§in doÄŸal Ã¼stel sÃ¶nÃ¼mlÃ¼ Ã§an/coin notasÄ± Ã¼retir.
    /// </summary>
    public static float[] ChimeNote(float hz, float seconds, float volume = 0.26f, float decayRate = 5.5f)
    {
        int count = (int)(SampleRate * seconds);
        var samples = new float[count];
        double phase = 0;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / count;
            phase += 2 * Math.PI * hz / SampleRate;

            // 6 ms yumuÅŸak atak + Ã¼stel Ã§an sÃ¶nÃ¼mlenmesi + son %10'da sÄ±fÄ±ra yumuÅŸak iniÅŸ
            float attack = Math.Min(1f, i / (SampleRate * 0.006f));
            float decay = MathF.Exp(-decayRate * t);
            float tail = t > 0.88f ? (1f - t) / 0.12f : 1f;
            float env = attack * decay * tail;

            float fundamental = (float)Math.Sin(phase);
            float harmonic2 = (float)Math.Sin(phase * 2.0) * 0.22f * MathF.Exp(-decayRate * 1.8f * t);
            float harmonic3 = (float)Math.Sin(phase * 3.0) * 0.07f * MathF.Exp(-decayRate * 2.5f * t);

            samples[i] = (fundamental + harmonic2 + harmonic3) * env * volume;
        }

        return samples;
    }

    /// <summary>
    /// Ã‡arpÄ±ÅŸma iÃ§in cÄ±zÄ±rtÄ±sÄ±z, alÃ§ak geÃ§iren filtreli tok darbe ve bas dÃ¼ÅŸÃ¼ÅŸÃ¼.
    /// </summary>
    public static float[] ImpactHit(float seconds = 0.24f, float volume = 0.34f)
    {
        int count = (int)(SampleRate * seconds);
        var samples = new float[count];
        var random = new Random(42);
        double phase = 0;
        float filteredNoise = 0f;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / count;
            // 210 Hz'den 58 Hz'e dÃ¼ÅŸen tok gÃ¶vde tonu
            float freq = 210f * MathF.Pow(58f / 210f, t);
            phase += 2 * Math.PI * freq / SampleRate;

            // Ham gÃ¼rÃ¼ltÃ¼yÃ¼ gÃ¼Ã§lÃ¼ low-pass filtreden geÃ§irerek cÄ±zÄ±rtÄ±yÄ± yok et
            float rawNoise = (float)(random.NextDouble() * 2 - 1);
            filteredNoise = filteredNoise * 0.92f + rawNoise * 0.08f;

            float attack = Math.Min(1f, i / (SampleRate * 0.004f));
            float bodyEnv = MathF.Pow(1f - t, 2.2f) * attack;
            float thumpEnv = MathF.Exp(-14f * t) * attack;

            float body = (float)Math.Sin(phase) * bodyEnv;
            float thump = filteredNoise * thumpEnv * 0.8f;

            samples[i] = (body * 0.75f + thump * 0.25f) * volume;
        }

        return samples;
    }

    /// <summary>Birden fazla ses parÃ§asÄ±nÄ± art arda ekler.</summary>
    public static float[] Concat(params float[][] parts) => parts.SelectMany(p => p).ToArray();

    /// <summary>
    /// Float (-1..+1) Ã¶rneklerini 16-bit mono PCM byte dizisine (ham veri) dÃ¶nÃ¼ÅŸtÃ¼rÃ¼r.
    /// </summary>
    public static byte[] ToPcm16(float[] samples)
    {
        var pcm = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(Math.Clamp(samples[i], -1f, 1f) * (short.MaxValue - 100));
            pcm[i * 2] = (byte)(value & 0xFF);
            pcm[i * 2 + 1] = (byte)((value >> 8) & 0xFF);
        }

        return pcm;
    }
}
