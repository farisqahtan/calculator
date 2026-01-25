using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace calc
{
    public partial class Form1 : Form
    {
        double result = 0;
        int open, close = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        void clear()
        {
            textBox1.Clear();
            label1.Text = "0";
            result = 0;
            open = 0;
            close = 0;
            label1.ForeColor = Color.White;
            button10.Enabled = true;
        }

        void button_numbers(string number) // code of the numbers button
        {
            if (textBox1.Text == "")
            {
                textBox1.Text += number;
            }
            else if (label1.Text.Length > 1 && char.IsDigit(textBox1.Text[textBox1.Text.Length - 1]))
            {
                clear();
                textBox1.Text += number;
            }
            else if (textBox1.Text[textBox1.Text.Length - 1] != ')')
            {
                textBox1.Text += number;
            }
            this.ActiveControl = null; // يمنع التركيز على الزر عند الضغط عليه 
        }

        double Equal(string input)
        {
            if (input == "" || (!char.IsDigit(input[input.Length - 1]) && input[input.Length - 1] != ')') || open != close) // اذا كان فاضي او اخر حرف لم يكن رقم ولا يساوي تسكيرة القوس
            {
                label1.Text = "Format Error";
                label1.ForeColor = Color.DarkRed;
                open = 0;
                close = 0;
                result = 0;
                button10.Enabled = true;
                return result;
            }

            List<double> number = new List<double>();
            List<char> op = new List<char>();
            string textReslut = input;
            string counter = "";
            int index_open = 0;
            int index_close = 0;

            // (5+(3+2))+2 
            for (int i = 0; i < textReslut.Length; i++)
            {
                if (textReslut[i] == '(')
                {
                    index_open = i; // save index of openbracket
                    int count_open = 1;
                    int count_close = 0;
                    int j = ++i;
                    for (;; j++)
                    {
                        if (textReslut[j] == '(')
                        {
                            count_open++;
                        }
                        if (textReslut[j] == ')')
                        {
                            count_close++;
                            if (count_close < count_open)
                            {
                                counter += textReslut[j];
                                continue;
                            }
                            else
                            {
                                index_close = j; // save index of closebracket
                                break;
                            }
                        }
                        counter += textReslut[j];
                    }
                    textReslut = textReslut.Remove(index_open, index_close - index_open + 1);
                    double r = Equal(counter);
                    textReslut = textReslut.Insert(index_open, r.ToString());
                    counter = "";
                }
            }

            for (int i = 0; i < textReslut.Length; i++)
            {
                if (textReslut[i] == '-' && i == 0 || (textReslut[i] == '-' && !char.IsDigit(textReslut[i - 1]))) // اذا لقى - في البداية او بعد رمز مثل (×و÷) يتعامل معاه كسالب
                {
                    if (textReslut[i + 1] == '-') i++; // in case --6 or ---6
                    else
                    {
                        counter += textReslut[i];
                        counter += textReslut[i + 1];
                        i++;
                    }
                }
                else if (char.IsDigit(textReslut[i]) || textReslut[i] == '.' || textReslut[i] == 'E' || (textReslut[i] == '+' && textReslut[i - 1] == 'E') ) // if for numbers and dots
                {
                    counter += textReslut[i];
                }
                else // if for +, *, /, -
                {
                    op.Add(textReslut[i]);
                    number.Add(Convert.ToDouble(counter));
                    counter = "";
                }
            }
            number.Add(Convert.ToDouble(counter));
            if (number.Count == 1) return number[0];

            // for loop for  × , ÷ 
            for (int i = 0; i < op.Count; i++)
            {
                switch (op[i])
                {
                    case '×':
                        number[i + 1] = number[i] * number[i + 1];
                        number.RemoveAt(i);
                        op.RemoveAt(i);
                        i--;
                        break;
                    case '÷':
                        if (number[i + 1] == 0)
                        {
                            label1.Text = "Cannot divide by zero";
                            return result;
                        }
                        else
                        {
                            number[i + 1] = number[i] / number[i + 1];
                            number.RemoveAt(i);
                            op.RemoveAt(i);
                            i--;
                        }
                        break;
                }
            }

            if (op.Count == 0 && number.Count == 1) // if operation all multiple or divison like (4*2*3*5), in the end will be number.count = 1, op.count = 0
            {
                result = number[0];
            }
            else if (op.Count == 0 && number.Count > 1) // (-5-5) will be number = [-5, -5] and op.count = 0 then add sum
            {
                for (int i = 0; i < number.Count; i++)
                {
                    result += number[i];
                }
            }

            // for loop for  + , -
            for (int i = 0; i < op.Count; i++)
            {
                switch (op[i]) // يتأكد من العملية
                {
                    case '+':
                        result = number[i] + number[i + 1];
                        number[i + 1] = result;
                        break;
                    case '-':
                        number[i + 1] *= -1;
                        result = number[i] + number[i + 1];
                        number[i + 1] = result;
                        break;
                }
            }

            if (input.Contains('.'))
            {
                button10.Enabled = false;
            }

            return result;
            //result = 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
        }

        private void button0_Click(object sender, EventArgs e)
        {
            button_numbers(button0.Text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button_numbers(button1.Text);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button_numbers(button2.Text);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            button_numbers(button3.Text);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            button_numbers(button4.Text);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            button_numbers(button5.Text);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            button_numbers(button6.Text);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            button_numbers(button7.Text);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            button_numbers(button8.Text);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            button_numbers(button9.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox1.Text[textBox1.Text.Length - 1] != '.')
            {
                textBox1.Text += button10.Text;
                button10.Enabled = false;
            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;

            char lastElem = textBox1.Text[textBox1.Text.Length - 1];

            //if (lastElem != '.') // to prevent enable point like 11. -> click +

            if (char.IsDigit(lastElem) || lastElem == ')')
            {
                textBox1.Text += button12.Text;
                label1.Text = "0";
                label1.ForeColor = Color.White;
                button10.Enabled = true;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            double result = Equal(textBox1.Text);
            if (label1.Text.Length == 1)
            {
                label1.Text = (textBox1.Text + " = " + Convert.ToString(result));
            }
            textBox1.Text = Convert.ToString(result); // لازم يحول النتيجة لنص عشان يعرضها في مربع النص
        }
        private void button13_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text += button13.Text;
            }
            else
            {
                char lastElem = textBox1.Text[textBox1.Text.Length - 1];
                if (lastElem != '.')
                {
                    button10.Enabled = true;
                }
                if (char.IsDigit(lastElem) || lastElem == '×' || lastElem == '÷' || lastElem == ')' || lastElem == '(')
                {
                    label1.Text = "0";
                    label1.ForeColor = Color.White;
                    textBox1.Text += button13.Text;
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            clear();
            button10.Enabled = true;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;
            char lastElem = textBox1.Text[textBox1.Text.Length - 1];
            if (lastElem == '.')
            {
                button10.Enabled = true;
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                return;
            }
            if (lastElem == ')')
            {
                close--;
            }
            else if (lastElem == '(')
            {
                open--;
            }

            string counter = "";
            if (lastElem == '+' || lastElem == '-' || lastElem == '×' || lastElem == '÷' || lastElem == ')' || lastElem == '(')
            {
                for (int i = textBox1.Text.Length - 2; i > -1 && (textBox1.Text[i] != '×' || textBox1.Text[i] != '÷' || textBox1.Text[i] != '+' || textBox1.Text[i] != '-'); i--)
                {
                    if (textBox1.Text[i] == '×' || textBox1.Text[i] == '÷') // 5.5 ×-
                    {
                        if (counter.Contains(".")) button10.Enabled = false;
                        textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                        return;
                    }
                    if (char.IsDigit(textBox1.Text[i]) || textBox1.Text[i] == '.')
                    {
                        counter += textBox1.Text[i];
                    }
                    else // 5 *- 5.5 +
                    {
                        if (counter.Contains(".")) // -6+ 
                        {
                            textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                            button10.Enabled = false;
                            counter = "";
                            return;
                        }
                        else // -.6-6-
                        {
                            textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                            button10.Enabled = true;
                            counter = "";
                            return;
                        }
                    }
                }

                if (counter.Contains("."))
                {
                    textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                    button10.Enabled = false;
                    counter = "";
                    return;
                }
            }
            textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;

            char lastElem = textBox1.Text[textBox1.Text.Length - 1];
            if (char.IsDigit(lastElem) || lastElem == ')')
            {
                label1.Text = "0";
                label1.ForeColor = Color.White;
                textBox1.Text += button14.Text;
                button10.Enabled = true;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;

            char lastElem = textBox1.Text[textBox1.Text.Length - 1];
            if (char.IsDigit(lastElem) || lastElem == ')')
            {
                label1.Text = "0";
                label1.ForeColor = Color.White;
                textBox1.Text += button15.Text;
                button10.Enabled = true;
            }
        }
        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // حالات الضغط على الارقام
                // check if the user press digit0 (D0) or number 0 in the pad (NumPad0) in keyboard
                case Keys.D0:
                case Keys.NumPad0:
                    if (e.Shift)
                    {
                        button18.PerformClick();
                        break;
                    }
                    button0.PerformClick();
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    button1.PerformClick();
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    button2.PerformClick();
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    button3.PerformClick();
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    button4.PerformClick();
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    button5.PerformClick();
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    button6.PerformClick();
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    button7.PerformClick();
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    button8.PerformClick();
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                    if (e.Shift)
                    {
                        button18.PerformClick();
                        break;
                    }
                    button9.PerformClick();
                    break;
                //----------------
                // حالات الضغط على العمليات
                // check if the user press (oemplus) or add button in the pad (add) in keyboard
                case Keys.Oemplus:
                case Keys.Add:
                    button12.PerformClick();
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    button13.PerformClick();
                    break;
                case Keys.Multiply:
                    button14.PerformClick();
                    break;
                case Keys.Divide:
                    button15.PerformClick();
                    break;
                case Keys.Enter:
                    button11.PerformClick();
                    break;
                case Keys.Back:
                    button16.PerformClick();
                    break;
                case Keys.Escape:
                    button17.PerformClick();
                    break;
                case Keys.Decimal:
                case Keys.OemPeriod:
                    button10.PerformClick();
                    break;
            }
        }

        private void label1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text += '(';
                open++;
                return;
            }
            
            char lastElem = textBox1.Text[textBox1.Text.Length - 1];

            if (lastElem == '+' || lastElem == '-' || lastElem == '×' || lastElem == '÷' || lastElem == '(' /*|| (char.IsDigit(lastElem) && open == close)*/)
            {
                textBox1.Text += '(';
                open++;
                button10.Enabled = true;
            }
            else if (char.IsDigit(lastElem) && close < open || lastElem == ')' && close < open)
            {
                textBox1.Text += ')';
                close++;
                button10.Enabled = true;
            }
        }
    }
}