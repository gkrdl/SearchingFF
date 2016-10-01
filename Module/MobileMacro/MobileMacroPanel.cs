﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Module.Handling;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Module.Properties;
using Patagames.Ocr;
using Patagames.Ocr.Enums;
using System.Resources;
using System.Globalization;
using System.Collections;
using MCF.Classes.Data;


namespace Module.MobileMacro
{
    public partial class MobileMacroPanel : UserControl
    {
        Thread t;
        Adb mAdb = new Adb();
        Dictionary<string, Module.Handling.Imaging.ImageRange> dictRange = new Dictionary<string, Imaging.ImageRange>();
        Dictionary<string, Point> dictPoint = new Dictionary<string,Point>();
        Dictionary<string, Bitmap> dictImage = new Dictionary<string, Bitmap>();
        Point PNull = new Point();
        int buyCount = 0;
        Int64 profit = 0;
        Int64 totalProfit = 0;
        Stopwatch sw = new Stopwatch();
        OcrApi api;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public MobileMacroPanel()
        {
            InitializeComponent();
            this.Load += MobileMacroPanel_Load;
        }

        void MobileMacroPanel_Load(object sender, EventArgs e)
        {
            mAdb.adbPath = SearchADBFilename();
            btnFind.Click += btnFind_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnStart.Click += btnStart_Click;
            btnCapture.Click += btnCapture_Click;
            btnRefresh_Click(null, null);

            if (cboADBList.Items.Count > 0)
                cboADBList.SelectedIndex = 0;

            Init();
            
            //btnStart.PerformClick();
        }

