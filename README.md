# Flappy Bird â€” C# / .NET 10 Windows Forms

**Nesne TabanlÄ± Programlama (Object-Oriented Programming)** dersi **1. Uygulama Ã–devi** kapsamÄ±nda C# ve .NET 10 Windows Forms kullanÄ±larak geliÅŸtirilmiÅŸ, nesne yÃ¶nelimli mimariye sahip **Flappy Bird** masaÃ¼stÃ¼ oyunudur.

Harici hiÃ§bir gÃ¶rsel veya ses dosyasÄ±na baÄŸÄ±mlÄ± olmadan; tÃ¼m grafikler (`GDI+`), parallax arka plan katmanlarÄ±, kuÅŸ kanat animasyonu, madalya Ã§izimleri ve ses efektleri (`RIFF/WAVE PCM`) tamamen kod ile prosedÃ¼rel olarak Ã¼retilmektedir.

---

## Ã–zellikler

- **Nesne TabanlÄ± Mimari (OOP):** Soyut temel sÄ±nÄ±flar (`GameObject`, `ParallaxLayer`), arayÃ¼z tabanlÄ± servis soyutlamalarÄ± (`IHighScoreRepository`, `ISoundService`), kapsÃ¼lleme ve polimorfizm prensiplerine uygun modÃ¼ler yapÄ±.
- **AkÄ±cÄ± Oyun DÃ¶ngÃ¼sÃ¼ & Fizik:** `Stopwatch` tabanlÄ± gerÃ§ek `deltaTime` hesaplamasÄ± ve `DoubleBuffered` (Ã§ift tamponlu) Ã§izim ile titremesiz 60+ FPS hareket, yerÃ§ekimi ivmesi, zÄ±plama fiziÄŸi ve hÄ±za baÄŸlÄ± kuÅŸ rotasyonu.
- **KuÅŸ Kanat Animasyonu:** UÃ§uÅŸ hÄ±zÄ±na gÃ¶re kare hÄ±zÄ± deÄŸiÅŸen 4 aÅŸamalÄ± kanat Ã§Ä±rpma animasyonu ve menÃ¼ ekranÄ±nda sinÃ¼s dalgasÄ±yla sÃ¼zÃ¼lme (`Hover`) hareketi.
- **Parallax Arka Plan:** FarklÄ± hÄ±z Ã§arpanlarÄ±yla (`SpeedFactor`) kayan bulut (`CloudLayer`), ÅŸehir silÃ¼eti (`CityLayer`), Ã§alÄ± (`BushLayer`) ve ÅŸeritli zemin (`Ground`) katmanlarÄ±.
- **Dinamik Zorluk Ã–lÃ§ekleme (`DifficultyScaler`):** Skor arttÄ±kÃ§a borularÄ±n kayma hÄ±zÄ± kademeli olarak artar, borular arasÄ± dikey geÃ§iÅŸ boÅŸluÄŸu daralÄ±r ve sol Ã¼stte mevcut zorluk seviyesi (`Seviye 1-5`) gÃ¶sterilir.
- **Madalya Sistemi (`MedalCalculator`):** Oyun sonunda kazanÄ±lan skora gÃ¶re **Bronz (5+)**, **GÃ¼mÃ¼ÅŸ (15+)**, **AltÄ±n (25+)** ve **Platin (40+)** madalya Ã¶dÃ¼llendirmesi.
- **KalÄ±cÄ± En YÃ¼ksek Skor (`FileHighScoreRepository`):** En yÃ¼ksek skor yerel dosya sisteminde (`LocalAppData\FlappyBirdCSharp\highscore.dat`) saklanÄ±r ve yeni rekor kÄ±rÄ±ldÄ±ÄŸÄ±nda oyun sonu panelinde belirtilir.
- **ProsedÃ¼rel Ses Efektleri (`WaveGenerator` & `SoundManager`):** ZÄ±plama, puan kazanma, Ã§arpma ve madalya kazanma sesleri Ã§alÄ±ÅŸma zamanÄ±nda bellekte 44.1 kHz 16-bit PCM olarak Ã¼retilir ve 4 kanallÄ± polifonik `winmm.dll (waveOut)` motoruyla sÄ±fÄ±r gecikmeli Ã§alÄ±nÄ±r.
- **Durum YÃ¶netimi (`GameState`):** BaÅŸlangÄ±Ã§ MenÃ¼sÃ¼ (`Menu`), Oyun Ä°Ã§i (`Playing`), DuraklatÄ±ldÄ± (`Paused`) ve Oyun Bitti (`GameOver`) ekranlarÄ±.

---

## KullanÄ±lan Teknolojiler

| BileÅŸen | Teknoloji |
|---|---|
| **Programlama Dili** | C# 13 |
| **Ã‡atÄ± (Framework)** | .NET 10.0 (`net10.0-windows`) |
| **ArayÃ¼z & Grafik** | Windows Forms (WinForms) & `System.Drawing` (GDI+ Anti-Aliased Vector Rendering) |
| **Ses Motoru** | `winmm.dll (waveOut)` 4 kanallÄ± polifonik ses motoru + ProsedÃ¼rel 44.1 kHz PCM sentezleyici (`WaveGenerator`) |
| **Veri KalÄ±cÄ±lÄ±ÄŸÄ±** | `System.IO` dosya tabanlÄ± skor saklama (`IHighScoreRepository`) |

---

## Ekran GÃ¶rÃ¼ntÃ¼leri

