﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace customMessageBox
{
    public enum MessageBoxType { Information, Error, Warning, YesNo }
    public enum MessageBoxResults { Close, Yes, No }

    public partial class MyMessageBox : Form
    {

        enum ButtonType { Close, Yes, No }
        private int numberOfButtons = 1;    //ustawienia wstępnie zainicjalizowane, nadpisywane w zależności od rodzaju MessageBoxa

        //wypełniane podczas określania zawartości MessageBoxa w zależności od jego typu
        private string[] buttonText = new string[2];
        private ButtonType[] mbButtons = new ButtonType[2]; //zmienić, jeżeli będzie potrzeba tworzenie MessageBoxa z większą liczbą buttonów

        private MessageBoxResults mbResult = MessageBoxResults.Close;
        private MessageBoxType mbType;
        private string message;
        private int stdButtonWidth = 75;
        private int stdButtonHeigth = 23;
        private int buttonHorizontalPadding = 15;
        private int buttonVerticalPadding = 15; //odstęp powyżej i poniżej przycisku
        private int formVerticalPadding = 50;   //suma obrzeży (górnego i dolnego) formatki
        private int formControlsWidth = 170;   //szerokość przycisków minimalizacji i zamykania formatki po prawej plus ikona po lewej

        public MyMessageBox(string message, MessageBoxType mbType)
        {
            InitializeComponent();
            this.message = message;
            this.mbType = mbType;
            setFormLayout();
        }

        public static MessageBoxResults display(string message, MessageBoxType mbType = MessageBoxType.Information)
        {
            MyMessageBox mmb = new MyMessageBox(message, mbType);
            mmb.textBox1.Text = mmb.message;
            mmb.ShowDialog();
            mmb.BringToFront();
            return mmb.mbResult;
        }

        private void setFormLayout()
        {
            getMBParameters();
            setTextBoxSize();
            setFormSize();
            setGroupBoxSize();
            generateButtons();
        }

        private void getMBParameters()
        {
            switch (mbType)
            {
                case MessageBoxType.Information:
                    numberOfButtons = 1;
                    buttonText[0]="OK";
                    mbButtons[0] = ButtonType.Close;
                    this.Text = "Informacja";
                    break;
                case MessageBoxType.Error:
                    numberOfButtons = 1;
                    buttonText[0] = "OK";
                    mbButtons[0] = ButtonType.Close;
                    this.Text = "Błąd";
                    break;
                case MessageBoxType.Warning:
                    numberOfButtons = 1;
                    buttonText[0] = "OK";
                    mbButtons[0] = ButtonType.Close;
                    this.Text = "Ostrzeżenie";
                    break;
                case MessageBoxType.YesNo:
                    numberOfButtons = 2;
                    buttonText[0] = "Tak"; buttonText[1] = "Nie";
                    mbButtons[0] = ButtonType.Yes; mbButtons[1] = ButtonType.No;
                    this.Text = "Decyzja";
                    break;
            }
        }

        private void setGroupBoxSize()
        {
            groupBox1.Height = stdButtonHeigth + 2 * buttonVerticalPadding;
            groupBox1.Width = this.Width;
            groupBox1.Location = new Point(0, textBox1.Height + buttonVerticalPadding);

        }

        private void setFormSize()
        {
            //określam minimalną szerokość formularza ze względu na liczbę buttonów
            int minGroupBoxWidth = numberOfButtons * (stdButtonWidth + buttonHorizontalPadding) + buttonHorizontalPadding;

            //określam minimalną szerokość formularza ze względu na długość nagłówka
            //szerokość formatki nie może być tak mała, że nagłówek się nie mieści
            int labelLength = TextRenderer.MeasureText(this.Text, this.Font).Width;
            int minLabelWidth = labelLength + formControlsWidth;

            int minFormWidth = Math.Max(minLabelWidth, minGroupBoxWidth);

            if (textBox1.Width > minFormWidth)
            {
                this.Width = textBox1.Width + 15; //dodaję odstęp tekstu od brzegu formatki po prawej
            }
            else
            {
                this.Width = minFormWidth;
            }

            int groupBoxHeigth = stdButtonHeigth + 3 * buttonVerticalPadding;

            this.Height = textBox1.Height + groupBoxHeigth + formVerticalPadding;

            //rozmiary formatki są niezmienialne po uruchomieniu
             this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private int[] getTextBoxSize()
        {
            int[] textBoxSize = new int[2]; //najpierw szerokość później wysokość

            //gdyby message był pustym stringiem funkcja split wywali błąd, ustalam więc minimalne wymiary w takiej sytuacji
            if (message.Equals(""))
            {
                textBoxSize[0] = 10;
                textBoxSize[1] = 10;
            }
            else
            {
                string[] splitMessage = message.Split(new[] { "\r\n" }, StringSplitOptions.None);

                //obliczam szerokość
                int maxLineSize = 0;
                foreach (string line in splitMessage)
                {
                    int lineSize = TextRenderer.MeasureText(line, textBox1.Font).Width;

                    if (lineSize > maxLineSize)
                    {
                        maxLineSize = lineSize;
                    }
                }
                //obliczam wysokość
                int numberOfLines = splitMessage.Length;
                int heigth = numberOfLines * textBox1.Font.Height;

                //kompiluję wynik końcowy, dodaję po wysokości fontów do obliczonych wartości bo inaczej tekst mieści się na styk
                //w obliczonej ramce i w polu tekstowym pojawiają się paski przewijania (scrollbars)
                textBoxSize[0] = maxLineSize + textBox1.Font.Height;
                textBoxSize[1] = heigth + textBox1.Font.Height;
            }
            return textBoxSize;
        }

        private void setTextBoxSize()
        {
            int[] size = getTextBoxSize();
            textBox1.Width = size[0];
            textBox1.Height = size[1];
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void generateButtons()
        {
            
            //odległość buttona od brzegu groupboxa
            int buttonSpacing = (groupBox1.Width - numberOfButtons * (stdButtonWidth + buttonHorizontalPadding)) / 2;

            for (int i = 0; i < numberOfButtons; i++)
            {
                Button button = new Button();
                button.Size = new Size(stdButtonWidth, stdButtonHeigth);
                Point newLoc = new Point(buttonSpacing + i * (button.Width + buttonHorizontalPadding), button.Location.Y + buttonVerticalPadding);
                button.Location = newLoc;
                button.Click += new EventHandler(ButtonClick);
                button.Tag = mbButtons[i];
                button.Text = buttonText[i];
                Controls.Add(button);
                groupBox1.Controls.Add(button);

            }
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                // now you know the button that was clicked
                switch (button.Tag)
                {
                    case ButtonType.Close:
                        this.Close();
                        break;
                    case ButtonType.Yes:
                        mbResult = MessageBoxResults.Yes;
                        this.Close();
                        break;
                    case ButtonType.No:
                        mbResult = MessageBoxResults.No;
                        this.Close();
                        break;
                        
                }
            }
        }
    }
}