        private void Macro()
        {            
            Imaging.GetScreen();
            Bitmap big = Imaging.bit;
            if (ImageMatch(big, "모바일_메인화면"))
            {
                Touch("모바일_상점_클릭");
                UStatus("상점메뉴 이동");
                Touch("모바일_선수영입_클릭");
                UStatus("선수영입메뉴 이동");
            }
            else if (ImageMatch(big, "모바일_선수영입"))
            {
                if (ImageMatch(big, "모바일_선수영입_일반선수"))
                {
                    if (ImageMatch(big, "모바일_선수영입_이용권소진"))
                    {
                        Touch("모바일_상점_클릭");
                        UStatus("상점메뉴 이동");
                        Touch("모바일_단품_클릭");
                        UStatus("단품메뉴 이동");
                    }
                    else
                    {
                        Touch("모바일_선수영입_영입_클릭", 4500);
                        //UStatus("선수영입 완료");
                    }
                }
                else
                    Touch("모바일_선수영입_일반선수_클릭");
            }
            else if (ImageMatch(big, "모바일_선수영입_공간필요"))
            {
                Touch("모바일_선수영입_공간필요닫기_클릭");
                UStatus("선수 자리 부족");
                Touch("모바일_구단관리_클릭");
                UStatus("구단관리메뉴 이동");
                Touch("모바일_트레이드_클릭");
                UStatus("트레이드메뉴 이동");

            }
            else if (ImageMatch(big, "모바일_트레이드"))
            {
                Touch("모바일_트레이드_초기화_클릭");
                for (int i = 1; i < 9; i++)
                {
                    string ocr = GetOCR(big, dictRange["모바일_트레이드_재료선수" + i.ToString()]);
                    try
                    {
                        Int64 price = 0;
                        Int64.TryParse(ParseOCR(ocr), out price);

                        if (price < 10000)
                        {
                            Touch("모바일_트레이드_재료선수" + i.ToString() + "_클릭");
                        }
                        else
                            break;
                    }
                    catch (Exception)
                    {
                        UStatus(ParseOCR(ocr));
                    }

                }
                Imaging.GetScreen();
                big = Imaging.bit;
                if (ImageMatch(big, "모바일_트레이드_트레이드실행"))
                {
                    Touch("모바일_트레이드_트레이드실행_클릭", 1000);
                }
                else
                {
                    Touch("모바일_이적시장_클릭");
                    UStatus("이적시장메뉴 이동");
                    Touch("모바일_판매_클릭", 3000);
                    Touch("모바일_이적시장_판매목록_클릭", 600);
                }
            }
            else if (ImageMatch(big, "모바일_트레이드_확인하기"))
            {
                Touch("모바일_트레이드_확인하기_클릭", 1000);
            }
            else if (ImageMatch(big, "모바일_트레이드_선수트레이드"))
            {
                
                string one1 = ParseOCR(GetOCR(big, dictRange["모바일_트레이드_선수1가격_1"], false));
                string one2 = ParseOCR(GetOCR(big, dictRange["모바일_트레이드_선수1가격_2"], false));
                string two1 = ParseOCR(GetOCR(big, dictRange["모바일_트레이드_선수2가격_1"], false));
                string two2 = ParseOCR(GetOCR(big, dictRange["모바일_트레이드_선수2가격_2"], false));
                string three1 = ParseOCR(GetOCR(big, dictRange["모바일_트레이드_선수3가격_1"], false));
                string three2 = ParseOCR(GetOCR(big, dictRange["모바일_트레이드_선수3가격_2"], false));



                one1 = one1 == string.Empty ? "0" : one1;
                one2 = one2 == string.Empty ? "0" : one2;
                two1 = two1 == string.Empty ? "0" : two1;
                two2 = two2 == string.Empty ? "0" : two2;
                three1 = three1 == string.Empty ? "0" : three1;
                three2 = three2 == string.Empty ? "0" : three2;
                Int64 one = 0;
                Int64 two = 0;
                Int64 three = 0;
                Int64 one11 = 0;
                Int64 one22 = 0;
                Int64 two11 = 0;
                Int64 two22 = 0;
                Int64 three11 = 0;
                Int64 three22 = 0;
                try
                {
                    Int64.TryParse(one1, out one11);
                    Int64.TryParse(one2, out one22);
                    Int64.TryParse(two1, out two11);
                    Int64.TryParse(two2, out two22);
                    Int64.TryParse(three1, out three11);
                    Int64.TryParse(three2, out three22);

                    UStatus("트레이드 결과 : ");
                    one = one11 + one22;
                    UStatus(string.Format("1번선수 EP합계 : {0}", one.ToString()));
                    two = two11 + two22;
                    UStatus(string.Format("2번선수 EP합계 : {0}", two.ToString()));
                    three = three11 + three22;
                    UStatus(string.Format("3번선수 EP합계 : {0}", three.ToString()));
                }
                catch (Exception)
                {
                    UStatus(one1 + "+" + one2);
                    UStatus(two1 + "+" + two2);
                    UStatus(three1 + "+" + three2);
                }
                finally
                {
                    Dictionary<string, Int64> dictPrice = new Dictionary<string, long>();
                    dictPrice.Add("one", one);
                    dictPrice.Add("two", two);
                    dictPrice.Add("three", three);

                    foreach (KeyValuePair<string, Int64> item in dictPrice.OrderByDescending(k => k.Value))
                    {
                        switch (item.Key)
                        {
                            case "one":
                                Touch("모바일_트레이드_선수1_클릭");
                                UStatus("1번선수 선택");
                                break;
                            case "two":
                                Touch("모바일_트레이드_선수2_클릭");
                                UStatus("2번선수 선택");
                                break;
                            case "three":
                                Touch("모바일_트레이드_선수3_클릭");
                                UStatus("3번선수 선택");
                                break;
                            default:
                                Touch("모바일_트레이드_선수1_클릭");
                                UStatus("1번선수 선택");
                                break;
                        }
                        break;
                    }

                    Touch("모바일_트레이드_트레이드실행2_클릭");
                }

            }
            else if (ImageMatch(big, "모바일_트레이드_트레이드결과"))
            {
                Touch("모바일_트레이드_완료_클릭");
                UStatus("트레이드 완료");
            }
            else if (ImageMatch(big, "모바일_이적시장_모두받기"))
            {
                Touch("모바일_이적시장_모두받기_클릭", 600);
            }
            else if (ImageMatch(big, "모바일_이적시장_모두받기_받기"))
            {
                string strProfit = "";
                try
                {
                    strProfit = ParseOCR(GetOCR(big, dictRange["모바일_이적시장_금액확인"]));
                    profit = Convert.ToInt64(strProfit);
                    //Int64.TryParse(strProfit, out profit);
                    UStatus(string.Format("판매대금 : {0}", profit.ToString()));
                    totalProfit += profit;
                }
                catch (Exception)
                {
                    UStatus(string.Format("판매대금 계산 오류"));
                    UStatus(strProfit);
                }
                finally
                {
                    Touch("모바일_이적시장_모두받기_받기_클릭", 5000);
                    UStatus(string.Format("판매대금 수령 완료"));
                    profit = 0;
                    UStatus("판매 시작");
                }
            }
            else if (ImageMatch(big, "모바일_이적시장"))
            {

                Touch("모바일_이적시장_판매등록화면_클릭");
                Touch("모바일_이적시장_첫선수_클릭");
                Imaging.GetScreen();
                big = Imaging.bit;
                if (ImageMatch(big, "모바일_이적시장_판매"))
                {
                    Touch("모바일_이적시장_판매_클릭", 700);
                    Imaging.GetScreen();
                    big = Imaging.bit;
                    if (ImageMatch(big, "모바일_이적시장_등록초과"))
                    {
                        Touch("모바일_이적시장_등록초과확인_클릭");
                        Touch("모바일_이적시장_취소_클릭");
                        UStatus("이적시장 등록초과");
                        Touch("모바일_상점_클릭");
                        UStatus("상점메뉴 이동");
                        Touch("모바일_선수영입_클릭");
                        UStatus("선수영입메뉴 이동");
                    }
                }
                else
                {
                    UStatus("모든 선수 판매 등록 완료.");
                    Touch("모바일_상점_클릭");
                    UStatus("상점메뉴 이동");
                    Touch("모바일_선수영입_클릭");
                    UStatus("선수영입메뉴 이동");
                }
            }
            else if (ImageMatch(big, "모바일_이적시장_판매등록"))
            {
                //big = Imaging.GetScreen();

                try
                {
                    if (ImageMatch(big, "모바일_이적시장_판매대기"))
                    {
                        string ocr = GetOCR(big, dictRange["모바일_이적시장_판매가격"]);
                        Int64 price = Convert.ToInt64(ocr.Replace(",", ""));

                        if ((price < 10000 && price >= 1000) || (price < 100 && ocr.Split(',').Length == 2))
                        {
                            Touch("모바일_이적시장_취소_클릭");
                            UStatus("10,000EP 이하의 선수로 판별. 판매 취소");
                            Touch("모바일_상점_클릭");
                            UStatus("상점메뉴 이동");
                            Touch("모바일_선수영입_클릭");
                            UStatus("선수영입메뉴 이동");
                        }
                        else
                        {
                            Touch("모바일_이적시장_판매2_클릭");
                            Touch("모바일_이적시장_판매확인_클릭");
                        }
                    }
                    else
                    {
                        Touch("모바일_이적시장_판매2_클릭");
                        Touch("모바일_이적시장_판매확인_클릭");
                    }
                }
                catch (Exception)
                {
                    Touch("모바일_이적시장_판매2_클릭");
                    Touch("모바일_이적시장_판매확인_클릭");
                }
            }
            else if (ImageMatch(big, "모바일_상점"))
            {
                Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
                Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
                Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
                Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
                Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
                if (buyCount < 10)
                {
                    Touch("모바일_상점_이용권구매_클릭");
                    UStatus("이용권 구매 시작");
                    Imaging.GetScreen();
                    big = Imaging.bit;
                    if (ImageMatch(big, "모바일_상점_이용권"))
                    {
                        Touch("모바일_상점_구매수_클릭", 600);
                        Touch("모바일_상점_구매수설정_클릭", 1000);
                        Touch("모바일_상점_구매수설정확인_클릭", 600);
                        Touch("모바일_상점_구입_클릭", 0);
                        Touch("모바일_상점_구입_클릭", 0);
                    }
                }
                else
                {
                    buyCount = 0;
                    Touch("모바일_상점_클릭");
                    UStatus("이용권 구입완료. 상점메뉴 이동");
                    Touch("모바일_선수영입_클릭");
                    UStatus("선수영입메뉴 이동");
                }
            }
            else if (ImageMatch(big, "모바일_상점_구입완료"))
            {
                Touch("모바일_상점_구입확인_클릭");
                buyCount++;
                UStatus("영입이용권 " + buyCount + "/10 구매완료");
                totalProfit -= 500000;
            }


            //GetOCR(big, new Point(170, 436), 201, 36);
            //mAdb.Touch(dictPoint["모바일_선수영입_영입_클릭"]);
            //big = null;
            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            Macro();
        }

