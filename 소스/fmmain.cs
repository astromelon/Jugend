using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MusicalNote
{
    public partial class FrmMain : Form
    {
        private Scale _scale = null
        private NoteInformation _noteInformation = new NoteInformation();
        private NoteHorizonHistogram[] _noteHorizonHistogram = new NoteHorizonHistogram[0];
        private VerticalHistogram[] _verticalHistogram = new VerticalHistogram[0];
        private SheetInformation[] _sheetInformation = new SheetInformation[0];

        #region PainoKeys Variable
        private Button[] btnBlackPianoKeys = new Button[0];
        private Button[] btnWhitePianoKeys = new Button[0];

        private Size _szBlackPianoKey = new Size(34, 93);
        private Size _szWhitePianoKey = new Size(45, 154);

        private Point[] _ptBlackPianoKey = new Point[] { 
            new Point(30, 0), new Point(74, 0), new Point(162, 0), new Point(206, 0), new Point(250, 0), 
            new Point(338, 0), new Point(382, 0), new Point(470, 0), new Point(514, 0), new Point(558, 0), 
            new Point(646, 0), new Point(690, 0), new Point(778, 0), new Point(822, 0), new Point(866, 0), 
        };
        private Point _ptWhitePianoKey = new Point(0, 0);

        private int[] _nBlackPianoKey = new int[] {
            54, 56, 59, 61, 63,
            66, 68, 71, 73, 75,
            78, 80, 83, 85, 87,
        };
        private int[] _nWhitePainoKey = new int[] {
            53, 55, 57, 58, 60, 62, 64,
            65, 67, 69, 70, 72, 74, 76,
            77, 79, 81, 82, 84, 86, 88,
        };
        #endregion

----------------------------------------------------------------------------------------------------------

        #region Variable
        /// <summary>
        /// 오선에 위치 정보를 가지고 있는다.
        /// </summary>
        private ArrayList _arraySheet = new ArrayList();

        /// <summary>
        /// 악보 이미지
        /// </summary>
        private Bitmap _bitmap = null

        /// <summary>
        /// 수평 Histogram 이미지
        /// </summary>
        private Bitmap _bmphistogram = null

        /// <summary>
        /// 음표를 하나씩 생성한다.
        /// </summary>
        private Bitmap[] _bmpNote = new Bitmap[0];

        /// <summary>
        /// 소절 오선 이미지
        /// </summary>
        private Bitmap[] _bmpSheet = new Bitmap[0];

        /// <summary>
        /// MIDI Handler
        /// </summary>
        private IntPtr _midiHandler = IntPtr.Zero;

        /// <summary>
        /// 현재 연주 음 
        /// </summary>
        private int _nAfterIndex = 0;

        /// <summary>
        /// 현재 연주하는 음에 전 음 높이
        /// </summary>
        private int _nBeforeIndex = 0;

        /// <summary>
        /// 현재 오선
        /// </summary>
        private int _nCurrent = 0;

        /// <summary>
        /// 볼륨크기
        /// </summary>
        private int _nVolume = 80;

        /// <summary>
        /// 선택한 미디 장치
        /// </summary>
        private int _nInstrument = 0;

        /// <summary>
        /// 현재 보여질 악보
        /// </summary>
        private int _nSheetIndex = 0;

        /// <summary>
        /// 악보 총 마디
        /// </summary>
        private int _nTotalBar = 0;

        /// <summary>
        /// 현재 타이머 위치
        /// </summary>
        private int _nTmrCurrent = 0;
        
        /// <summary>
        /// 수평 Histogram 최대값
        /// </summary>
        private int _nHistogramMax = 0;

        /// <summary>
        /// 수평 Histogram 배열 값
        /// </summary>
        private int[] _nHorHistogram = new int[0];

        /// <summary>
        /// 이미지 파일 경로
        /// </summary>
        private string _filename = ""
        #endregion

----------------------------------------------------------------------------------------------------------

        #region Initialize
        /// <summary>
        /// 초기화
        /// </summary>
        private void Initialize()
        {
            tmrPlay.Enabled = false

            _scale = new Scale();
            _noteInformation = new NoteInformation();
            _noteHorizonHistogram = new NoteHorizonHistogram[0];
            _verticalHistogram = new VerticalHistogram[0];
            _sheetInformation = new SheetInformation[0];

            _arraySheet = new ArrayList();

            _nAfterIndex = 0;
            _nBeforeIndex = 0;

            _nCurrent = 0;

            _nHistogramMax = 0;
            _nHorHistogram = new int[0];

            _nVolume = 80;
            _nTotalBar = 0;

            _nTmrCurrent = 0;

            _filename = ""

            InitializeVolumn();
            InitializeMIDI();

            picMusicalNote.Image = null
            tblPlayIndex.Text = string.Format("{0} / {1}", 0, 0);
        }
        #endregion

----------------------------------------------------------------------------------------------------------

        #region InitializeButtonWhitePianoKeys
        private void InitializeButtonWhitePianoKeys()
        {
            btnWhitePianoKeys = new Button[21];

            for (int i = 0; i < btnWhitePianoKeys.Length; i++)
            {
                btnWhitePianoKeys[i] = new Button();
                btnWhitePianoKeys[i].Name = Convert.ToString(btnWhitePianoKeys[i]);
                btnWhitePianoKeys[i].BackColor = Color.White;
                btnWhitePianoKeys[i].Location = _ptWhitePianoKey;
                btnWhitePianoKeys[i].Size = _szWhitePianoKey;
                btnWhitePianoKeys[i].Tag = _nWhitePainoKey[i];

                pnlPianoKeys.Controls.Add(btnWhitePianoKeys[i]);

                _ptWhitePianoKey.X += _szWhitePianoKey.Width - 1;
            }

            this.Width = _ptWhitePianoKey.X + _szWhitePianoKey.Width;
        }
        #endregion

----------------------------------------------------------------------------------------------------------

        #region InitializeButtonBlackPianoKeys
        private void InitializeButtonBlackPianoKeys()
        {
            btnBlackPianoKeys = new Button[15];

            for (int i = 0; i < btnBlackPianoKeys.Length; i++)
            {
                btnBlackPianoKeys[i] = new Button();
                btnBlackPianoKeys[i].Name = Convert.ToString(btnBlackPianoKeys[i]);
                btnBlackPianoKeys[i].BackColor = Color.Black;
                btnBlackPianoKeys[i].Location = _ptBlackPianoKey[i];
                btnBlackPianoKeys[i].Size = _szBlackPianoKey;
                btnBlackPianoKeys[i].Tag = _nBlackPianoKey[i];

                pnlPianoKeys.Controls.Add(btnBlackPianoKeys[i]);
                btnBlackPianoKeys[i].BringToFront();
            }
        }
        #endregion

----------------------------------------------------------------------------------------------------------

        #region InitializeVolumn
        /// <summary>
        /// 볼륨 초기화
        /// </summary>
        private void InitializeVolumn()
        {
            for (int i = 0; i < tlbVolumeList.Items.Count; i++)
            {
                if (tlbVolumeList.Items[i].Equals(Convert.ToString(_nVolume)))
                    tlbVolumeList.SelectedIndex = i;
            }
        }
        #endregion

----------------------------------------------------------------------------------------------------------

        #region FrmMain
        public FrmMain()
        {
            InitializeComponent();

            InitializeButtonWhitePianoKeys();
            InitializeButtonBlackPianoKeys();

            Initialize();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            string[] strNazvyRejstriku = new string[128];
            //Piano:
            strNazvyRejstriku[0] = "Acoustic Grand Piano"
            strNazvyRejstriku[1] = "Bright Acoustic Piano"
            strNazvyRejstriku[2] = "Electric Grand Piano"
            strNazvyRejstriku[3] = "Honky-Tonk Piano"
            strNazvyRejstriku[4] = "Rhodes Piano"
            strNazvyRejstriku[5] = "Chorused Piano"
            strNazvyRejstriku[6] = "Harpsichord"
            strNazvyRejstriku[7] = "Clavinet"
            //Chromaticke bici:
            strNazvyRejstriku[8] = "Celesta"
            strNazvyRejstriku[9] = "Glockenspiel"
            strNazvyRejstriku[10] = "Music Box"
            strNazvyRejstriku[11] = "Vibraphone"
            strNazvyRejstriku[12] = "Marimba"
            strNazvyRejstriku[13] = "Xylophone"
            strNazvyRejstriku[14] = "Tubular Bells"
            strNazvyRejstriku[15] = "Dulcimer"
            // Varhany:
            strNazvyRejstriku[16] = "Hammond Organ"
            strNazvyRejstriku[17] = "Percussive Organ"
            strNazvyRejstriku[18] = "Rock Organ"
            strNazvyRejstriku[19] = "Church Organ"
            strNazvyRejstriku[20] = "Reed Organ"
            strNazvyRejstriku[21] = "Accordian"
            strNazvyRejstriku[22] = "Harmonica"
            strNazvyRejstriku[23] = "Tango Accordian"
            // Kytara:
            strNazvyRejstriku[24] = "Acoustic Guitar (nylon)"
            strNazvyRejstriku[25] = "Acoustic Guitar (steel)"
            strNazvyRejstriku[26] = "Electric Guitar (jazz)"
            strNazvyRejstriku[27] = "Electric Guitar (clean)"
            strNazvyRejstriku[28] = "Electric Guitar (muted)"
            strNazvyRejstriku[29] = "Overdriven Guitar"
            strNazvyRejstriku[30] = "Distortion Guitar"
            strNazvyRejstriku[31] = "Guitar Harmonics"
            // Baskytara:
            strNazvyRejstriku[32] = "Acoustic Bass"
            strNazvyRejstriku[33] = "Electric Bass (finger)"
            strNazvyRejstriku[34] = "Electric Bass (pick)"
            strNazvyRejstriku[35] = "Fretless Bass"
            strNazvyRejstriku[36] = "Slap Bass 1"
            strNazvyRejstriku[37] = "Slap Bass 2"
            strNazvyRejstriku[38] = "Synth Bass 1"
            strNazvyRejstriku[39] = "Synth Bass 2"
            // Smy?ce:
            strNazvyRejstriku[40] = "Violin"
            strNazvyRejstriku[41] = "Viola"
            strNazvyRejstriku[42] = "Cello"
            strNazvyRejstriku[43] = "Contrabass"
            strNazvyRejstriku[44] = "Tremolo Strings"
            strNazvyRejstriku[45] = "Pizzicato Strings"
            strNazvyRejstriku[46] = "Orchestral Harp"
            strNazvyRejstriku[47] = "Timpani"
            // Sbor:
            strNazvyRejstriku[48] = "String Ensemble 1"
            strNazvyRejstriku[49] = "String Ensemble 2"
            strNazvyRejstriku[50] = "Synth Strings 1"
            strNazvyRejstriku[51] = "Synth Strings 2"
            strNazvyRejstriku[52] = "Choir Aahs"
            strNazvyRejstriku[53] = "Voice Oohs"
            strNazvyRejstriku[54] = "Synth Voice"
            strNazvyRejstriku[55] = "Orchestra Hit"
            // ?est?:
            strNazvyRejstriku[56] = "Trumpet"
            strNazvyRejstriku[57] = "Trombone"
            strNazvyRejstriku[58] = "Tuba"
            strNazvyRejstriku[59] = "Muted Trumpet"
            strNazvyRejstriku[60] = "French Horn"
            strNazvyRejstriku[61] = "Brass Section"
            strNazvyRejstriku[62] = "Synth Brass 1"
            strNazvyRejstriku[63] = "Synth Brass 2"
            // Dechove:
            strNazvyRejstriku[64] = "Soprano Sax"
            strNazvyRejstriku[65] = "Alto Sax"
            strNazvyRejstriku[66] = "Tenor Sax"
            strNazvyRejstriku[67] = "Baritone Sax"
            strNazvyRejstriku[68] = "Oboe"
            strNazvyRejstriku[69] = "English Horn"
            strNazvyRejstriku[70] = "Basso0n"
            strNazvyRejstriku[71] = "Clarinet"
            // Pi??aly:
            strNazvyRejstriku[72] = "Piccolo"
            strNazvyRejstriku[73] = "Flute"
            strNazvyRejstriku[74] = "Recorder"
            strNazvyRejstriku[75] = "Pan Flute"
            strNazvyRejstriku[76] = "Bottle Bow"
            strNazvyRejstriku[77] = "Shakuhachi"
            strNazvyRejstriku[78] = "Whistle"
            strNazvyRejstriku[79] = "Ocarina"
            // Synteticke 1:
            strNazvyRejstriku[80] = "Lead 1 (square)"
            strNazvyRejstriku[81] = "Lead 2 (sawtooth)"
            strNazvyRejstriku[82] = "Lead 3 (caliope lead)"
            strNazvyRejstriku[83] = "Lead 4 (chiff lead)"
            strNazvyRejstriku[84] = "Lead 5 (charang)"
            strNazvyRejstriku[85] = "Lead 6 (voice)"
            strNazvyRejstriku[86] = "Lead 7 (fifths)"
            strNazvyRejstriku[87] = "Lead 8 (brass + lead)"
            // Synteticke 2:
            strNazvyRejstriku[88] = "Pad 1 (new age)"
            strNazvyRejstriku[89] = "Pad 2 (warm)"
            strNazvyRejstriku[90] = "Pad 3 (polysynth)"
            strNazvyRejstriku[91] = "Pad 4 (choir)"
            strNazvyRejstriku[92] = "Pad 5 (bowed)"
            strNazvyRejstriku[93] = "Pad 6 (metallic)"
            strNazvyRejstriku[94] = "Pad 7 (halo)"
            strNazvyRejstriku[95] = "Pad 8 (sweep)"
            // Efekty:
            strNazvyRejstriku[96] = "FX 1 (rain)"
            strNazvyRejstriku[97] = "FX 2 (soundtrack)"
            strNazvyRejstriku[98] = "FX 3 (crystal)"
            strNazvyRejstriku[99] = "FX 4 (atmosphere)"
            strNazvyRejstriku[100] = "FX 5 (brightness)"
            strNazvyRejstriku[101] = "FX 6 (goblins)"
            strNazvyRejstriku[102] = "FX 7 (echoes)"
            strNazvyRejstriku[103] = "FX 8 (sci-fi)"
            // R?zne:
            strNazvyRejstriku[104] = "Sitar"
            strNazvyRejstriku[105] = "Banjo"
            strNazvyRejstriku[106] = "Shamisen"
            strNazvyRejstriku[107] = "Koto"
            strNazvyRejstriku[108] = "Kalimba"
            strNazvyRejstriku[109] = "Bagpipe"
            strNazvyRejstriku[110] = "Fiddle"
            strNazvyRejstriku[111] = "Shanai"
            // Bici:
            strNazvyRejstriku[112] = "Tinkle Bell"
            strNazvyRejstriku[113] = "Agogo"
            strNazvyRejstriku[114] = "Steel Drums"
            strNazvyRejstriku[115] = "Woodblock"
            strNazvyRejstriku[116] = "Taiko Drum"
            strNazvyRejstriku[117] = "Melodic Tom"
            strNazvyRejstriku[118] = "Synth Drum"
            strNazvyRejstriku[119] = "Reverse Cymbal"
            // Zvukove efekty:
            strNazvyRejstriku[120] = "Guitar Fret Noise"
            strNazvyRejstriku[121] = "Breath Noise"
            strNazvyRejstriku[122] = "Seashore"
            strNazvyRejstriku[123] = "Bird Tweet"
            strNazvyRejstriku[124] = "Telephone Ring"
            strNazvyRejstriku[125] = "Helicopter"
            strNazvyRejstriku[126] = "Applause"
            strNazvyRejstriku[127] = "Gunshot"

            for (int i = 0; i < strNazvyRejstriku.Length; i++)
                tlbInstrumentList.Items.Add(strNazvyRejstriku[i]);

            tlbInstrumentList.SelectedIndex = 0;
        }
        #endregion

---------------------------------------------------------------------------------------------------------


        private void LoadImage()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "이미지 파일을 선택하세요!"
            openFileDlg.Filter = "이미지 파일|*.bmp;*.jpg;*.gif;*.tif;*.tiff"

            if (openFileDlg.ShowDialog().Equals(DialogResult.OK))
            {
                _filename = openFileDlg.FileName;
                _bitmap = new Bitmap(_filename);
            }
        }

     //   private void Save(Bitmap bitmap, string method)
     //   {
     //       bitmap.Save(string.Format(@"C:\a\{0}.bmp", method));
     //   }

        /// <summary>
        /// 악보 이미지를 이진화 한다.
        /// </summary>
        private void ImageToBinarization()
        {
            for (int y = 0; y < _bitmap.Height; y++)
            {
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    if (_bitmap.GetPixel(x, y).R < 126 && _bitmap.GetPixel(x, y).G < 126 && _bitmap.GetPixel(x, y).B < 126)
                        _bitmap.SetPixel(x, y, Color.Black);
                    else
                        _bitmap.SetPixel(x, y, Color.White);
                }
            }

        //    Save(_bitmap, "ImageToBinarization");
        }

        /// <summary>
        /// 악보 이미지 전체를 수평기준으로 Histogram 한다.
        /// </summary>
        private void ImageToHistogram()
        {
            _nHorHistogram = new int[_bitmap.Height];
            //y축을 기준으로 x축을 돌면서 값 누적하기
            for (int y = 0; y < _bitmap.Height; y++)
            {
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    if (_bitmap.GetPixel(x, y).R == 0 && _bitmap.GetPixel(x, y).G == 0 && _bitmap.GetPixel(x, y).B == 0)
                        _nHorHistogram[y] = _nHorHistogram[y] + 1;
                }
            }
            
            _bmphistogram = new Bitmap(_bitmap.Width, _bitmap.Height);
            for (int y = 0; y < _bitmap.Height; y++)
            {
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    if (x < _nHorHistogram[y])
                        _bmphistogram.SetPixel(x, y, Color.Black);
                    else
                        _bmphistogram.SetPixel(x, y, Color.White);
                }

                // 수평 히스토그램 최대값 저장..
                if (_nHistogramMax < _nHorHistogram[y])
                {
                    _nHistogramMax = _nHorHistogram[y];
                }
            }

        //    Save(_bmphistogram, "ImageToHistogram");
        }

        /// <summary>
        /// Histogram을 참조하여 악보 이미지에 오선위치, 두께, 간격, 소절을 정보를 가지고 온다.
        /// </summary>
        private void ImageToInformation()
        {
            SheetCheck tmp_st = null
            SheetCheck sheetCheck = new SheetCheck();

            bool bStarted = false
            bool bBlack = false

            int doogge = 0;
            int gap = 0;
            int count_line = 0;
            int count_gap = 0;
            int barCount = 0;            
            
            for (int y = 0; y < _bmphistogram.Height; y++)
            {
                //흰 -> 검
                if (!bBlack 
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).R == 0
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).G == 0
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).B == 0)
                {
                    //처음부분이 아니다
                    if (bStarted)
                    {
                        sheetCheck.Gap[count_gap] = gap;
                        gap = 0;
                        count_gap++;
                    }
                    //처음 부분이다
                    if (!bStarted) 
                        bStarted = true

                    bBlack = true
                    sheetCheck.Line[count_line] = y;
                    doogge++;                    
                }

                    // 검 -> 검
                else if (bBlack
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).R == 0
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).G == 0
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).B == 0)
                {
                    doogge++;
                }
                     //검 -> 흰
                else if (bBlack
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).R == 255
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).G == 255
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).B == 255)
                {
                    bBlack = false
                    sheetCheck.Width[count_line] = doogge;
                    doogge = 0;
                    count_line++;
                    if (count_line == 5)
                    {
                        count_line = 0;
                        count_gap = 0;
                        barCount++;
                        bStarted = false
                        _arraySheet.Add(sheetCheck);
                        sheetCheck = new SheetCheck();
                    }
                }

                //흰 -> 흰
                else if (!bBlack && bStarted
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).R == 255
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).G == 255
                    && _bmphistogram.GetPixel((_nHistogramMax * 7) / 10, y).B == 255)
                {
                    gap++;
                }
            }

            for (int i = 0; i < _arraySheet.Count; i++)
            {
                tmp_st = (SheetCheck)_arraySheet[i];

                for (int j = 0; j < tmp_st.Line.Length; j++)
                {
                    if (j == 4) continue
                }
            }

            _nTotalBar = barCount;
        }

        /// <summary>
        /// 각 오선에 첫 번째 / 마지막 줄에 위치를 생성 한다.
        /// </summary>
        private void ImageToSheetPosition()
        {
            _sheetInformation = new SheetInformation[_nTotalBar];

            for (int i = 0; i < _nTotalBar; i++)
            {
                _sheetInformation[i] = new SheetInformation();
            }

            for (int i = 0; i < _nTotalBar; i++)
            {
                _sheetInformation[i].Tops = new Point(_sheetInformation[i].Tops.X, ((SheetCheck)_arraySheet[i]).Line[0] - ((((SheetCheck)_arraySheet[i]).Gap[0]) * 2 + ((SheetCheck)_arraySheet[i]).Width[0]));
                _sheetInformation[i].Downs = new Point(_sheetInformation[i].Downs.X, ((SheetCheck)_arraySheet[i]).Line[4] + ((((SheetCheck)_arraySheet[i]).Gap[3]) * 3 + ((SheetCheck)_arraySheet[i]).Width[4] * 2));
            }
        }

        /// <summary>
        /// 악보 이미지에서 오선을 삭제 한다.
        /// </summary>
        private void ImageToSheetDelete()
        {
            for (int i = 0; i < _sheetInformation.Length; i++)
            {
                for (int y = 0; y < _bitmap.Height; y++)
                {
                    for (int o = 0; o < 5; o++)
                    {
                        if (y == ((SheetCheck)_arraySheet[i]).Line[o])
                        {
                            for (int x = 0; x < _bitmap.Width; x++)
                            {
                                if (_bitmap.GetPixel(x, y).R == 0 && _bitmap.GetPixel(x, y).G == 0 && _bitmap.GetPixel(x, y).B == 0
                                    && _bitmap.GetPixel(x, y - 1).R == 255 && _bitmap.GetPixel(x, y - 1).G == 255 && _bitmap.GetPixel(x, y - 1).B == 255
                                    && _bitmap.GetPixel(x, y + 1).R == 255 && _bitmap.GetPixel(x, y + 1).G == 255 && _bitmap.GetPixel(x, y + 1).B == 255)
                                {
                                    _bitmap.SetPixel(x, y, Color.White);
                                }

                            }
                        }
                    }
                }
            }

         //   Save(_bitmap, "ImageToSheetDelete");
        }

        /// <summary>
        /// 악보 이미지에 각 소절에 Histogram을 생성한다.
        /// </summary>
        private void ImageToBarHistogram()
        {
            for (int i = 0; i < _sheetInformation.Length; i++)
            {
                _sheetInformation[i].VericalHistogram = new int[_bitmap.Width];

                for (int x = 0; x < _bitmap.Width; x++)
                {
                    for (int y = _sheetInformation[i].Tops.Y; y < _sheetInformation[i].Downs.Y; y++)
                    {
                        if (_bitmap.GetPixel(x, y).R == 0 && _bitmap.GetPixel(x, y).G == 0 && _bitmap.GetPixel(x, y).B == 0)
                        {
                            _sheetInformation[i].VericalHistogram[x] += 1;
                        }
                    }
                }
            }            
        }

        /// <summary>
        /// 각 소절을 단위로 생성한다.
        /// </summary>
        private void ImageToBar()
         {
             for (int i = 0; i < _sheetInformation.Length; i++)
             {
                 for (int x = 0; x < _bitmap.Width; x++)
                 {
                     if (_sheetInformation[i].VericalHistogram[x] > 0)
                     {
                         _sheetInformation[i].Tops = new Point(x - 1, _sheetInformation[i].Tops.Y);
                         break
                     }
                 }

                 for (int x = _bitmap.Width - 1; x > 0; x--)
                 {
                     if (_sheetInformation[i].VericalHistogram[x] > 0)
                     {
                         _sheetInformation[i].Downs = new Point(x + 1, _sheetInformation[i].Downs.Y);
                         break
                     }
                 }
             }      
         }
        
        /// <summary>
        /// 마디의 총 음표 개수를 확인한다.
        /// </summary>
        private void ImageToNoteCount()
        {
            bool isBlack = false

            int tempCount = 0;
            int nuzukCount = 0;

            for (int i = 0; i < _sheetInformation.Length; i++)
            {
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    if (isBlack == false && _sheetInformation[i].VericalHistogram[x] > 0)
                    {
                        isBlack = true
                    }
                    else if (isBlack == true && _sheetInformation[i].VericalHistogram[x] <= 0)
                    {
                        tempCount++;
                        nuzukCount++;
                        isBlack = false
                    }
                }

                _noteInformation.TotalCount += tempCount;
                _sheetInformation[i].SheetCount = tempCount;
                _sheetInformation[i].NoteCount = nuzukCount;
                tempCount = 0;
            }
        }

        /// <summary>
        /// 음표 좌우 포인트를 측정 한다.
        /// </summary>
        private void ImageToNotePosition()
        {
            bool bBlack = false
            int tempCount = 0;

            for (int i = 0; i < _sheetInformation.Length; i++)
            {
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    if (!bBlack && _sheetInformation[i].VericalHistogram[x] > 0)
                    {
                        _noteInformation.Tops[tempCount] = new Point(x - 1, _noteInformation.Tops[tempCount].Y);
                        bBlack = true
                    }
                    else if (bBlack && _sheetInformation[i].VericalHistogram[x] <= 0)
                    {
                        _noteInformation.Downs[tempCount] = new Point(x, _noteInformation.Tops[tempCount].Y);
                        bBlack = false

                        tempCount++;
                    }
                }
            }

            Bitmap originBMP = new Bitmap(_filename);
            _bmpSheet = new Bitmap[_sheetInformation.Length];
            for (int i = 0; i < _bmpSheet.Length; i++)
            {
                _bmpSheet[i] = new Bitmap(_sheetInformation[i].Downs.X - _sheetInformation[i].Tops.X, _sheetInformation[i].Downs.Y - _sheetInformation[i].Tops.Y);
                for (int y = _sheetInformation[i].Tops.Y + 1; y < _sheetInformation[i].Downs.Y; y++)
                {
                    for (int x = _sheetInformation[i].Tops.X + 1; x < _sheetInformation[i].Downs.X; x++)
                    {

                        _bmpSheet[i].SetPixel(x - (_sheetInformation[i].Tops.X + 1), y - (_sheetInformation[i].Tops.Y + 1), originBMP.GetPixel(x, y));

                    }
                }

              //  Save(_bmpSheet[i], "ImageToNotePosition" + i.ToString());
            }
        }

       /// <summary>
        /// 음표를 잘라낸다.
        /// </summary>
        private void ImageToNoteCut()
        {
            Bitmap[] bmpNote = new Bitmap[_noteInformation.TotalCount];

            int osun_width = 0;
            int osun_height = 0;
            int osun_count = 0;

            int note_count = 0;

            for (int i = 0; i < bmpNote.Length; i++)
            {
                osun_height = _sheetInformation[osun_count].Downs.Y - _sheetInformation[osun_count].Tops.Y;
                osun_width = _noteInformation.Downs[i].X - _noteInformation.Tops[i].X;                

                bmpNote[i] = new Bitmap(osun_width, osun_height);

                _noteInformation.Tops[i] = new Point(_noteInformation.Tops[i].X, _sheetInformation[osun_count].Tops.Y);
                _noteInformation.Downs[i] = new Point(_noteInformation.Downs[i].X, _sheetInformation[osun_count].Downs.Y);

                for (int y = _sheetInformation[osun_count].Tops.Y; y < _sheetInformation[osun_count].Downs.Y; y++)
                {
                    for (int x = _noteInformation.Tops[i].X + 1; x < _noteInformation.Downs[i].X; x++)
                    {
                        if (_bitmap.GetPixel(x, y).R == 0 &&_bitmap.GetPixel(x, y).B == 0 && _bitmap.GetPixel(x, y).G == 0)
                        {
                            bmpNote[i].SetPixel(x - _noteInformation.Tops[i].X, y - _sheetInformation[osun_count].Tops.Y, Color.Black);
                        }
                        else
                        {
                            bmpNote[i].SetPixel(x - _noteInformation.Tops[i].X, y - _sheetInformation[osun_count].Tops.Y, Color.White);
                        }
                    }
                }

                note_count++;
                if (note_count == _sheetInformation[osun_count].SheetCount)
                {
                    note_count = 0;
                    osun_count++;
                }

            //    Save(bmpNote[i], "ImageToNoteCut" + i.ToString());
            }

            ImageToNoteHistogram(bmpNote);
        }

        /// <summary>
        /// 음표에 Histogram을 작성 한다.
        /// </summary>
        private void ImageToNoteHistogram(Bitmap[] bmpNote)
        {
            _noteHorizonHistogram = new NoteHorizonHistogram[_noteInformation.TotalCount];
            
            for (int i = 0; i < _noteInformation.TotalCount; i++)
            {
                _noteHorizonHistogram[i] = new NoteHorizonHistogram();
                _noteHorizonHistogram[i].TotalHeight = (_noteInformation.Downs[i].Y - 1) - (_noteInformation.Tops[i].Y + 1) + 1;
            }

            for (int i = 0; i < _noteInformation.TotalCount; i++)
            {
                for (int y = 0; y < _noteHorizonHistogram[i].TotalHeight; y++)
                {
                    for (int x = _noteInformation.Tops[i].X + 1; x < _noteInformation.Downs[i].X; x++)
                    {
                        if (bmpNote[i].GetPixel(x - _noteInformation.Tops[i].X, y).R == 0 && bmpNote[i].GetPixel(x - _noteInformation.Tops[i].X, y).G == 0 && bmpNote[i].GetPixel(x - _noteInformation.Tops[i].X, y).B == 0)
                        {
                            _noteHorizonHistogram[i].HorizonHistogrma[y] += 1;
                        }
                    }
                }
            }

            Bitmap[] tmp_h_bmp = new Bitmap[_noteInformation.TotalCount];

            int note_width = 0;
            int note_height = 0;

            for (int i = 0; i < _noteInformation.TotalCount; i++)
            {
                note_height = _noteHorizonHistogram[i].TotalHeight;

                note_width = _noteInformation.Downs[i].X - _noteInformation.Tops[i].X - 1;

                tmp_h_bmp[i] = new Bitmap(note_width, note_height);


                for (int y = 0; y < note_height; y++)
                {
                    for (int x = 0; x < note_width; x++)
                    {
                        if (x < _noteHorizonHistogram[i].HorizonHistogrma[y])
                        {
                            tmp_h_bmp[i].SetPixel(x, y, Color.Black);
                        }
                        else
                        {
                            tmp_h_bmp[i].SetPixel(x, y, Color.White);
                        }
                    }
                }

           //     Save(tmp_h_bmp[i], "ImageToNoteHistogram" + i.ToString());
            }
        }

        /// <summary>
        /// 수평 Histogram에 위,아래를 잘라내기 한다.
        /// </summary>
        private void ImageToHoriHistogramCut()
        {
            bool isStart = false
            bool isBlack = false
            int new_total_height = 0;
            int original_note_LeftTop_Y = 0;


            for (int i = 0; i < _noteInformation.TotalCount; i++)
            {
                new_total_height = 0;
                original_note_LeftTop_Y = _noteInformation.Tops[i].Y;

                if (_noteHorizonHistogram[i].HorizonHistogrma[0] == 0)
                {
                    // 변동사항 없음..
                }
                else if (_noteHorizonHistogram[i].HorizonHistogrma[0] != 0)
                {
                    isStart = true
                    isBlack = true
                    new_total_height++;
                    //noteSet1.note_1_LeftTop[i].Y 는  오선에세 받은값 그대로 쓴다.
                }


                for (int y = 1; y < _noteHorizonHistogram[i].TotalHeight; y++)
                {
                    if (_noteHorizonHistogram[i].HorizonHistogrma[y] == 0 && isBlack == false)
                    {
                        continue       //현재 픽셀이 흰색 && 이전 필셀은 흰색
                    }
                    else if (_noteHorizonHistogram[i].HorizonHistogrma[y] != 0 && isBlack == false && isStart == false)
                    {
                        //현재 픽셀이 검은색 && 이전 필셀은 흰색
                        _noteInformation.Tops[i] = new Point(_noteInformation.Tops[i].X, original_note_LeftTop_Y + y - 1);
                        isBlack = true
                        isStart = true
                        new_total_height++;
                    }
                    else if (_noteHorizonHistogram[i].HorizonHistogrma[y] != 0 && isBlack == true)
                    {
                        //현재 픽셀이 검은색 && 이전 픽셀이 검은색
                        new_total_height++;
                        continue
                    }
                    else if (_noteHorizonHistogram[i].HorizonHistogrma[y] == 0 && isBlack == true)
                    {
                        //현재 픽셀은 흰색 && 이전 픽셀은 검은색
                        _noteInformation.Downs[i] = new Point(_noteInformation.Downs[i].X, original_note_LeftTop_Y + y);
                        isBlack = false
                        isStart = false
                        _noteHorizonHistogram[i].TotalHeight = new_total_height;
                        break
                    }
                }
            }
        }

        /// <summary>
        /// 수평 Histogram에서 잘래내기한 이미지를 만든다.
        /// </summary>
        private void ImageToBitamp()
        {
            _bmpNote = new Bitmap[_noteInformation.TotalCount];

            int osun_width = 0;
            int osun_height = 0;

            for (int i = 0; i < _bmpNote.Length; i++)
            {
                osun_height = _noteInformation.Downs[i].Y - _noteInformation.Tops[i].Y - 1;
                osun_width = _noteInformation.Downs[i].X - _noteInformation.Tops[i].X - 1;


                _bmpNote[i] = new Bitmap(osun_width, osun_height);

                for (int y = _noteInformation.Tops[i].Y + 1; y < _noteInformation.Downs[i].Y; y++)
                {
                    for (int x = _noteInformation.Tops[i].X + 1; x < _noteInformation.Downs[i].X; x++)
                    {
                        if (_bitmap.GetPixel(x, y).R == 0 && _bitmap.GetPixel(x, y).B == 0 && _bitmap.GetPixel(x, y).G == 0)
                        {
                            _bmpNote[i].SetPixel(x - (_noteInformation.Tops[i].X + 1), y - (_noteInformation.Tops[i].Y + 1), Color.Black);
                        }
                        else
                        {
                            _bmpNote[i].SetPixel(x - (_noteInformation.Tops[i].X + 1), y - (_noteInformation.Tops[i].Y + 1), Color.White);
                        }
                    }
                }

             //   Save(_bmpNote[i], "ImageToBitamp" + i.ToString());
            }
        }



        //1차 음표들의 수직 히스토그램 표 만들기.

        /// <summary>
        /// 음표을 이용하여 수직 Histogram을 생성한다.
        /// </summary>
        private void ImageToCreateNoteHistogram()
        {
            _verticalHistogram = new VerticalHistogram[_noteInformation.TotalCount];

            for (int i = 0; i < _verticalHistogram.Length; i++)
            {
                _verticalHistogram[i] = new VerticalHistogram();
                _verticalHistogram[i].TotalWidth = _bmpNote[i].Width;

                for (int x = 0; x < _bmpNote[i].Width; x++)
                {
                    for (int y = 0; y < _bmpNote[i].Height; y++)
                    {
                        if(_bmpNote[i].GetPixel(x, y).R == 0 && _bmpNote[i].GetPixel(x, y).G == 0 && _bmpNote[i].GetPixel(x, y).B == 0)
                        {
                            _verticalHistogram[i].VeriticalHistogram[x]++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 음계를 분리 한다.
        /// </summary>
        private void ImageToScale()
        {
            int note_height = 0;
            int gap = 0;

            _scale.ScaleCount = _noteInformation.TotalCount;

            for (int i = 0; i < _scale.ScaleCount; i++)
            {
                note_height = _noteInformation.Downs[i].Y - 3;
                gap = ((SheetCheck)_arraySheet[0]).Gap[0];
                for (int osunC = 0; osunC < _sheetInformation.Length; osunC++)
                {
                    SheetCheck sheetCheck = ((SheetCheck)_arraySheet[osunC]);
                    if (_sheetInformation[osunC].Tops.Y <= note_height && note_height <= _sheetInformation[osunC].Downs.Y)
                    {
                        if (sheetCheck.Line[1] + 1 < note_height && note_height <= sheetCheck.Line[2] - 2)
                        {
                            _scale.ScaleHeight[i] = 77;
                        }
                        else if (sheetCheck.Line[2] - 2 < note_height && note_height < sheetCheck.Line[2] + 2)
                        {
                            _scale.ScaleHeight[i] = 76;
                        }
                        else if (sheetCheck.Line[2] + 1 < note_height && note_height <= sheetCheck.Line[3] - 2)
                        {
                            _scale.ScaleHeight[i] = 74;
                        }
                        else if (sheetCheck.Line[3] - 2 < note_height && note_height <= sheetCheck.Line[3] + 2)
                        {
                            _scale.ScaleHeight[i] = 72;
                        }
                        else if (sheetCheck.Line[3] + 1 < note_height && note_height <= sheetCheck.Line[4] - 2)
                        {
                            _scale.ScaleHeight[i] = 70;
                        }
                        else if (sheetCheck.Line[4] - 2 < note_height && note_height <= sheetCheck.Line[4] + 1)
                        {
                            _scale.ScaleHeight[i] = 69;
                        }
                        else if (sheetCheck.Line[4] + 2 <= note_height && note_height <= sheetCheck.Line[4] + sheetCheck.Gap[3])
                        {
                            _scale.ScaleHeight[i] = 67;
                        }
                        else if (sheetCheck.Line[4] + sheetCheck.Gap[3] - 2 <= note_height && note_height <= sheetCheck.Line[4] + sheetCheck.Gap[3] * 2 + 1)
                        {
                            _scale.ScaleHeight[i] = 65;
                        }
                        else
                        {
                            _scale.ScaleHeight[i] = 61;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 박자를 확인 한다.
        /// </summary>
        private void ImageToBeat()
        {
            int length;

            int max_value; // 배열 원소중 최대값 저장
            int max_index; // 배열 원소중 최대값 가진 인덱스
            int osunC = 0;

            bool isBlack = false
            int x = 0;
            int tailCount = 0;



            SheetCheck sheetCheck = ((SheetCheck)_arraySheet[0]);

            for (int i = 0; i < _verticalHistogram.Length; i++)
            {
                if (i == _sheetInformation[osunC].NoteCount)
                {
                    osunC++;
                    sheetCheck = ((SheetCheck)_arraySheet[osunC]);
                }

                length = _verticalHistogram[i].VeriticalHistogram.Length;

                max_value = 0;
                max_index = 0;

                ///
                /// 음표 하나가 가지고있는 배열원소중 최대값 찾기
                /// 
                for (int index = 0; index < _verticalHistogram[i].VeriticalHistogram.Length; index++)
                {
                    if (max_value < _verticalHistogram[i].VeriticalHistogram[index])
                    {
                        max_value = _verticalHistogram[i].VeriticalHistogram[index];
                        max_index = index;
                    }
                }

                if (max_index == _verticalHistogram[i].VeriticalHistogram.Length - 1)
                {
                    if (max_index == 0)
                        _scale.ScaleKind[i] = 0;
                    else
                    {
                        if (_verticalHistogram[i].VeriticalHistogram[max_index / 2] <= (sheetCheck.Gap[0] + 1) - 1)
                        {
                            _scale.ScaleKind[i] = 1;
                            _scale.ScaleLength[i] = 35;
                        }
                        else
                        {
                            _scale.ScaleKind[i] = 1;
                            _scale.ScaleLength[i] = 25;
                        }
                    }
                }
                else
                {
                    if (_verticalHistogram[i].VeriticalHistogram.Length < 5)
                    {
                        if (_verticalHistogram[i].VeriticalHistogram[0] > 25)
                        {
                            _scale.ScaleKind[i] = 0;
                            break
                        }

                        else
                        {
                            int helf;
                            _scale.ScaleKind[i] = 3;
                            helf = _scale.ScaleLength[i - 1] / 2;
                            _scale.ScaleLength[i - 1] = _scale.ScaleLength[i - 1] + helf;
                            _scale.ScaleKind[i] = 0;
                        }// 점
                    }

                    else
                    {
                        if (_verticalHistogram[i].VeriticalHistogram[0] > 5)
                        {
                            if (_verticalHistogram[i].VeriticalHistogram[0] > 20)
                            {
                                if (_verticalHistogram[i].VeriticalHistogram[0] > 25)
                                {
                                    if (_verticalHistogram[i].VeriticalHistogram[1] > 25)
                                    {
                                        _scale.ScaleKind[i] = 0;
                                        break
                                    }
                                    else
                                    {
                                        _scale.ScaleKind[i] = 1;
                                        _scale.ScaleHeight[i] = 25;
                                    }
                                }
                                else
                                {
                                    _scale.ScaleKind[i] = 0;
                                }
                            }
                            else
                            {

                                if (_verticalHistogram[i].VeriticalHistogram[3] > 18)
                                {
                                    _scale.ScaleKind[i] = 2;
                                    _scale.ScaleLength[i] = 25;
                                }
                                else
                                {
                                    _scale.ScaleKind[i] = 0;
                                }
                            }
                        }

                        else
                        {
                            if (_verticalHistogram[i].VeriticalHistogram[5] <= 7)
                            {
                                if (_verticalHistogram[i].VeriticalHistogram[2] > 11)
                                {
                                    _scale.ScaleKind[i] = 2;
                                    _scale.ScaleLength[i] = 25;
                                }
                                else
                                {
                                    if(max_index == _verticalHistogram[i].VeriticalHistogram.Length - 1 ||
                                        (_verticalHistogram[i].VeriticalHistogram[0] == 1 && _verticalHistogram[i].VeriticalHistogram[1] == 1 &&
                                        _verticalHistogram[i].VeriticalHistogram[2] == 1))
                                    {
                                        x = max_index + 1;
                                        tailCount = 0;

                                        if(_bmpNote[i].GetPixel(x, 0).R == 0 && _bmpNote[i].GetPixel(x, 0).G == 0 && _bmpNote[i].GetPixel(x, 0).B == 0)
                                        { isBlack = true }
                                        else
                                        { isBlack = false }

                                        for (int y = 1; y < _bmpNote[i].Height / 2; y++)
                                        {
                                            //// 검 -> 검
                                            if (isBlack == true && _bmpNote[i].GetPixel(x, y).R == 0 && _bmpNote[i].GetPixel(x, y).G == 0 && _bmpNote[i].GetPixel(x, y).B == 0)
                                            { continue }

                                            //// 검 -> 흰
                                            else if (isBlack == true && _bmpNote[i].GetPixel(x, y).R == 255 && _bmpNote[i].GetPixel(x, y).G == 255 && _bmpNote[i].GetPixel(x, y).B == 255)
                                            { isBlack = false tailCount++; }

                                            //// 흰 -> 검
                                            else if (isBlack == false && _bmpNote[i].GetPixel(x, y).R == 0 && _bmpNote[i].GetPixel(x, y).G == 0 && _bmpNote[i].GetPixel(x, y).B == 0)
                                            { isBlack = true }

                                            //// 흰 -> 흰
                                            else if (isBlack == false &&_bmpNote[i].GetPixel(x, y).R == 255 && _bmpNote[i].GetPixel(x, y).G == 255 && _bmpNote[i].GetPixel(x, y).B == 255)
                                            { continue }

                                        }

                                        if (tailCount == 1)
                                        {
                                            _scale.ScaleKind[i] = 1;
                                            _scale.ScaleLength[i] = 15;
                                        }
                                        else if (tailCount == 2)
                                        {
                                            _scale.ScaleKind[i] = 1;
                                            _scale.ScaleLength[i] = 10;
                                        }
                                        else
                                        {
                                            if (_verticalHistogram[i].VeriticalHistogram[max_index / 2] <= (sheetCheck.Gap[0] + 1) - 1)
                                            {
                                                _scale.ScaleKind[i] = 1;
                                                _scale.ScaleLength[i] = 35;
                                            }// 2분 음표
                                            else
                                            {
                                                _scale.ScaleKind[i] = 1;
                                                _scale.ScaleLength[i] = 25;
                                            }// 4분 음표
                                        }
                                    }
                                    else
                                    {
                                        _scale.ScaleKind[i] = 2;
                                        _scale.ScaleLength[i] = 15;
                                    }
                                }
                            }

                            else
                            {
                                if ((_verticalHistogram[i].VeriticalHistogram[0] == 4 &&
                                    _verticalHistogram[i].VeriticalHistogram[1] == 8) ||
                                    _verticalHistogram[i].VeriticalHistogram[0] == 2)
                                {
                                    _scale.ScaleKind[i] = 0;
                                }
                                else
                                {
                                    x = max_index + 1;
                                    tailCount = 0;

                                    if(_bmpNote[i].GetPixel(x, 0).R == 0 && _bmpNote[i].GetPixel(x, 0).G == 0 && _bmpNote[i].GetPixel(x, 0).B == 0)
                                    { isBlack = true }
                                    else
                                    { isBlack = false }

                                    for (int y = 1; y < _bmpNote[i].Height / 2; y++)
                                    {
                                        //// 검 -> 검
                                        if (isBlack == true && _bmpNote[i].GetPixel(x, y).R == 0 && _bmpNote[i].GetPixel(x, y).G == 0 && _bmpNote[i].GetPixel(x, y).B == 0)
                                        { continue }

                                        //// 검 -> 흰
                                        else if (isBlack == true && _bmpNote[i].GetPixel(x, y).R == 255 && _bmpNote[i].GetPixel(x, y).G == 255 && _bmpNote[i].GetPixel(x, y).B == 255)
                                        { isBlack = false tailCount++; }

                                        //// 흰 -> 검
                                        else if (isBlack == false && _bmpNote[i].GetPixel(x, y).R == 0 && _bmpNote[i].GetPixel(x, y).G == 0 && _bmpNote[i].GetPixel(x, y).B == 0)
                                        { isBlack = true }

                                        //// 흰 -> 흰
                                        else if (isBlack == false && _bmpNote[i].GetPixel(x, y).R == 255 && _bmpNote[i].GetPixel(x, y).G == 255 && _bmpNote[i].GetPixel(x, y).B == 255)
                                        { continue }

                                    }

                                    if (tailCount == 1)
                                    {
                                        _scale.ScaleKind[i] = 1;
                                        _scale.ScaleLength[i] = 15;
                                    }
                                    else if (tailCount == 2)
                                    {
                                        _scale.ScaleKind[i] = 1;
                                        _scale.ScaleLength[i] = 10;
                                    }
                                    else
                                    {
                                        _scale.ScaleKind[i] = 0;
                                        _scale.ScaleLength[i] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #region SheetView
        /// <summary>
        /// 현재 소절 보여주기
        /// </summary>
        private void SheetPreView()
        {
            picMusicalNote.Image = _bmpSheet[_nSheetIndex];
            tblPlayIndex.Text = string.Format("{0} / {1}", _nSheetIndex + 1, _nTotalBar);
        }

        /// <summary>
        /// 이전 소절 듣기
        /// </summary>
        private void BackPlay()
        {
            if (_nSheetIndex - 1 >= 0 && _nSheetIndex - 1 < _sheetInformation.Length)
            {
                _nSheetIndex--;
                _nCurrent = _nSheetIndex;
                btnWhitePianoKeys[_nBeforeIndex].BackColor = Color.White;
                _nBeforeIndex = 0;
                _nTmrCurrent = 0;
                if (_nSheetIndex == 0)
                {
                    _nAfterIndex = 0;
                }
                else if (_nSheetIndex >= 1)
                {
                    _nAfterIndex = _sheetInformation[_nSheetIndex - 1].NoteCount;
                }
            }
            else
            {
                MessageBox.Show("악보 첫 소절입니다.");
            }

            picMusicalNote.Image = _bmpSheet[_nSheetIndex];
            tblPlayIndex.Text = string.Format("{0} / {1}", Convert.ToString(_nSheetIndex + 1), _nTotalBar);
        }

        /// <summary>
        /// 다음 소절 듣기
        /// </summary>
        private void NextPlay()
        {
            if (_nSheetIndex + 1 >= 0 && _nSheetIndex + 1 < _sheetInformation.Length)
            {
                _nSheetIndex++;
                _nCurrent = _nSheetIndex;
                btnWhitePianoKeys[_nBeforeIndex].BackColor = Color.White;
                _nBeforeIndex = 0;
                _nTmrCurrent = 0;
                _nAfterIndex = _sheetInformation[_nSheetIndex - 1].NoteCount;
            }
            else
            {
                MessageBox.Show("악보 마지막 소절입니다.");
            }
            picMusicalNote.Image = _bmpSheet[_nSheetIndex];
            tblPlayIndex.Text = string.Format("{0} / {1}", Convert.ToString(_nSheetIndex + 1), _nTotalBar);
        }
        #endregion

---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 악보분석
        /// </summary>
        private void Analysis()
        {
            //이진화및 히스토그램생성
            ImageToBinarization(); 
            ImageToHistogram();

            //Histogram을 참조하여 악보 이미지에 오선위치, 두께, 간격, 소절을 정보를 가지고 온다.
            ImageToInformation();

            //각 오선에 첫 줄 / 마지막 줄을 위치로 소절에 크기를 생성 한다.
            ImageToSheetPosition();

            //오선 삭제
            ImageToSheetDelete();

            //각 소절에 히스토그램 정보를 생성한다.
            ImageToBarHistogram();

            //소절을 단위로 생성
            ImageToBar();

            //음표 개수 파악
            ImageToNoteCount();

            //각 음표 위치 측정
            ImageToNotePosition();

            //각 음표를 잘라낸다.
            ImageToNoteCut();

            //수평 histogram으로 음표에 위, 아래를 잘라낸다.
            ImageToHoriHistogramCut();

            //잘라내기 한 후 이미지를 생성 한다.
            ImageToBitamp();

            //음표를 이용하여 수직 histogram을 한다.
            ImageToCreateNoteHistogram();

            //histogram을 이용하여 음계를 분리 한다.
            ImageToScale();

            //박자를 생성한다.
            ImageToBeat();

            //첫 번째 소절 보여주기
            SheetPreView();
        }

        private void MusicalNotePlay()
        {
            tmrPlay.Enabled = true
        }

        private void MusicalNoteStop()
        {
            tmrPlay.Enabled = false
            _nAfterIndex = 0;
            _nTmrCurrent = 0;

            btnWhitePianoKeys[_nBeforeIndex].BackColor = Color.White;

            _nBeforeIndex = 0;
            
            InitializeMIDI();
            SheetPreView();
        }

        private void MusicalNotePause()
        {
            if (tmrPlay.Enabled == false)
            {
                tmrPlay.Enabled = true
                MidiNoteOn(this._midiHandler, _scale.ScaleHeight[_nAfterIndex], _nVolume, 0);
            }
            else
            {
                tmrPlay.Enabled = false
                
                InitializeMIDI();
            }
        }

        #region MIDI
        private void InitializeMIDI()
        {
            CloseMIDI();

            if (WIN32.midiOutOpen(ref _midiHandler, 0, IntPtr.Zero, IntPtr.Zero, 0) != 0)
            {
                MessageBox.Show("장비오류", Text);
            }
            PlayMidi(0xC0, _nInstrument, 0, 0);
        }

        private void CloseMIDI()
        {
            WIN32.midiOutClose(_midiHandler);
        }

        private IntPtr PlayMidi(int stavovyBajt, int prvniDatovyBajt, int druhyDatovyBajt, int kanal)
        {

            int dwZprava = stavovyBajt | kanal | (prvniDatovyBajt << 8) | (druhyDatovyBajt << 16);
            return WIN32.midiOutShortMsg(_midiHandler, dwZprava);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            MusicalNotePlay();
        }

        private IntPtr MidiNoteOn(IntPtr handleMIDIOut, int MIDIVyska, int rychlostStisku, int kanal)
        {
            return PlayMidi(0x090, MIDIVyska, rychlostStisku, kanal);
        }

        private IntPtr MidiNoteOff(IntPtr handleMIDIOut, int MIDIVyska, int rychlostUvolneni, int kanal)
        {
            return PlayMidi(0x080, MIDIVyska, rychlostUvolneni, kanal);
        }
        #endregion

---------------------------------------------------------------------------------------------------------

        #region Menubar
        /// <summary>
        /// 이미지 불러오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuLoadMusicalNote_Click(object sender, EventArgs e)
        {
            Initialize();
            LoadImage();
        }

        /// <summary>
        /// 악보분석
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuAnalysisMusicalNote_Click(object sender, EventArgs e)
        {
            Analysis();
        }

        /// <summary>
        /// 악보 - 재생
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuPlay_Click(object sender, EventArgs e)
        {
            MusicalNotePlay();
        }

        /// <summary>
        /// 악보 - 일시정지
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuPause_Click(object sender, EventArgs e)
        {
            MusicalNotePause();
        }

        /// <summary>
        /// 악보 - 정지
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuStop_Click(object sender, EventArgs e)
        {
            MusicalNoteStop();
        }

        /// <summary>
        /// 프로그램 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

---------------------------------------------------------------------------------------------------------

        #region toolbar
        private void tlbInstrumentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _nInstrument = tlbInstrumentList.SelectedIndex;
            
            InitializeMIDI(); 
        }

        private void tlbVolumeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _nVolume = Convert.ToInt32(tlbVolumeList.SelectedItem);
        }

        private void tlbBackPlay_Click(object sender, EventArgs e)
        {
            BackPlay();
        }

        private void tlbNextPlay_Click(object sender, EventArgs e)
        {
            NextPlay();
        }
        #endregion

----------------------------------------------------------------------------------------------------------

        #region Timer
        private void tmrPlay_Tick(object sender, EventArgs e)
        {
            if (_nAfterIndex == _scale.ScaleCount)
            {
                tmrPlay.Enabled = false
                _nCurrent = 0;
                _nBeforeIndex = 0;
                _nAfterIndex = 0;
                _nTmrCurrent = 0;

                _nSheetIndex = 0;
                SheetPreView();
            }
            else if (_nAfterIndex == _sheetInformation[_nCurrent].NoteCount && _nCurrent + 1 < _sheetInformation.Length)
            {
                _nSheetIndex = _nCurrent + 1;
                SheetPreView();
            }


            if ((_nAfterIndex != _scale.ScaleCount) && _scale.ScaleLength[_nAfterIndex] == _nTmrCurrent)
            {
                MidiNoteOff(this._midiHandler, _scale.ScaleHeight[_nAfterIndex], _nVolume, 0);
                btnWhitePianoKeys[_nBeforeIndex].BackColor = Color.White;
                _nAfterIndex++;
                _nTmrCurrent = 0;
                return
            }
            else if ((_nAfterIndex != _scale.ScaleCount) && _scale.ScaleKind[_nAfterIndex] == 0)
            {
                _nAfterIndex++;
            }
            else if ((_nAfterIndex != _scale.ScaleCount) &&_scale.ScaleKind[_nAfterIndex] == 1 && _nTmrCurrent == 0)
            {
                MidiNoteOn(this._midiHandler, _scale.ScaleHeight[_nAfterIndex], _nVolume, 0);

                for (int i = 0; i < btnWhitePianoKeys.Length; i++)
                {
                    int ntag = Convert.ToInt32(btnWhitePianoKeys[i].Tag);

                    if (ntag.Equals(_scale.ScaleHeight[_nAfterIndex]))
                    {
                        _nBeforeIndex = i;
                        btnWhitePianoKeys[i].BackColor = Color.Orange;
                        break
                    }
                }
            }
            else if ((_nAfterIndex != _scale.ScaleCount) && _scale.ScaleKind[_nAfterIndex] == 2 && _nTmrCurrent == 0)
            {
                MidiNoteOff(this._midiHandler, _scale.ScaleHeight[_nAfterIndex], _nVolume, 0);
                btnWhitePianoKeys[_nBeforeIndex].BackColor = Color.White;
            }

            _nTmrCurrent++;

            if (tmrPlay.Enabled == false)
            {
                _nAfterIndex = 0;
                _nTmrCurrent = 0;
            }
        }
        #endregion

    }//FrmMain class

----------------------------------------------------------------------------------------------------------

    #region VerticalHistogram
    public class VerticalHistogram
    {
        private int _nTotalWidth = 0;
        private int[] _nVerticalHistogram = new int[0];

        public VerticalHistogram() 
        {
            TotalWidth = 0;
        }

        public int TotalWidth
        {
            get
            {
                return _nTotalWidth;
            }
            set
            {
                _nTotalWidth = value

                VeriticalHistogram = new int[TotalWidth];
            }
        }

        public int[] VeriticalHistogram
        {
            get
            {
                return _nVerticalHistogram;
            }
            set
            {
                _nVerticalHistogram = value
            }
        }
    }
    #endregion

----------------------------------------------------------------------------------------------------------

    #region NoteHorizonHistogram
    public class NoteHorizonHistogram
    {
        private int _nTotalHeight = 0;
        private int[] _nHorizonHistogram = new int[0];

        public NoteHorizonHistogram() 
        {
            TotalHeight = 0;
        }

        public int TotalHeight
        {
            get
            {
                return _nTotalHeight;
            }
            set
            {
                _nTotalHeight = value

                HorizonHistogrma = new int[TotalHeight];
            }
        }

        public int[] HorizonHistogrma
        {
            get
            {
                return _nHorizonHistogram;
            }
            set
            {
                _nHorizonHistogram = value
            }
        }
    }
    #endregion

----------------------------------------------------------------------------------------------------------

    #region SheetInformation
    public class SheetInformation
    {
        private Point _ptTop = new Point();
        private Point _ptDown = new Point();

        private int _noteCount = 0;
        private int _sheetCount = 0;
        private int[] _nVeritcalHistogram = new int[0];

        public Point Tops
        {
            get
            {
                return _ptTop;
            }
            set
            {
                _ptTop = value
            }
        }

        public Point Downs
        {
            get
            {
                return _ptDown;
            }
            set
            {
                _ptDown = value
            }
        }

        public int NoteCount
        {
            get
            {
                return _noteCount;
            }
            set
            {
                _noteCount = value
            }
        }

        public int SheetCount
        {
            get
            {
                return _sheetCount;
            }
            set
            {
                _sheetCount = value
            }
        }

        public int[] VericalHistogram
        {
            get
            {
                return _nVeritcalHistogram;
            }
            set
            {
                _nVeritcalHistogram = value
            }
        }
        

    }
    #endregion

----------------------------------------------------------------------------------------------------------

    #region SheetCheck
    public class SheetCheck
    {
        private int[] _nGap = new int[4];
        private int[] _nLine = new int[5];
        private int[] _nWidth = new int[5];

        public SheetCheck()
        {
            Gap = new int[4];
            Line = new int[5];
            Width = new int[5];
        }

        public int[] Gap
        {
            get
            {
                return _nGap;
            }
            set
            {
                _nGap = value
            }
        }

        public int[] Line
        {
            get
            {
                return _nLine;
            }
            set
            {
                _nLine = value
            }
        }

        public int[] Width
        {
            get
            {
                return _nWidth;
            }
            set
            {
                _nWidth = value
            }
        }        
    }
    #endregion

----------------------------------------------------------------------------------------------------------

    #region NoteInformation
    /// <summary>
    /// 음표 정보를 저장 한다. (음표개수, 위치(사각형)등 저장)
    /// </summary>
    public class NoteInformation
    {
        private int _nTotalCount = 0;
        private Point[] _ptTop = new Point[0];
        private Point[] _ptDown = new Point[0];
        
        public NoteInformation() 
        {
            TotalCount = 0;
        }

        public int TotalCount
        {
            get
            {
                return _nTotalCount;
            }
            set
            {
                _nTotalCount = value

                Tops = new Point[TotalCount];
                Downs = new Point[TotalCount];
            }
        }

        public Point[] Tops
        {
            get
            {
                return _ptTop;
            }
            set
            {
                _ptTop = value
            }
        }

        public Point[] Downs
        {
            get
            {
                return _ptDown;
            }
            set
            {
                _ptDown = value
            }
        }
    }
    #endregion

----------------------------------------------------------------------------------------------------------

    #region Scale
    /// <summary>
    /// 악보 이미지에 음표, 박자에 정보를 가지고 있는다.
    /// </summary>
    public class Scale
    {
        private int _nScaleCount = 0;
        private int[] _nScaleKind = new int[0];
        private int[] _nScaleHeight = new int[0];
        private int[] _nScaleLength = new int[0];

        public Scale() 
        {
            ScaleCount = 0;
        }

        public int ScaleCount
        {
            get
            {
                return _nScaleCount;
            }
            set
            {
                _nScaleCount = value

                ScaleKind = new int[ScaleCount];
                ScaleHeight = new int[ScaleCount];
                ScaleLength = new int[ScaleCount];
            }
        }

        public int[] ScaleKind
        {
            get
            {
                return _nScaleKind;
            }
            set
            {
                _nScaleKind = value
            }
        }

        public int[] ScaleHeight
        {
            get
            {
                return _nScaleHeight;
            }
            set
            {
                _nScaleHeight = value
            }
        }

        public int[] ScaleLength
        {
            get
            {
                return _nScaleLength;
            }
            set
            {
                _nScaleLength = value
            }
        }
    }
    #endregion
}
