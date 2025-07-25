﻿using ESC_POS_USB_NET.Enums;
using ESC_POS_USB_NET.EpsonCommands;
using ESC_POS_USB_NET.Extensions;
using ESC_POS_USB_NET.Helper;
using ESC_POS_USB_NET.Interfaces.Command;
using ESC_POS_USB_NET.Interfaces.Printer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ESC_POS_USB_NET.Printer
{
    // main repo: https://github.com/mtmsuhail/ESC-POS-USB-NET
    // updated fork: https://github.com/salvadorjhai/ESC-POS-USB-NET
    public class Printer : IPrinter
    {
        private byte[] _buffer;
        private string _printerName;
        private readonly IPrintCommand _command;
        private string _codepage;

        public Printer()
        {
            _command = new EscPos();
            _codepage = "Windows-1257";
            SetCodePage(_codepage);
        }

        public Printer(string printerName, string codepage = "Windows-1257")
        {
            _printerName = string.IsNullOrEmpty(printerName) ? "escpos.prn" : printerName.Trim();
            _command = new EscPos();
            _codepage = codepage;
            SetCodePage(_codepage);
        }

        public Printer SetPrinterName(string printerName)
        {
            _printerName = string.IsNullOrEmpty(printerName) ? "escpos.prn" : printerName.Trim();
            return this;
        }
        public Printer SetCodePage(string codepage = "Windows-1257") {
            _codepage = codepage;
            PrinterExtensions.DefaultEncoding = Encoding.GetEncoding(_codepage);
            return this;
        }

        public int ColsNomal
        {
            get
            {
                return _command.ColsNomal;
            }
        }

        public int ColsCondensed
        {
            get
            {
                return _command.ColsCondensed;
            }
        }

        public int ColsExpanded
        {
            get
            {
                return _command.ColsExpanded;
            }
        }

        public byte[] Document()
        {
            return _buffer;
        }

        public void SetDocument(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer), "Buffer cannot be null");
            _buffer = buffer;
        }

        public void PrintDocument()
        {
            if (_buffer == null)
                return;

            if (string.IsNullOrEmpty(_printerName))
                throw new ArgumentException("Printer not set");

            if (!RawPrinterHelper.SendBytesToPrinter(_printerName, _buffer))
                throw new ArgumentException("Unable to access printer : " + _printerName);

        }

        public void Append(string value)
        {
            AppendString(value, true);
        }

        public void Append(byte[] value)
        {
            if (value == null)
                return;
            var list = new List<byte>();
            if (_buffer != null)
                list.AddRange(_buffer);
            list.AddRange(value);
            _buffer = list.ToArray();
        }

        public void AppendWithoutLf(string value)
        {
            AppendString(value, false);
        }

        private void AppendString(string value, bool useLf)
        {
            if (string.IsNullOrEmpty(value))
                return;
            if (useLf)
                value += "\n";
            var list = new List<byte>();
            if (_buffer != null)
                list.AddRange(_buffer);
            var bytes = Encoding.GetEncoding(_codepage).GetBytes(value);
            list.AddRange(bytes);
            _buffer = list.ToArray();
        }

        public void NewLine()
        {
            Append("\r");
        }

        public void NewLines(int lines)
        {
            for (int i = 1, loopTo = lines - 1; i <= loopTo; i++)
                NewLine();
        }

        public void Clear()
        {
            _buffer = null;
        }

        public void Separator(char speratorChar = '-')
        {
            Append(_command.Separator(speratorChar ));
        }

        public void AutoTest()
        {
            Append(_command.AutoTest());
        }

        public void TestPrinter()
        {
            Append("NORMAL - 48 COLUMNS");
            Append("1...5...10...15...20...25...30...35...40...45.48");
            Separator();
            Append("Text Normal");
            BoldMode("Bold Text");
            UnderlineMode("Underlined text");
            Separator();
            ExpandedMode(PrinterModeState.On);
            Append("Expanded - 23 COLUMNS");
            Append("1...5...10...15...20..23");
            ExpandedMode(PrinterModeState.Off);
            Separator();
            CondensedMode(PrinterModeState.On);
            Append("Condensed - 64 COLUMNS");
            Append("1...5...10...15...20...25...30...35...40...45...50...55...60..64");
            CondensedMode(PrinterModeState.Off);
            Separator();
            DoubleWidth2();
            Append("Font Width 2");
            DoubleWidth3();
            Append("Font Width 3");
            // ------------------
            DoubleHeight(); Append("Font DoubleHeight");
            DoubleWidth(); Append("Font DoubleWidth");
            DoubleSize(); Append("Font DoubleSize");
            Scale2xHeight(); Append("Font Scale2xHeight");
            Scale2x(); Append("Font Scale2x");
            Scale(2,2); Append("Scaled (2,2)");
            Scale(3,1); Append("Scaled (3,1)");
            Scale(4,3); Append("Scaled (4,3)");
            Scale(8,8); Append("Scaled (8,8)");
            // ------------------
            NormalWidth();
            Append("Normal width");
            NormalWidth();
            Append("Normal width");
            Separator();
            AlignRight();
            Append("Right aligned text");
            AlignCenter();
            Append("Center-aligned text");
            AlignLeft();
            Append("Left aligned text");
            Separator();
            Font("Font A", Fonts.FontA);
            Font("Font B", Fonts.FontB);
            Font("Font C", Fonts.FontC);
            Font("Font D", Fonts.FontD);
            Font("Font E", Fonts.FontE);
            Font("Font Special A", Fonts.SpecialFontA);
            Font("Font Special B", Fonts.SpecialFontB);
            Separator();
            //InitializePrint();
            SetLineHeight(24);
            Append("This is first line with line height of 30 dots");
            SetLineHeight(40);
            Append("This is second line with line height of 24 dots");
            Append("This is third line with line height of 40 dots");
            NewLines(3);
            Append("End of Test :)");
            Separator();
        }

        public void BoldMode(string value)
        {
            Append(_command.FontMode.Bold(value));
        }

        public void BoldMode(PrinterModeState state)
        {
            Append(_command.FontMode.Bold(state));
        }

        public void Font(string value, Fonts state)
        {
            Append(_command.FontMode.Font(value, state));
        }

        public void UnderlineMode(string value)
        {
            Append(_command.FontMode.Underline(value));
        }

        public void UnderlineMode(PrinterModeState state)
        {
            Append(_command.FontMode.Underline(state));
        }

        public void ExpandedMode(string value)
        {
            Append(_command.FontMode.Expanded(value));
        }

        public void ExpandedMode(PrinterModeState state)
        {
            Append(_command.FontMode.Expanded(state));
        }

        public void CondensedMode(string value)
        {
            Append(_command.FontMode.Condensed(value));
        }

        public void CondensedMode(PrinterModeState state)
        {
            Append(_command.FontMode.Condensed(state));
        }

        public void NormalWidth()
        {
            Append(_command.FontWidth.Normal());
        }

        public void DoubleWidth2()
        {
            Append(_command.FontWidth.DoubleWidth2());
        }

        public void DoubleWidth3()
        {
            Append(_command.FontWidth.DoubleWidth3());
        }

        // additional methods
        public void DoubleHeight() => Append(_command.FontWidth.DoubleHeight());
        public void DoubleWidth() => Append(_command.FontWidth.DoubleWidth());
        public void DoubleSize() => Append(_command.FontWidth.DoubleSize());
        public void Scale2xHeight() => Append(_command.FontWidth.Scale2xHeight());
        public void Scale2x() => Append(_command.FontWidth.Scale2x());
        public void Scale(int widthMultiplier, int heightMultiplier) => Append(_command.FontWidth.Scale(widthMultiplier, heightMultiplier));

        public void AlignLeft()
        {
            Append(_command.Alignment.Left());
        }

        public void AlignRight()
        {
            Append(_command.Alignment.Right());
        }

        public void AlignCenter()
        {
            Append(_command.Alignment.Center());
        }

        public void FullPaperCut()
        {
            Append(_command.PaperCut.Full());
        }

        public void PartialPaperCut()
        {
            Append(_command.PaperCut.Partial());
        }

        public void OpenDrawer()
        {
            Append(_command.Drawer.Open());
        }

        public void QrCode(string qrData)
        {
            Append(_command.QrCode.Print(qrData));
        }

        public void QrCode(string qrData, QrCodeSize qrCodeSize )
        {
            Append(_command.QrCode.Print(qrData, qrCodeSize));
        }

        public void Code128(string code, Positions printString = Positions.NotPrint)
        {
            Append(_command.BarCode.Code128(code,  printString));
        }

        public void Code39(string code, Positions printString=Positions.NotPrint)
        {
            Append(_command.BarCode.Code39(code,  printString));
        }

        public void Ean13(string code, Positions printString = Positions.NotPrint)
        {
            Append(_command.BarCode.Ean13(code,  printString));
        }

        public void InitializePrint()
        {
            RawPrinterHelper.SendBytesToPrinter(_printerName, _command.InitializePrint.Initialize());
        }

        public void Image(Bitmap image)
        {
            Append(_command.Image.Print(image));
        }
        public void NormalLineHeight()
        {
            Append(_command.LineHeight.Normal());
        }

        public void SetLineHeight(byte height)
        {
            Append(_command.LineHeight.SetLineHeight(height));
        }
    }
}