        void Init()
        {
            OcrApi.PathToEngine = Environment.CurrentDirectory + @"\tesseract.dll";
            api = OcrApi.Create();
            Languages[] lang = { Languages.English };
            api.Init(lang, null, OcrEngineMode.OEM_CUBE_ONLY);
            api.SetVariable("tessedit_char_whitelist", "0123456789");


            timer.Interval = 1000;
            timer.Tick += timer_Tick;

            ResourceSet set = global::Module.Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            foreach (DictionaryEntry  item in set)
            {
                if (item.Value is Bitmap)
                    dictImage.Add(item.Key.ToString(), (Bitmap)item.Value);
            }

            Imaging.ImageRange range = new Imaging.ImageRange(0, 35, 483, 100);
            dictRange.Add("모바일_메인화면", range);
            dictRange.Add("모바일_상점", range);
            dictRange.Add("모바일_선수영입", range);
            dictRange.Add("모바일_이적시장", range);
            dictRange.Add("모바일_트레이드", range);
            range = new Imaging.ImageRange(27, 172, 171, 235);
            dictRange.Add("모바일_상점_이용권", range);
            range = new Imaging.ImageRange(35, 125, 411, 51);
            dictRange.Add("모바일_상점_구입완료", range);
            range = new Imaging.ImageRange(207, 127, 277, 69);
            dictRange.Add("모바일_선수영입_일반선수", range);
            range = new Imaging.ImageRange(30, 320, 455, 75);
            dictRange.Add("모바일_선수영입_공간필요", range);
            range = new Imaging.ImageRange(0, 690, 484, 67);
            dictRange.Add("모바일_선수영입_이용권소진", range);

            range = new Imaging.ImageRange(360, 400, 124, 30);
            dictRange.Add("모바일_트레이드_재료선수1", range);
            range = new Imaging.ImageRange(360, 446, 124, 34);
            dictRange.Add("모바일_트레이드_재료선수2", range);
            range = new Imaging.ImageRange(360, 496, 124, 30);
            dictRange.Add("모바일_트레이드_재료선수3", range);
            range = new Imaging.ImageRange(360, 542, 124, 30);
            dictRange.Add("모바일_트레이드_재료선수4", range);
            range = new Imaging.ImageRange(360, 588, 124, 30);
            dictRange.Add("모바일_트레이드_재료선수5", range);
            range = new Imaging.ImageRange(360, 634, 124, 30);
            dictRange.Add("모바일_트레이드_재료선수6", range);
            range = new Imaging.ImageRange(360, 682, 124, 30);
            dictRange.Add("모바일_트레이드_재료선수7", range);
            range = new Imaging.ImageRange(360, 728, 124, 30);
            dictRange.Add("모바일_트레이드_재료선수8", range);
            range = new Imaging.ImageRange(250, 300, 221, 43);
            dictRange.Add("모바일_트레이드_트레이드실행", range);
            range = new Imaging.ImageRange(135, 765, 230, 55);
            dictRange.Add("모바일_트레이드_확인하기", range);
            range = new Imaging.ImageRange(170, 65, 155, 45);
            dictRange.Add("모바일_트레이드_선수트레이드", range);
            range = new Imaging.ImageRange(250, 345, 80, 20);
            dictRange.Add("모바일_트레이드_선수1가격_1", range);
            range = new Imaging.ImageRange(250, 363, 80, 27);
            dictRange.Add("모바일_트레이드_선수1가격_2", range);
            range = new Imaging.ImageRange(250, 458, 80, 22);
            dictRange.Add("모바일_트레이드_선수2가격_1", range);
            range = new Imaging.ImageRange(250, 478, 80, 22);
            dictRange.Add("모바일_트레이드_선수2가격_2", range);
            range = new Imaging.ImageRange(250, 569, 80, 20);
            dictRange.Add("모바일_트레이드_선수3가격_1", range);
            range = new Imaging.ImageRange(250, 589, 80, 20);
            dictRange.Add("모바일_트레이드_선수3가격_2", range);
            range = new Imaging.ImageRange(163, 63, 165, 41);
            dictRange.Add("모바일_트레이드_트레이드결과", range);
            


            range = new Imaging.ImageRange(345, 300, 140, 60);
            dictRange.Add("모바일_이적시장_판매", range);
            range = new Imaging.ImageRange(147, 75, 313, 48);
            dictRange.Add("모바일_이적시장_판매등록", range);
            range = new Imaging.ImageRange(175, 315, 154, 67);
            dictRange.Add("모바일_이적시장_등록초과", range);
            range = new Imaging.ImageRange(25, 675, 433, 81);
            dictRange.Add("모바일_이적시장_판매대기", range);
            range = new Imaging.ImageRange(150, 395, 294, 44);
            dictRange.Add("모바일_이적시장_판매가격", range);
            range = new Imaging.ImageRange(355, 300, 105, 37);
            dictRange.Add("모바일_이적시장_모두받기", range);
            range = new Imaging.ImageRange(250, 580, 170, 37);
            dictRange.Add("모바일_이적시장_모두받기_받기", range);
            range = new Imaging.ImageRange(150, 400, 290, 42);
            dictRange.Add("모바일_이적시장_금액확인", range);


            dictPoint.Add("모바일_상점_클릭", new Point(360, 765));
            dictPoint.Add("모바일_단품_클릭", new Point(215, 540));
            dictPoint.Add("모바일_선수영입_클릭", new Point(370, 435));
            dictPoint.Add("모바일_구단관리_클릭", new Point(110, 755));
            dictPoint.Add("모바일_트레이드_클릭", new Point(360, 535));
            dictPoint.Add("모바일_이적시장_클릭", new Point(195, 765));
            dictPoint.Add("모바일_판매_클릭", new Point(235, 665));


            dictPoint.Add("모바일_선수영입_일반선수_클릭", new Point(320, 120));
            dictPoint.Add("모바일_선수영입_영입_클릭", new Point(110, 685));
            dictPoint.Add("모바일_선수영입_공간필요닫기_클릭", new Point(90, 470));


            dictPoint.Add("모바일_트레이드_재료선수1_클릭", new Point(222, 375));
            dictPoint.Add("모바일_트레이드_재료선수2_클릭", new Point(222, 425));
            dictPoint.Add("모바일_트레이드_재료선수3_클릭", new Point(222, 475));
            dictPoint.Add("모바일_트레이드_재료선수4_클릭", new Point(222, 520));
            dictPoint.Add("모바일_트레이드_재료선수5_클릭", new Point(222, 570));
            dictPoint.Add("모바일_트레이드_재료선수6_클릭", new Point(222, 615));
            dictPoint.Add("모바일_트레이드_재료선수7_클릭", new Point(222, 665));
            dictPoint.Add("모바일_트레이드_재료선수8_클릭", new Point(222, 710));
            dictPoint.Add("모바일_트레이드_트레이드실행_클릭", new Point(300, 280));
            dictPoint.Add("모바일_트레이드_확인하기_클릭", new Point(155, 745));
            dictPoint.Add("모바일_트레이드_선수1_클릭", new Point(160, 300));
            dictPoint.Add("모바일_트레이드_선수2_클릭", new Point(160, 420));
            dictPoint.Add("모바일_트레이드_선수3_클릭", new Point(160, 525));
            dictPoint.Add("모바일_트레이드_트레이드실행2_클릭", new Point(300, 755));
            dictPoint.Add("모바일_트레이드_완료_클릭", new Point(34, 46));
            dictPoint.Add("모바일_트레이드_초기화_클릭", new Point(120, 285));
            


            dictPoint.Add("모바일_이적시장_판매목록_클릭", new Point(305, 110));
            dictPoint.Add("모바일_이적시장_판매등록화면_클릭", new Point(163, 115));
            dictPoint.Add("모바일_이적시장_모두받기_클릭", new Point(400, 285));
            dictPoint.Add("모바일_이적시장_모두받기_받기_클릭", new Point(335, 565));
            dictPoint.Add("모바일_이적시장_첫선수_클릭", new Point(85, 380));
            dictPoint.Add("모바일_이적시장_판매_클릭", new Point(400, 285));
            dictPoint.Add("모바일_이적시장_판매2_클릭", new Point(310, 675));
            dictPoint.Add("모바일_이적시장_판매확인_클릭", new Point(210, 675));
            dictPoint.Add("모바일_이적시장_취소_클릭", new Point(110, 675));
            dictPoint.Add("모바일_이적시장_등록초과확인_클릭", new Point(235, 470));

            
            dictPoint.Add("모바일_상점_스와이프1", new Point(265, 640));
            dictPoint.Add("모바일_상점_스와이프2", new Point(265, 135));
            dictPoint.Add("모바일_상점_이용권구매_클릭", new Point(50, 515));


            dictPoint.Add("모바일_상점_구매수_클릭", new Point(240, 173));
            dictPoint.Add("모바일_상점_구매수설정_클릭", new Point(120, 325));
            dictPoint.Add("모바일_상점_구매수설정확인_클릭", new Point(280, 570));
            dictPoint.Add("모바일_상점_구입_클릭", new Point(275, 650));
            dictPoint.Add("모바일_상점_구입확인_클릭", new Point(275, 650));
        }

