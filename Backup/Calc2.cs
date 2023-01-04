///////////////////////////////////////////////////////////////////////////////
//
//     http://www.interestprograms.ru - программы, игры и их исходные коды
//
///////////////////////////////////////////////////////////////////////////////



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace Calc2
{
    public partial class Calc2 : Form
    {
        public Calc2()
        {
            InitializeComponent();
            labelMainTablo.Text = null;
            labelMemory.Text = null;

            // Функциональные клавишы
            buttonAdd.Tag = FunctionKey.Add;
            buttonSubtract.Tag = FunctionKey.Subtract;
            buttonMultiply.Tag = FunctionKey.Multiply;
            buttonDivide.Tag = FunctionKey.Divide;
            buttonEqual.Tag = FunctionKey.Equal;
            buttonSqrt.Tag = FunctionKey.Sqrt;
            buttonPow2.Tag = FunctionKey.Pow2;
            buttonBackspace.Tag = FunctionKey.Backspace;

            // Клавишы набора чисел
            button_0.Tag = "0";
            button_1.Tag = "1";
            button_2.Tag = "2";
            button_3.Tag = "3";
            button_4.Tag = "4";
            button_5.Tag = "5";
            button_6.Tag = "6";
            button_7.Tag = "7";
            button_8.Tag = "8";
            button_9.Tag = "9";
            buttonSepDecimal.Tag = "SepDecimal";
            buttonSign.Tag = "Sign";

            // Сервисные клавишы
            buttonResetC.Tag = "ResetC";
            buttonResetCE.Tag = "ResetCE";
            buttonBackspace.Tag = "Backspace";


            buttonClipboard.Tag = "Clipboard"; 

            // Клавишы работы с памятью
            buttonTabloToMemory.Tag = "TabloToMemory";
            buttonMemoryToTablo.Tag = "MemoryToTablo";
            buttonMemoryAdd.Tag = "MemoryAdd";
            buttonMemorySubtract.Tag = "MemorySubtract";
            buttonMemoryMultiply.Tag = "MemoryMultiply";
            buttonMemoryDivide.Tag = "MemoryDivide";


            SeparatorDecimalBuffer = SeparatorDecimal;

           
        }

        

        enum CalcMode { P0, P1_1, P1_2, P2_1, P2_2, P2_3, P2_4 };
        CalcMode calcMode = CalcMode.P0;
        enum FunctionKey { Emptly, Add, Subtract, Multiply, Divide, Equal, Sqrt, Pow2, Backspace};
        FunctionKey functionKey;

        string OnTablo = "0";
        string TopRegister = null;
        string BottomRegister = null;
        string MemoryRegister = null;
        string Precision = "F2";
        string SeparatorDecimal = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
        string SeparatorDecimalBuffer = null;
        bool FractionalNumber = false;
        double FormOpacity = 1;
        

        private void buttonNumber_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            switch ((string)bt.Tag)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    string add = (string)bt.Tag;

                    switch (calcMode)
                    {
                        case CalcMode.P0:
                            TopRegister = null;
                            goto case CalcMode.P1_1;
                        case CalcMode.P1_1:
                            calcMode = CalcMode.P1_1;
                            NumberToTablo(add, ref TopRegister);
                            OnTablo = TopRegister;
                            break;
                        case CalcMode.P2_1:
                            calcMode = CalcMode.P1_2;
                            BottomRegister = null;
                            goto case CalcMode.P1_2;
                        
                        case CalcMode.P1_2:
                            NumberToTablo(add, ref BottomRegister);
                            OnTablo = BottomRegister;
                            break;
                    }
                    break;
                case "SepDecimal":
                    FractionalNumber = true;
                    switch (calcMode)
                    {
                        case CalcMode.P2_1:
                            if (BottomRegister == null) BottomRegister = "0";
                                OnTablo = BottomRegister;
                            break;
                    }
                    break;
                case "Sign":
                    double d = 0;
                    switch (calcMode)
                    {
                        case CalcMode.P1_1:
                        case CalcMode.P2_1:
                        case CalcMode.P2_4:
                            d = double.Parse(TopRegister);
                            d *= -1;
                            TopRegister = d.ToString();
                            OnTablo = TopRegister;
                            break;
                        case CalcMode.P1_2:
                            d = double.Parse(BottomRegister);
                            d *= -1;
                            BottomRegister = d.ToString();
                            OnTablo = BottomRegister;
                            break;
                    }
                    break;
            }

            

            
        }  
        
        private void buttonFunction_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            FractionalNumber = false;

            switch ((FunctionKey)button.Tag)
            {
                case FunctionKey.Add:
                case FunctionKey.Subtract:
                case FunctionKey.Multiply:
                case FunctionKey.Divide:
                    switch (calcMode)
                    {
                        case CalcMode.P1_1:
                        case CalcMode.P2_1:
                        case CalcMode.P2_4:
                            functionKey = (FunctionKey)button.Tag;
                            calcMode = CalcMode.P2_1;
                            break;
                        case CalcMode.P1_2:
                            DoEquals();
                            functionKey = (FunctionKey)button.Tag;
                            calcMode = CalcMode.P2_1;
                            break;
                    }
                    break;
                case FunctionKey.Equal:
                    switch (calcMode)
                    {
                        case CalcMode.P2_1:
                            if(BottomRegister == null)
                                BottomRegister = TopRegister;
                            goto case CalcMode.P2_3;
                        case CalcMode.P1_2:
                        case CalcMode.P2_3:
                        case CalcMode.P2_4:
                            calcMode = CalcMode.P2_4;
                            DoEquals();
                            break;
                    }
                    break;
                case FunctionKey.Sqrt:
                    double temp = 0;
                    switch (calcMode)
                    {
                        case CalcMode.P1_1:
                        case CalcMode.P2_4:
                            temp = double.Parse(TopRegister);
                            temp = Math.Pow(temp, 1.0 / 2.0);
                            TopRegister = temp.ToString(Precision);
                            OnTablo = TopRegister;
                            break;
                        case CalcMode.P1_2:
                            temp = double.Parse(BottomRegister);
                            temp = Math.Pow(temp, 1.0 / 2.0);
                            BottomRegister = temp.ToString(Precision);
                            OnTablo = BottomRegister;
                            break;
                    }
                    break;
                case FunctionKey.Pow2:
                    temp = 0;
                    switch (calcMode)
                    {
                        case CalcMode.P1_1:
                        case CalcMode.P2_4:
                            temp = double.Parse(TopRegister);
                            temp = Math.Pow(temp, 2.0);
                            TopRegister = temp.ToString(Precision);
                            OnTablo = TopRegister;
                            break;
                        case CalcMode.P1_2:
                            temp = double.Parse(BottomRegister);
                            temp = Math.Pow(temp, 2.0);
                            BottomRegister = temp.ToString(Precision);
                            OnTablo = BottomRegister;
                            break;
                    }
                    break;
            }
 
        }

        private void buttonService_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            switch ((string)bt.Tag)
            {
                case "ResetC":
                    calcMode = CalcMode.P0;
                    functionKey = FunctionKey.Emptly;
                    OnTablo = "0";
                    TopRegister = BottomRegister = null;
                    FractionalNumber = false;
                    break;
                case "ResetCE":
                    switch (calcMode)
                    {
                        case CalcMode.P1_1:
                            DoResetCE(ref TopRegister, CalcMode.P0);
                            break;
                        case CalcMode.P1_2:
                            DoResetCE(ref BottomRegister, CalcMode.P2_1);
                            break;
                    }
                    break;
                case "Backspace":
                    switch (calcMode)
                    {
                        case CalcMode.P1_1:
                            if (TopRegister.Length > 1)
                            {
                                DoBackspace(ref TopRegister);
                            }
                            else if (TopRegister.Length <= 1)
                            {
                                DoResetCE(ref TopRegister, CalcMode.P0);
                            }
                            break;
                        case CalcMode.P1_2:
                            if (BottomRegister.Length > 1)
                            {
                                DoBackspace(ref BottomRegister);
                            }
                            else
                            {
                                DoResetCE(ref BottomRegister, CalcMode.P2_1);
                            }
                            break;
                    }
                    break;
                case "Clipboard":
                    Clipboard.SetText(OnTablo.Replace(SeparatorDecimal, SeparatorDecimalBuffer), TextDataFormat.Text);
                    break;
            }
            
        }

        private void buttonMemoryFunction_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            switch ((string)bt.Tag)
            {
                case "TabloToMemory":
                    MemoryRegister = OnTablo;
                    break;
                case "MemoryToTablo":
                    if (MemoryRegister == null) MemoryRegister = "0";
                    switch (calcMode)
                    {
                        case CalcMode.P0:
                        case CalcMode.P1_1:
                            TopRegister = MemoryRegister;
                            OnTablo = TopRegister;
                            // calcMode = CalcMode.P1_1;
                            calcMode = CalcMode.P2_1;
                            break;
                        case CalcMode.P1_2:
                        case CalcMode.P2_1:
                            BottomRegister = MemoryRegister;
                            OnTablo = BottomRegister;
                            calcMode = CalcMode.P1_2;
                            break;
                        default:
                            TopRegister = MemoryRegister;
                            OnTablo = TopRegister;
                            break;
                    }
                    break;
                    
                case "MemoryAdd":
                case "MemorySubtract":
                case "MemoryMultiply":
                case "MemoryDivide":
                    DoFunctionMemory((string)bt.Tag);
                    calcMode = CalcMode.P0;
                    break;
            }
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            SetDlg setDlg = new SetDlg();
            setDlg.Precision = Precision;
            setDlg.SeparatorDecimalBuffer = SeparatorDecimalBuffer;
            setDlg.FormOpacity = FormOpacity;
            if (setDlg.ShowDialog() == DialogResult.OK)
            {
                Precision = setDlg.Precision;
                SeparatorDecimalBuffer = setDlg.SeparatorDecimalBuffer;
                FormOpacity = setDlg.FormOpacity;
            }
        }

       
        int count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (OnTablo.Length < 13)
            {
                labelMainTablo.Text = OnTablo;
            }
            else
            {
                double t = double.Parse(OnTablo);
                OnTablo = t.ToString("E10");
                labelMainTablo.Text = OnTablo;
            }
            

            if (MemoryRegister == "0") MemoryRegister = null;
            labelMemory.Text = MemoryRegister;

            

            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;

            if (x < (Location.X + Width) && x > Location.X && y < (Location.Y + Height) && y > Location.Y)
            {
                if (Opacity < 0.9)
                {
                    Opacity += 0.03;

                }
                else
                {
                    Opacity = 0.9999;
                }

                count = 0;

            }
            else
            {

                if (count > 100)
                {
                    if (Opacity > FormOpacity)
                        Opacity -= 0.005;

                }
                else
                {
                    count++;
                }
            }

        }

        private void Calc2_Load(object sender, EventArgs e)
        {
            FromFileIni();
        }

        private void Calc2_FormClosed(object sender, FormClosedEventArgs e)
        {
            ToFileIni();
        }

        
        
        
        #region  Вспомогательные функции 

        void NumberToTablo(string addnumber, ref string number)
        {
            if (FractionalNumber == false)
            {
                number += addnumber;
            }
            else
            {
                if (number == null)
                    number = "0";

                int i = number.IndexOf(SeparatorDecimal);
                if (i != -1)
                {
                    number += addnumber;
                }
                else
                {
                    number += SeparatorDecimal + addnumber;
                }

            }

        }

        private void DoEquals()
        {
            double tr = double.Parse(TopRegister);
            double br = double.Parse(BottomRegister);
            switch (functionKey)
            {
                case FunctionKey.Add:
                    OnTablo = (tr + br).ToString(Precision);
                    break;
                case FunctionKey.Subtract:
                    OnTablo = (tr - br).ToString(Precision);
                    break;
                case FunctionKey.Multiply:
                    OnTablo = (tr * br).ToString(Precision);
                    break;
                case FunctionKey.Divide:
                    OnTablo = (tr / br).ToString(Precision);
                    break;
            }

            TopRegister = OnTablo;
        }
        
        void DoBackspace(ref string registry)
        {
            registry = OnTablo.Substring(0, OnTablo.Length - 1);
            if (SeparatorDecimal == registry[registry.Length - 1].ToString())
                registry = OnTablo.Substring(0, OnTablo.Length - 2);

            OnTablo = registry;
        }

        void DoResetCE(ref string registry, CalcMode finalCalcMode)
        {
            calcMode = finalCalcMode;
            registry = null;
            OnTablo = "0";
        }

        void DoFunctionMemory(string function)
        {
            if (MemoryRegister == null) MemoryRegister = "0";
            double d = double.Parse(MemoryRegister);
            double ot = double.Parse(OnTablo);
            double res = d + ot;

            switch (function)
            {
                case "MemoryAdd":
                    res = d + ot;
                    break;
                case "MemorySubtract":
                    res = d - ot;
                    break;
                case "MemoryMultiply":
                    res = d * ot;
                    break;
                case "MemoryDivide":
                    res = d / ot;
                    break;
            }

            MemoryRegister = res.ToString();
        }

        void ToFileIni()
        {
            try
            {
                string filename = Application.StartupPath + "\\" + "setup.txt";
                StreamWriter sw = new StreamWriter(filename);
                sw.WriteLine(Precision);
                sw.WriteLine(SeparatorDecimalBuffer);
                sw.WriteLine(this.Location.X);
                sw.WriteLine(this.Location.Y);
                sw.WriteLine(FormOpacity);
                sw.Close();
            }
            catch
            {
                MessageBox.Show("Невозможно создать файл настройки!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        void FromFileIni()
        {
            

            string firstSymbol = null;
            string subPresition = null;
            int check = -1;
            try
            {
                string filename = Application.StartupPath + "\\" + "setup.txt";
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists == false) return;

                StreamReader sr = new StreamReader(filename);
                Precision = sr.ReadLine();
                SeparatorDecimalBuffer = sr.ReadLine();
                int x = int.Parse(sr.ReadLine());
                int y = int.Parse(sr.ReadLine());
                this.Location = new Point(x, y);
                FormOpacity = double.Parse(sr.ReadLine());
                sr.Close();


                firstSymbol = Precision.Substring(0, 1);
                subPresition = Precision.Substring(1, Precision.Length - 1);
                check = int.Parse(subPresition);
            }
            catch
            {
                MessageBox.Show("Файл настройки поврежден!\nВозврат к настройкам по умолчанию.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                if (firstSymbol != "F" ||
                    check > 8 ||
                    check < 0 ||
                    subPresition.Length > 1 ||
                    SeparatorDecimalBuffer.Length > 2)
                {
                    Precision = "F2";
                    SeparatorDecimalBuffer = SeparatorDecimal;
                }
            }

        }

        #endregion


      
       
    }
}