| BaÅŸlangÄ±Ã§ MenÃ¼sÃ¼ | Oyun Ä°Ã§i (Seviye & Parallax) | Oyun Bitti & Madalya Paneli |
|:---:|:---:|:---:|
| ![BaÅŸlangÄ±Ã§ MenÃ¼sÃ¼](screenshots/menu.png) | ![Oyun Ä°Ã§i](screenshots/gameplay.png) | ![Oyun Bitti](screenshots/gameover.png) |

---

## Nesne TabanlÄ± Mimari ve Proje YapÄ±sÄ±

```text
FlappyBird/
â”œâ”€â”€ FlappyBird.sln
â”œâ”€â”€ README.md
â”œâ”€â”€ .gitignore
â”œâ”€â”€ screenshots/
â”‚   â”œâ”€â”€ menu.png
â”‚   â”œâ”€â”€ gameplay.png
â”‚   â””â”€â”€ gameover.png
â””â”€â”€ FlappyBird/
    â”œâ”€â”€ FlappyBird.csproj
    â”œâ”€â”€ Program.cs                        # Uygulama giriÅŸ noktasÄ±
    â”œâ”€â”€ GameForm.cs                       # Ana pencere, oyun dÃ¶ngÃ¼sÃ¼ ve durum makinesi
    â”œâ”€â”€ Core/
    â”‚   â”œâ”€â”€ GameObject.cs                 # TÃ¼m oyun nesneleri iÃ§in soyut temel sÄ±nÄ±f
    â”‚   â”œâ”€â”€ GameState.cs                  # Oyun durumlarÄ± (Menu, Playing, Paused, GameOver)
    â”‚   â”œâ”€â”€ DifficultyScaler.cs           # Skora baÄŸlÄ± hÄ±z ve boru boÅŸluÄŸu Ã¶lÃ§ekleyici
    â”‚   â””â”€â”€ MedalCalculator.cs            # Madalya hesaplama ve vektÃ¶rel madalya Ã§izimi
    â”œâ”€â”€ Entities/
    â”‚   â”œâ”€â”€ Bird.cs                       # KuÅŸ fiziÄŸi, rotasyon ve kanat Ã§Ä±rpma animasyonu
    â”‚   â”œâ”€â”€ Pipe.cs                       # Ãœst/alt boru Ã§ifti ve Ã§arpÄ±ÅŸma kontrolÃ¼
    â”‚   â”œâ”€â”€ PipeManager.cs                # Boru Ã¼retimi, kaydÄ±rma ve skor sayÄ±mÄ±
    â”‚   â””â”€â”€ Ground.cs                     # Kayan Ã§imen ÅŸeritli zemin nesnesi
    â”œâ”€â”€ Rendering/
    â”‚   â”œâ”€â”€ ParallaxLayer.cs              # Soyut parallax arka plan katmanÄ± (Template Method)
    â”‚   â”œâ”€â”€ BackgroundLayers.cs           # CloudLayer, CityLayer ve BushLayer sÄ±nÄ±flarÄ±
    â”‚   â”œâ”€â”€ Background.cs                 # TÃ¼m arka plan katmanlarÄ±nÄ± yÃ¶neten bileÅŸik sÄ±nÄ±f
    â”‚   â””â”€â”€ OutlinedText.cs               # GraphicsPath tabanlÄ± kenarlÄ±klÄ± metin Ã§izici
    â”œâ”€â”€ Services/
    â”‚   â”œâ”€â”€ IHighScoreRepository.cs       # YÃ¼ksek skor saklama arayÃ¼zÃ¼
    â”‚   â”œâ”€â”€ FileHighScoreRepository.cs    # Dosya tabanlÄ± yÃ¼ksek skor kaydÄ±
    â”‚   â””â”€â”€ ISoundService.cs              # Ses servisi arayÃ¼zÃ¼ ve SoundEffect enum'u
    â””â”€â”€ Audio/
        â”œâ”€â”€ WaveGenerator.cs              # ProsedÃ¼rel 44.1 kHz 16-bit PCM ses dalgasÄ± Ã¼retici
        â””â”€â”€ SoundManager.cs               # 4 kanallÄ± polifonik waveOut ses yÃ¶neticisi
```

---

## Kurulum ve Ã‡alÄ±ÅŸtÄ±rma AdÄ±mlarÄ±

### Gereksinimler
- **.NET 10 SDK** (veya Ã¼zeri)
- Windows iÅŸletim sistemi

### 1. Projeyi KlonlayÄ±n
```bash
git clone https://github.com/Seha83/FlappyBird.git
cd FlappyBird
```

### 2. Projeyi Derleyin
```bash
dotnet build FlappyBird.sln
```

### 3. UygulamayÄ± Ã‡alÄ±ÅŸtÄ±rÄ±n
```bash
dotnet run --project FlappyBird/FlappyBird.csproj
```

---

## Kontroller

| TuÅŸ / Girdi | Ä°ÅŸlev |
|---|---|
| `SPACE` / `W` / `YukarÄ± Ok` / `Sol TÄ±k` | Oyunu baÅŸlat / KuÅŸu zÄ±plat / Oyun sonunda menÃ¼ye dÃ¶n |
| `P` | Oyunu duraklat / devam ettir |
| `M` | Ses efektlerini aÃ§ / kapat (Sessiz mod) |
| `ESC` | Oyundan Ã§Ä±k |