        #region 이벤트
        void timer_Tick(object sender, EventArgs e)
        {
            //this.Invoke(new MethodInvoker(delegate()
            //{
                label8.Text = sw.Elapsed.ToString("hh\\:mm\\:ss");
                label7.Text = totalProfit.ToString();
            //}));
        }

        void btnCapture_Click(object sender, EventArgs e)
        {
            Imaging.GetScreen();
            Bitmap big = Imaging.bit;
            big = Imaging.ConvertFormat(big, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            big = grayscale(big);
            big.Save(Environment.CurrentDirectory + "\\test2.png");
            //Clipboard.SetImage(();
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            cboADBList.Items.Clear();
            Cursor = Cursors.WaitCursor;
            btnRefresh.Text = "갱신중";
            btnRefresh.Enabled = false;

            try
            {
                string[] devices = mAdb.getDievices();
                foreach (string device in devices)
                    cboADBList.Items.Add(device);
            }
            catch (Exception exc)
            {
                MessageBox.Show(this, exc.ToString(), "예외 발생", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnRefresh.Enabled = true;
            btnRefresh.Text = "새로고침";
            Cursor = Cursors.Default;
        }

        void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text.Equals("시작"))
            {
                if (cboADBList.SelectedIndex < 0)
                    return;

                btnRefresh.Enabled = false;
                cboADBList.Enabled = false;
                chkReceive.Enabled = false;
                chkSell.Enabled = false;
                chkTrade.Enabled = false;
                txtSell.Enabled = false;
                txtTrade.Enabled = false;

                Imaging.GetHWND();
                btnStart.Text = "중지";
                mAdb.device = cboADBList.Text;
                t = new Thread(Macro, Int32.MaxValue);
                t.Start();
                sw.Start();
                timer.Start();
            }
            else
            {
                btnRefresh.Enabled = true;
                cboADBList.Enabled = true;
                chkReceive.Enabled = true;
                chkSell.Enabled = true;
                chkTrade.Enabled = true;
                txtSell.Enabled = true;
                txtTrade.Enabled = true;
                
                btnStart.Text = "시작";
                t.Abort();
                sw.Stop();
                timer.Stop();
            }
        }

        void btnFind_Click(object sender, EventArgs e)
        {
            //mAdb.bmp.Save("C:\\test.png");
            //mAdb.Touch(150, 480);
            
            //Point p = ImageMatch("Mobile_shop_button");
            //mAdb.Touch(p.X, p.Y + dictRange["Mobile_shop_button"].loc.Y);
            ////p = new Point(p.X, p.Y);
            ////Handling.MessageCtr.SendKey(p);
            //Monitoring();

            //Imaging.GetScreen().Save("C:\\test123.png");
            //Handling.MessageCtr.SendKey(new Point(156, 511));

            
        }
        #endregion

        #region 매크로 영역

        #endregion
        #region 사용자함수
        Point NullPoint = new Point(0, 0);
        public Bitmap grayscale(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int[] arr = new int[225];
            int i = 0;
            Color p;

            //Grayscale
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    p = bmp.GetPixel(x, y);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    int avg = (r + g + b) / 3;

                    //if (g > 60 && g < 160)
                    //    avg = 255;
                    avg = avg < 127 ? 0 : 255;     // Converting gray pixels to either pure black or pure white
                    
                    bmp.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }
            return bmp;
        }
        string ParseOCR(string str)
        {
            str = str.Split(' ')[0].Replace("EP", "")
                    .Replace("EF", "")
                    .Replace(" ", "")
                    .Replace("\n\n", "")
                    .Replace("l", "1")
                    .Replace("I", "1")
                    .Replace("L", "1")
                    .Replace("q", "4")
                    .Replace("A", "4")
                    .Replace("O", "0")
                    .Replace("T", "7")
                    .Replace("tr", "0")
                    .Replace("im", "00")
                    .Replace("t1", "0")
                    .Replace(",", "")
                    .Replace("-", "")
                    .Replace("/f", "6")
                    .Replace("S", "5")
                    .Replace("ir", "0")
                    .Replace("B", "8")
                    .Replace("G", "6")
                    .Replace("W", "0")
                    .Replace("i", "1")
                    .Replace("n", "1")
                    .Replace("/?", "9")
                    .Replace("d", "0")
                    .Replace("f1", "8");

            return str;
        }
        void UStatus(string msg)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                txtLog.AppendText("[" + GetServerTime().ToString("MM-dd HH:mm:ss") + "] " + msg);
                txtLog.AppendText("\r\n");
                txtLog.ScrollToCaret();
            }));
        }

        DateTime GetServerTime()
        {
            string sql = string.Format(@"SELECT NOW();");
            DateTime nowTIme = Convert.ToDateTime(MySqlHelper.ExecuteDataTable(sql).Rows[0][0].ToString());
            return nowTIme;
        }

        private string GetOCR(Bitmap big, Imaging.ImageRange range, bool flag = true)
        {
            Point p = new Point();
            int width = 0;
            int height = 0;

            p.X = range.loc.X;
            p.Y = range.loc.Y;
            width = range.width;
            height = range.height;

            big = Imaging.CropImage(big, p, width, height);
            big = Imaging.ConvertFormat(big, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            if (!flag)
                big = Imaging.MakeGrayscale3(big);
            else
                big = grayscale(big);

            string plainText = api.GetTextFromImage(big);
            ////Console.WriteLine(plainText);
            ////this.Invoke(new MethodInvoker(delegate() { txtLog.AppendText(plainText + Environment.NewLine); }));
            //api.Dispose();
            //api = null;
            //System.GC.Collect(0, GCCollectionMode.Forced);
            //System.GC.WaitForPendingFinalizers();
            return plainText;
        }

        //private void Monitoring()
        //{
        //    while (true)
        //    {
        //        //mAdb.Capture();
        //        //image.Image = mAdb.bmp;
        //        image.Image = Imaging.GetScreen();
        //        Thread.Sleep(200);
        //    }
            
        //}

        private string SearchADBFilename()
        {
            string ADBFilename;

            //블루스택 설치된 폴더로 찾기
            ADBFilename = string.Format(@"{0}\BlueStacks\HD-Adb.exe", Environment.GetEnvironmentVariable(
                (8 == IntPtr.Size || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))) ?
                "ProgramFiles(x86)" : "ProgramFiles"));
            if (File.Exists(ADBFilename))
                return ADBFilename;

            //녹스 설치된 폴더로 찾기
            ADBFilename = string.Format(@"{0}\Nox\bin\nox_adb.exe", Environment.GetEnvironmentVariable("APPDATA"));
            if (File.Exists(ADBFilename))
                return ADBFilename;

            //실행중인 프로세스로 찾기
            List<Process> processes = new List<Process>();
            processes.AddRange(Process.GetProcessesByName("HD-Frontend"));
            processes.AddRange(Process.GetProcessesByName("Nox"));
            if (processes.Count > 0)
            {
                string ADBName = null;
                switch (processes[0].ProcessName)
                {
                    case "HD-Frontend":
                        ADBName = "HD-Adb";
                        break;

                    case "Nox":
                        ADBName = "nox_adb";
                        break;
                }
                ADBFilename = processes[0].Modules[0].FileName;
                ADBFilename = string.Format("{0}{1}.exe", ADBFilename.Remove(ADBFilename.LastIndexOf("\\") + 1), ADBName);
                return ADBFilename;
            }

            return null;
        }

        private bool ImageMatch(Bitmap big, string destName)
        {
            //((Bitmap)Resources.ResourceManager.GetObject(destName)).Save("C:\\test1.png");
            return Imaging.ImgMatch(big, dictImage[destName], dictRange[destName]) != PNull;
        }

        private void Touch(string destName, int sleepTime = 300)
        {
            mAdb.Touch(dictPoint[destName]);
            Thread.Sleep(sleepTime);
        }

        private void Swipe(string destName, string destName2, int sleepTime = 300)
        {
            mAdb.Swipe(dictPoint[destName], dictPoint[destName2]);
            Thread.Sleep(sleepTime);
        }
        #endregion
    }
}
