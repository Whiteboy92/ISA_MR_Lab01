using System;
using Eto.Forms;
using Eto.Drawing;

namespace ISA_Ryba_Marcin
{
    public partial class MainForm : Form
    {
        private TextBox AInput;
        private TextBox BInput;
        private TextBox Input;
        private TextBox NInput;

        private DropDown DInput;

        private Button StartButton;

        private static void StartIsa()
        {
            
        }
        
        public MainForm()
        {
            Title = "INA Marcin Ryba";
            MinimumSize = new Size(800, 600);

            AInput = new TextBox()
            {
                Text = "-4",
            };
            
            BInput = new TextBox()
            {
                Text = "12",
            };
            
            NInput = new TextBox()
            {
                Text = "12",
            };

            DInput = new DropDown()
            {
                Items = { "1", "0.1", "0.01", "0.001" },
                SelectedIndex = 3
            };

            StartButton = new Button()
            {
                Text = "Start",
                Command = new Command((sender, eventArgs) => StartIsa()),
            };


            Content = new StackLayout
            {
                Padding = 10,
                Orientation = Orientation.Vertical,
                Items =
                {
                    new StackLayout()
                    {
                        VerticalContentAlignment = VerticalAlignment.Center,
                        AlignLabels = true,
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            new Label()
                            {
                                Text = "A: "
                            },
                            AInput,
                            new Panel()
                            {
                                Width = 50,
                            },
                            
                            new Label()
                            {
                                Text = "B: "
                            },
                            BInput,
                            new Panel()
                            {
                                Width = 50,
                            },
                            
                            new Label()
                            {
                                Text = "D: "
                            },
                            DInput,
                            new Panel()
                            {
                                Width = 50,
                            },
                            
                            new Label()
                            {
                                Text = "N: "
                            },
                            NInput,
                            new Panel()
                            {
                                Width = 50,
                            },
                            
                            StartButton,
                            new Panel()
                            {
                                Width = 50,
                            },
                        }
                    }
                }
            };
        }
    }
}