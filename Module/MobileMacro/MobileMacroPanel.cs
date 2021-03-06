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
//using Patagames.Ocr;
//using Patagames.Ocr.Enums;
using System.Resources;
using System.Globalization;
using System.Collections;
using MCF.Classes.Data;
//using MODI;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;


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
        Stopwatch sw = new Stopwatch();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        bool runFlag = false;
        MacroStatus status = MacroStatus.Release;
        int errorCount = 0;

        private enum MacroStatus
        {
            Main, Trade, Scout, Shop, TradeMarket, Release
        }

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

            cboSupervisor.SelectedIndex = 1;

            Init();
            
            //btnStart.PerformClick();
        }

        void Init()
        {
            timer.Interval = 500;
            timer.Tick += timer_Tick;

            ResourceSet set = global::Module.Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            foreach (DictionaryEntry  item in set)
            {
                if (item.Value is Bitmap)
                    dictImage.Add(item.Key.ToString(), (Bitmap)item.Value);
            }

            Imaging.ImageRange range = new Imaging.ImageRange(0, 35, 483, 100);
            //dictRange.Add("모바일_메인화면", range);
            //dictRange.Add("모바일_상점", range);
            //dictRange.Add("모바일_선수영입", range);
            //dictRange.Add("모바일_이적시장", range);
            //dictRange.Add("모바일_트레이드", range);


            //range = new Imaging.ImageRange(147, 129, 210, 47);
            //dictRange.Add("모바일_상점_구입하기", range);
            //range = new Imaging.ImageRange(27, 172, 171, 235);
            //dictRange.Add("모바일_상점_이용권", range);
            //range = new Imaging.ImageRange(35, 125, 411, 51);
            //dictRange.Add("모바일_상점_구입완료", range);
            //range = new Imaging.ImageRange(99, 291, 295, 43);
            //dictRange.Add("모바일_상점_구매수", range);


            //range = new Imaging.ImageRange(207, 127, 277, 69);
            //dictRange.Add("모바일_선수영입_일반선수", range);
            //range = new Imaging.ImageRange(70, 645, 350, 70);
            //dictRange.Add("모바일_선수영입_프리미엄선수", range);
            //range = new Imaging.ImageRange(30, 320, 455, 75);
            //dictRange.Add("모바일_선수영입_공간필요", range);
            //range = new Imaging.ImageRange(0, 690, 484, 67);
            //dictRange.Add("모바일_선수영입_이용권소진", range);

            range = new Imaging.ImageRange(250, 214, 67, 25);
            dictRange.Add("모바일_트레이드_재료선수1", range);
            range = new Imaging.ImageRange(250, 243, 67, 25);
            dictRange.Add("모바일_트레이드_재료선수2", range);
            range = new Imaging.ImageRange(250, 272, 67, 30);
            dictRange.Add("모바일_트레이드_재료선수3", range);
            range = new Imaging.ImageRange(250, 300, 67, 30);
            dictRange.Add("모바일_트레이드_재료선수4", range);
            range = new Imaging.ImageRange(250, 328, 67, 30);
            dictRange.Add("모바일_트레이드_재료선수5", range);
            range = new Imaging.ImageRange(250, 356, 67, 30);
            dictRange.Add("모바일_트레이드_재료선수6", range);
            range = new Imaging.ImageRange(250, 383, 67, 30);
            dictRange.Add("모바일_트레이드_재료선수7", range);
            range = new Imaging.ImageRange(250, 413, 67, 30);
            dictRange.Add("모바일_트레이드_재료선수8", range);
            //range = new Imaging.ImageRange(250, 300, 221, 43);
            //dictRange.Add("모바일_트레이드_트레이드실행", range);
            //range = new Imaging.ImageRange(135, 765, 230, 55);
            //dictRange.Add("모바일_트레이드_확인하기", range);
            //range = new Imaging.ImageRange(170, 65, 155, 45);
            //dictRange.Add("모바일_트레이드_선수트레이드", range);
            range = new Imaging.ImageRange(170, 186, 51, 11);
            dictRange.Add("모바일_트레이드_선수1가격_1", range);
            range = new Imaging.ImageRange(170, 199, 51, 11);
            dictRange.Add("모바일_트레이드_선수1가격_2", range);
            range = new Imaging.ImageRange(170, 254, 51, 11);
            dictRange.Add("모바일_트레이드_선수2가격_1", range);
            range = new Imaging.ImageRange(170, 266, 51, 11);
            dictRange.Add("모바일_트레이드_선수2가격_2", range);
            range = new Imaging.ImageRange(170, 320, 51, 11);
            dictRange.Add("모바일_트레이드_선수3가격_1", range);
            range = new Imaging.ImageRange(170, 332, 51, 11);
            dictRange.Add("모바일_트레이드_선수3가격_2", range);
            //range = new Imaging.ImageRange(163, 63, 165, 41);
            //dictRange.Add("모바일_트레이드_트레이드결과", range);
            //range = new Imaging.ImageRange(100, 590, 295, 238);
            //dictRange.Add("모바일_트레이드_재협상", range);
            


            //range = new Imaging.ImageRange(345, 300, 140, 60);
            //dictRange.Add("모바일_이적시장_판매", range);
            //range = new Imaging.ImageRange(147, 75, 313, 48);
            //dictRange.Add("모바일_이적시장_판매등록", range);
            //range = new Imaging.ImageRange(175, 315, 154, 67);
            //dictRange.Add("모바일_이적시장_등록초과", range);
            //range = new Imaging.ImageRange(25, 675, 433, 81);
            //dictRange.Add("모바일_이적시장_판매대기", range);
            //range = new Imaging.ImageRange(255, 220, 64, 20); // 리스트화면 가격
            //range = new Imaging.ImageRange(174, 218, 110, 18); // 등록화면 가격
            //range = new Imaging.ImageRange(131, 215, 159, 27); // 등록화면 가격
            //range = new Imaging.ImageRange(230, 290, 55, 18); // 등록화면 하한가
            range = new Imaging.ImageRange(207, 220, 75, 18); // 등록화면 하한가
            dictRange.Add("모바일_이적시장_판매가격", range);
            //range = new Imaging.ImageRange(355, 300, 105, 37);
            //dictRange.Add("모바일_이적시장_모두받기", range);
            //range = new Imaging.ImageRange(250, 580, 170, 37);
            //dictRange.Add("모바일_이적시장_모두받기_받기", range);
            //range = new Imaging.ImageRange(150, 400, 290, 42);
            //dictRange.Add("모바일_이적시장_금액확인", range);


            dictPoint.Add("모바일_메인화면_1번감독선택", new Point(165, 180));
            dictPoint.Add("모바일_메인화면_2번감독선택", new Point(165, 335));
            dictPoint.Add("모바일_메인화면_게임종료취소", new Point(95, 285));


            dictPoint.Add("모바일_상점_클릭", new Point(240, 460));
            dictPoint.Add("모바일_단품_클릭", new Point(163, 337));
            dictPoint.Add("모바일_선수영입_클릭", new Point(260, 275));
            dictPoint.Add("모바일_구단관리_클릭", new Point(83, 460));
            dictPoint.Add("모바일_팀관리_클릭", new Point(65, 340));
            dictPoint.Add("모바일_트레이드_클릭", new Point(260, 335));
            dictPoint.Add("모바일_이적시장_클릭", new Point(135, 460));
            dictPoint.Add("모바일_판매_클릭", new Point(160, 400));


            dictPoint.Add("모바일_선수영입_일반선수_클릭", new Point(240, 75));
            dictPoint.Add("모바일_선수영입_영입_클릭", new Point(225, 415));
            dictPoint.Add("모바일_선수영입_공간필요닫기_클릭", new Point(100, 285));


            dictPoint.Add("모바일_트레이드_재료선수1_클릭", new Point(222, 225));
            dictPoint.Add("모바일_트레이드_재료선수2_클릭", new Point(222, 255));
            dictPoint.Add("모바일_트레이드_재료선수3_클릭", new Point(222, 285));
            dictPoint.Add("모바일_트레이드_재료선수4_클릭", new Point(222, 310));
            dictPoint.Add("모바일_트레이드_재료선수5_클릭", new Point(222, 340));
            dictPoint.Add("모바일_트레이드_재료선수6_클릭", new Point(222, 370));
            dictPoint.Add("모바일_트레이드_재료선수7_클릭", new Point(222, 400));
            dictPoint.Add("모바일_트레이드_재료선수8_클릭", new Point(222, 430));
            dictPoint.Add("모바일_트레이드_트레이드실행_클릭", new Point(245, 170));
            dictPoint.Add("모바일_트레이드_확인하기_클릭", new Point(235, 455));
            dictPoint.Add("모바일_트레이드_선수1_클릭", new Point(185, 190));
            dictPoint.Add("모바일_트레이드_선수2_클릭", new Point(185, 255));
            //dictPoint.Add("모바일_트레이드_선수3_클릭", new Point(160, 525));
            dictPoint.Add("모바일_트레이드_트레이드실행2_클릭", new Point(235, 455));
            dictPoint.Add("모바일_트레이드_완료_클릭", new Point(30, 25));
            dictPoint.Add("모바일_트레이드_초기화_클릭", new Point(80, 170));
            dictPoint.Add("모바일_트레이드_재협상취소_클릭", new Point(110, 340));
            dictPoint.Add("모바일_트레이드_계속진행취소_클릭", new Point(68, 285));
            


            dictPoint.Add("모바일_이적시장_판매목록_클릭", new Point(215, 70));
            dictPoint.Add("모바일_이적시장_판매등록화면_클릭", new Point(110, 70));
            dictPoint.Add("모바일_이적시장_모두받기_클릭", new Point(270, 170));
            dictPoint.Add("모바일_이적시장_모두받기_받기_클릭", new Point(225, 335));
            dictPoint.Add("모바일_이적시장_첫선수_클릭", new Point(185, 230));
            dictPoint.Add("모바일_이적시장_판매_클릭", new Point(270, 170));
            dictPoint.Add("모바일_이적시장_판매2_클릭", new Point(225, 405));
            dictPoint.Add("모바일_이적시장_판매확인_클릭", new Point(160, 405));
            dictPoint.Add("모바일_이적시장_취소_클릭", new Point(100, 405));
            dictPoint.Add("모바일_이적시장_등록초과확인_클릭", new Point(160, 280));

            
            dictPoint.Add("모바일_상점_스와이프1", new Point(180, 395));
            dictPoint.Add("모바일_상점_스와이프2", new Point(180, 70));
            dictPoint.Add("모바일_상점_이용권구매_클릭", new Point(60, 315));
            dictPoint.Add("모바일_상점_구매취소", new Point(100, 395));
            dictPoint.Add("모바일_상점_구매수_클릭", new Point(230, 105));
            dictPoint.Add("모바일_상점_구매수설정_클릭", new Point(160, 195));
            dictPoint.Add("모바일_상점_구매수설정확인_클릭", new Point(215, 345));
            dictPoint.Add("모바일_상점_구입_클릭", new Point(225, 395));
            dictPoint.Add("모바일_상점_구입확인_클릭", new Point(225, 395));

            dictPoint.Add("모바일_선수단_클릭", new Point(162, 68));
            dictPoint.Add("모바일_선수단_소속정렬_클릭", new Point(298, 232));
            dictPoint.Add("모바일_선수단_선수1_클릭", new Point(222, 255));
            dictPoint.Add("모바일_선수단_선수2_클릭", new Point(222, 285));
            dictPoint.Add("모바일_선수단_선수3_클릭", new Point(222, 310));
            dictPoint.Add("모바일_선수단_선수4_클릭", new Point(222, 340));
            dictPoint.Add("모바일_선수단_선수5_클릭", new Point(222, 370));
            dictPoint.Add("모바일_선수단_선수6_클릭", new Point(222, 400));
            dictPoint.Add("모바일_선수단_선수7_클릭", new Point(222, 430));
            dictPoint.Add("모바일_선수단_방출_클릭", new Point(110, 205));
            dictPoint.Add("모바일_선수방출_방출_클릭", new Point(225, 400));
        }

        #region 매크로
        private void MobileGo_TradeMarket()
        {
            Touch("모바일_이적시장_클릭");
            UStatus("이적시장메뉴 이동");
            Touch("모바일_판매_클릭");
            Thread.Sleep(3000);
            if (chkReceive.Checked)
                Touch("모바일_이적시장_판매목록_클릭");
            Thread.Sleep(500);

            status = MacroStatus.TradeMarket;
        }
        private void MobileGo_Scout()
        {
            Touch("모바일_상점_클릭");
            UStatus("상점메뉴 이동");
            Touch("모바일_선수영입_클릭");
            UStatus("선수영입메뉴 이동");

            status = MacroStatus.Scout;
        }
        private void MobileGo_Trade()
        {
            Touch("모바일_구단관리_클릭");
            UStatus("구단관리메뉴 이동");
            Touch("모바일_트레이드_클릭");
            UStatus("트레이드메뉴 이동");

            status = MacroStatus.Trade;
        }

        private void MobileGo_Shop()
        {
            Touch("모바일_상점_클릭");
            UStatus("상점메뉴 이동");
            Touch("모바일_단품_클릭");
            UStatus("단품메뉴 이동");
            status = MacroStatus.Shop;
        }

        private void MobileGo_Release()
        {
            Touch("모바일_구단관리_클릭");
            UStatus("구단관리메뉴 이동");
            Touch("모바일_팀관리_클릭");
            UStatus("팀관리메뉴 이동");
            status = MacroStatus.Release;
        }


        private void MobileEscape()
        {
            runFlag = true;
            Escape();
            Escape();
            Escape();
            Escape();
            Escape();

            Bitmap big = Imaging.GetScreen();
            if (ImageMatch(big, "모바일_메인화면_게임종료"))
            {
                this.Invoke(new MethodInvoker(delegate()
                    {
                        Touch("모바일_메인화면_게임종료취소");
                        Touch("모바일_메인화면_" + cboSupervisor.Text.Substring(0, 1) + "번감독선택");
                        status = MacroStatus.Main;
                        errorCount = 0;
                    }));
            }
            else if (ImageMatch(big, "모바일_메인화면_감독선택"))
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    Touch("모바일_메인화면_" + cboSupervisor.Text.Substring(0, 1) + "번감독선택");
                    status = MacroStatus.Main;
                    errorCount = 0;
                }));
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            Thread.Sleep(10000);
            runFlag = false;
        }
        private bool MobileScout(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_선수영입"))
            {
                if (ImageMatch(big, "모바일_선수영입_이용권소진"))
                {
                    MobileGo_Shop();
                    //Thread.Sleep(1000);
                }
                else if (ImageMatch(big, "모바일_선수영입_프리미엄선수"))
                {
                    Touch("모바일_선수영입_일반선수_클릭");
                }
                else
                {
                    Touch("모바일_선수영입_영입_클릭");
                }
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();

            return flag;
        }

        private bool MobileTrade(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();
            

            if (ImageMatch(big, "모바일_트레이드"))
            {
                Touch("모바일_트레이드_초기화_클릭");
                bool selectFlag = false;
                for (int i = 1; i < 9; i++)
                {
                    //string ocr = "";
                    //if (!selectFlag)
                    //    ocr = GetOCR(big, dictRange["모바일_트레이드_재료선수" + i.ToString()]);
                    //try
                    //{

                    //    if (selectFlag)
                    //    {
                    Touch("모바일_트레이드_재료선수" + i.ToString() + "_클릭", 0);
                    //}
                    //else
                    //{
                    //    Int64 price = 0;
                    //    Int64.TryParse(ParseOCR(ocr), out price);
                    //    Int32 defaultP = 20000;
                    //    Int32.TryParse(txtTrade.Text, out defaultP);
                    //    if (price < defaultP && price != 0)
                    //    {
                    //        Touch("모바일_트레이드_재료선수" + i.ToString() + "_클릭");
                    //        selectFlag = true;
                    //    }
                    //}
                    //}
                    //catch (Exception)
                    //{
                    //    //UStatus(ParseOCR(ocr));
                    //}

                }

                flag = true;
                MobileTrade_Run();
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private void MobileTrade_Run(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();

            if (ImageMatch(big, "모바일_트레이드_트레이드실행"))
            {
                Touch("모바일_트레이드_트레이드실행_클릭");
                Thread.Sleep(500);
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
        }

        private bool MobileTrade_Continue(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();

            if (ImageMatch(big, "모바일_트레이드_계속진행"))
            {
                Touch("모바일_트레이드_계속진행취소_클릭");
                MobileGo_TradeMarket();
                flag = true;
            }


            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();


            return flag;
        }

        private bool MobileTrade_Open(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_트레이드_확인하기"))
            {
                Touch("모바일_트레이드_확인하기_클릭");
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileTrade_Select(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_트레이드_선수트레이드"))
            {
                Touch("모바일_트레이드_선수1_클릭");
                Touch("모바일_트레이드_트레이드실행2_클릭");
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileTrade_Renego(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_트레이드_재협상"))
            {
                Touch("모바일_트레이드_재협상취소_클릭");
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileTrade_Result(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_트레이드_트레이드결과"))
            {
                Touch("모바일_트레이드_완료_클릭");
                UStatus("트레이드 완료");
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileMarket_Allget(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_이적시장_모두받기"))
            {
                Touch("모바일_이적시장_모두받기_클릭");
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileTradeMarket_Allget_get(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_이적시장_모두받기_받기"))
            {

                Touch("모바일_이적시장_모두받기_받기_클릭");
                UStatus(string.Format("판매대금 수령 완료"));
                UStatus("판매 시작");
                flag = true;
                Thread.Sleep(5000);

            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileTradeMarket_Sell(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_이적시장"))
            {

                Touch("모바일_이적시장_판매등록화면_클릭");
                Touch("모바일_이적시장_첫선수_클릭");
                MobileMarket_Sell_Run();
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private void MobileMarket_Sell_Run()
        {
            Bitmap big = Imaging.GetScreen();

            if (ImageMatch(big, "모바일_이적시장_판매"))
            {
                Touch("모바일_이적시장_판매_클릭");
            }
            else
            {
                UStatus("모든 선수 판매 등록 완료.");
                MobileGo_Scout();
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
        }

        private bool MobileTradeMarket_Sell_Confirm(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_이적시장_판매등록"))
            {
                string ocr = GetOCR(big, dictRange["모바일_이적시장_판매가격"], true).Replace(".", "").Replace(",", "").Replace(" ","").Trim();
                int intOCR = 0;
                Int32.TryParse(ocr, out intOCR);
                Console.WriteLine(ocr);
                if (intOCR > 30000 || intOCR < 1000)
                {
                    Touch("모바일_이적시장_판매2_클릭");
                    Touch("모바일_이적시장_판매확인_클릭");
                    UStatus("선수 가격 : " + intOCR + " EP 등록완료.");
                }
                else
                {
                    Touch("모바일_이적시장_취소_클릭");
                    UStatus("모든 선수 판매 등록 완료.");
                    MobileGo_Release();
                }
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();

            return flag;
        }

        private bool MobileTradeMarket_Sell_Over(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_이적시장_등록초과"))
            {
                Touch("모바일_이적시장_등록초과확인_클릭");
                Touch("모바일_이적시장_취소_클릭");
                UStatus("이적시장 등록초과");
                MobileGo_Scout();
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileShop(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_상점"))
            {
                if (buyCount < 10)
                {
                    if (ImageMatch(big, "모바일_상점_이용권선택"))
                        Touch("모바일_상점_이용권구매_클릭");
                    else
                        MobileShop_Swipe();

                }
                else
                {
                    buyCount = 0;
                    MobileGo_Scout();
                }

                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private void MobileShop_Swipe()
        {
            Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
            Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
            Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
            Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
            Swipe("모바일_상점_스와이프1", "모바일_상점_스와이프2");
        }

        private bool MobileShop_Buy(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_상점_구입하기"))
            {
                if (ImageMatch(big, "모바일_상점_이용권"))
                    Touch("모바일_상점_구매수_클릭");
                else
                    Touch("모바일_상점_구매취소");

                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileShop_CountSelect(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_상점_구매수"))
            {
                Touch("모바일_상점_구매수설정_클릭");
                Touch("모바일_상점_구매수설정확인_클릭");
                Touch("모바일_상점_구입_클릭");
                Touch("모바일_상점_구입_클릭");

                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileShop_Buy_Confirm(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_상점_구입완료"))
            {
                Touch("모바일_상점_구입확인_클릭");
                buyCount++;
                UStatus("영입이용권 " + buyCount + "/10 구매완료");

                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileScout_NeedEmpty(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_선수영입_공간필요"))
            {
                Touch("모바일_선수영입_공간필요닫기_클릭");
                UStatus("선수 자리 부족");
                MobileGo_Trade();
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileMain(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_메인화면"))
            {
                MobileGo_Scout();
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileRelease(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_팀관리"))
            {
                Touch("모바일_선수단_클릭");
                if (!ImageMatch(big, "모바일_선수단_소속정렬"))
                    Touch("모바일_선수단_소속정렬_클릭");
                else
                {
                    for (int i = 1; i < 8; i++)
                    {
                        Touch("모바일_선수단_선수" + i.ToString() + "_클릭");
                    }
                    Thread.Sleep(500);
                    big = Imaging.GetScreen();
                    big.Save("C:\\test.png");
                    if (ImageMatch(big, "모바일_선수단_방출"))
                    {
                        Touch("모바일_선수단_방출_클릭");
                    }
                    else
                    {
                        for (int i = 7; i > 0; i--)
                        {
                            Touch("모바일_선수단_선수" + i.ToString() + "_클릭");
                            Thread.Sleep(500);
                            big = Imaging.GetScreen();
                            if (ImageMatch(big, "모바일_선수단_방출"))
                            {
                                Touch("모바일_선수단_방출_클릭");
                                UStatus("더 이상 방출 할 수 있는 선수가 없습니다.");
                                MobileGo_Scout();
                                break;
                            }
                            else if (i == 1)
                            {
                                UStatus("더 이상 방출 할 수 있는 선수가 없습니다.");
                                MobileGo_Scout();
                            }
                        }
                    }
                }
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }

        private bool MobileRelease_Confirm(bool flag = false)
        {
            Bitmap big = Imaging.GetScreen();


            if (ImageMatch(big, "모바일_선수방출_방출"))
            {
                Touch("모바일_선수방출_방출_클릭");
                UStatus("방출완료");
                flag = true;
            }

            big.Dispose();
            big = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return flag;
        }


        private void MainMacro()
        {
            runFlag = true;

            if (MobileMain())
                errorCount = 0;
            else
                errorCount++;

            runFlag = false;

        }

        private void ReleaseMacro()
        {
            runFlag = true;
            if (MobileRelease())
                errorCount = 0;
            else if (MobileRelease_Confirm())
                errorCount = 0;
            else
                errorCount++;

            runFlag = false;
        }
        private void ScoutMacro()
        {
            runFlag = true;
            if (MobileScout())
                errorCount = 0;
            else if (MobileScout_NeedEmpty())
                errorCount = 0;
            else
                errorCount++;

            runFlag = false;
        }

        private void TradeMacro()
        {
            runFlag = true;
            if (MobileTrade())
                errorCount = 0;
            else if (MobileTrade_Continue())
                errorCount = 0;
            else if (MobileTrade_Open())
                errorCount = 0;
            else if (MobileTrade_Select())
                errorCount = 0;
            else if (MobileTrade_Renego())
                errorCount = 0;
            else if (MobileTrade_Result())
                errorCount = 0;
            else
                errorCount++;

            runFlag = false;
        }

        private void TradeMarketMacro()
        {
            runFlag = true;
            if (MobileMarket_Allget())
                errorCount = 0;
            else if (MobileTradeMarket_Allget_get())
                errorCount = 0;
            else if (MobileTradeMarket_Sell())
                errorCount = 0;
            else if (MobileTradeMarket_Sell_Confirm())
                errorCount = 0;
            else if (MobileTradeMarket_Sell_Over())
                errorCount = 0;
            else
                errorCount++;
            runFlag = false;
        }

        private void ShopMacro()
        {
            runFlag = true;
            if (MobileShop())
                errorCount = 0;
            else if (MobileShop_Buy())
                errorCount = 0;
            else if (MobileShop_CountSelect())
                errorCount = 0;
            else if (MobileShop_Buy_Confirm())
                errorCount = 0;
            else
                errorCount++;

            runFlag = false;
        }
        #endregion

        #region 이벤트
        void timer_Tick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                label4.Text = "러닝타임 : " + sw.Elapsed.ToString("hh\\:mm\\:ss");
            }));

            if (!runFlag)
            {
                if (errorCount > 60)
                {
                    UStatus("오류발생 감지. 감독재선택");
                    t = new Thread(MobileEscape, Int32.MaxValue);
                    t.Start();
                }
                else
                {
                    switch (status)
                    {
                        case MacroStatus.Main:
                            t = new Thread(MainMacro);
                            t.Start();
                            break;
                        case MacroStatus.Trade:
                            t = new Thread(TradeMacro);
                            t.Start();
                            break;
                        case MacroStatus.Scout:
                            t = new Thread(ScoutMacro);
                            t.Start();
                            break;
                        case MacroStatus.Shop:
                            t = new Thread(ShopMacro);
                            t.Start();
                            break;
                        case MacroStatus.TradeMarket:
                            t = new Thread(TradeMarketMacro);
                            t.Start();
                            break;
                        case MacroStatus.Release:
                            t = new Thread(ReleaseMacro);
                            t.Start();
                            break;
                        default:
                            t = new Thread(MainMacro);
                            t.Start();
                            break;
                    }
                }
            }
        }

        void btnCapture_Click(object sender, EventArgs e)
        {
            //Bitmap big = Imaging.GetScreen();
            //
            //big = Imaging.ConvertFormat(big, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            //big = grayscale(big);
            //big.Save(Environment.CurrentDirectory + "\\test2.png");
            mAdb.device = cboADBList.Text;
            mAdb.Capture();
            
            Clipboard.SetImage((System.Drawing.Image)mAdb.GetBitmap());
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

                Imaging.GetHWND();
                btnStart.Text = "중지";
                mAdb.device = cboADBList.Text;
                //t = new Thread(Macro, Int32.MaxValue);
                //t.Start();
                sw.Start();
                timer.Start();
            }
            else
            {
                btnRefresh.Enabled = true;
                cboADBList.Enabled = true;
                chkReceive.Enabled = true;

                btnStart.Text = "시작";
                t.Abort();
                sw.Stop();
                timer.Stop();
                runFlag = false;
                status = MacroStatus.Main;
                buyCount = 0;
                //timer.Stop();
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
            str = str.Replace("EP", "")
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
                    .Replace("f1", "8")
                    .Replace("f", "6")
                    .Replace("o", "0")
                    .Replace("o", "0")
                    .Replace("I", "1")
                    .Replace("b", "8");

            return str;
        }
        void UStatus(string msg)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                txtLog.AppendText("[" + GetServerTime().ToString("MM-dd HH:mm:ss") + "] " + msg);
                txtLog.AppendText("\r\n");
                txtLog.ScrollToCaret();

                string[] tempStr = txtLog.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                if (tempStr.Length > 100)
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = tempStr.Length - 99; i < tempStr.Length; i++)
                    {
                        sb.AppendLine(tempStr[i]);
                    }
                    txtLog.Text = sb.ToString();
                }
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
            big = Imaging.ConvertFormat(big, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //if (!flag)
            //big = Imaging.MakeGrayscale3(big);
            ////;
            //else
            //big = grayscale(big);

            //big.Save(Environment.CurrentDirectory + "\\test.png");
            ////tessnet2.Tesseract api = new tessnet2.Tesseract();

            ////api.Init()
            //OcrApi.PathToEngine = Environment.CurrentDirectory + @"\tesseract.dll";
            //OcrApi api = OcrApi.Create();
            //Languages[] lang = { Languages.English };
            //api.Init(lang, null, OcrEngineMode.OEM_TESSERACT_CUBE_COMBINED);
            //api.SetVariable("tessedit_char_whitelist", "0123456789");

            //string plainText = api.GetTextFromImage(big);
            //////Console.WriteLine(plainText);
            //////this.Invoke(new MethodInvoker(delegate() { txtLog.AppendText(plainText + Environment.NewLine); }));
            //api.Dispose();
            //api = null;
            //System.GC.Collect(0, GCCollectionMode.Forced);
            //System.GC.WaitForPendingFinalizers();

            //return plainText;
            var tmpFile = Path.GetTempFileName();
            string text = "";
            var bmp = new Bitmap(Math.Max(big.Width, 1024), Math.Max(big.Height, 768));
            var gfxResize = Graphics.FromImage(bmp);
            gfxResize.DrawImage(big, new Rectangle(0, 0, big.Width, big.Height));
            bmp.Save(tmpFile + ".png", ImageFormat.Png);

            //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L); // 0(최저화질 고압축) ~ 100L(최고화질 저압축)
            //myEncoderParameters.Param[0] = myEncoderParameter;
            //bmp.Save(tmpFile + ".jpg", jpgEncoder, myEncoderParameters);

            var doc = new MODI.Document();
            doc.Create(tmpFile + ".png");

            //Regex reg = new Regex(@"[^\d]+");
            try
            {
                doc.OCR(MODI.MiLANGUAGES.miLANG_ENGLISH, true, true);
                var img = (MODI.Image)doc.Images[0];
                var layout = img.Layout;
                text = layout.Text;
                File.Delete(tmpFile);
                File.Delete(tmpFile + ".png");
            }
            catch (Exception e)
            {
                try
                {
                    doc.OCR(MODI.MiLANGUAGES.miLANG_ENGLISH, false, false);
                    var img = (MODI.Image)doc.Images[0];
                    var layout = img.Layout;
                    text = layout.Text;
                }
                catch (Exception)
                {
                }
                finally
                {
                    File.Delete(tmpFile);
                    File.Delete(tmpFile + ".png");
                }

            }
            finally
            {
                File.Delete(tmpFile);
                File.Delete(tmpFile + ".png");
            }

            Int32 tryCheck = 0;
            text = Regex.Replace(text.Replace(" ", "").Replace(",", "").Replace(".", "").Trim(), @"[^\d]", "");
            Int32.TryParse(text, out tryCheck);
            if (tryCheck == 0 || tryCheck < 1000)
            {
                    try
                    {
                        
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                        tmpFile = Path.GetTempFileName();
                        text = "";
                        bmp = new Bitmap(Math.Max(big.Width, 1024), Math.Max(big.Height, 768));
                        gfxResize = Graphics.FromImage(bmp);
                        gfxResize.DrawImage(big, new Rectangle(0, 0, big.Width, big.Height));

                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L); // 0(최저화질 고압축) ~ 100L(최고화질 저압축)
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        bmp.Save(tmpFile + ".jpg", jpgEncoder, myEncoderParameters);

                        doc = new MODI.Document();
                        doc.Create(tmpFile + ".jpg");
                        doc.OCR(MODI.MiLANGUAGES.miLANG_ENGLISH, true, true);
                        var img = (MODI.Image)doc.Images[0];
                        var layout = img.Layout;
                        text = layout.Text;
                    }
                    catch (Exception)
                    {
                        doc.OCR(MODI.MiLANGUAGES.miLANG_ENGLISH, false, false);
                        var img = (MODI.Image)doc.Images[0];
                        var layout = img.Layout;
                        text = layout.Text;
                    }
                    finally
                    {
                        File.Delete(tmpFile);
                        File.Delete(tmpFile + ".jpg");
                    }
            }

            text = Regex.Replace(text.Replace(" ", "").Replace(",", "").Replace(".", "").Trim(), @"[^\d]", "");
            doc.Close();
            doc = null;
            bmp.Dispose();
            bmp = null;
            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            return text;
            //return "";
        }

        // Image format에 대한 Codec 정보를 가져온다.
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        //private void Monitoring()
        //{
        //    while (true)
        //    {
        //        //mAdb.Capture();
        //        //image.Image = mAdb.bmp;
        //        image.Image = Bitmap big = Imaging.GetScreen();
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
            return Imaging.ImgMatch(big, dictImage[destName]) != PNull;
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

        private void Escape(int sleepTime = 500)
        {
            mAdb.Escape();
            Thread.Sleep(sleepTime);
        }
        #endregion
    }
}
