using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Eto.Forms;
using Eto.Drawing;

namespace ISA_Ryba_Marcin
{
    public partial class MainForm : Form
    {
        private static TextBox AInput;
        private static TextBox BInput;
        private static TextBox NInput;

        private static DropDown DInput;

        private Button StartButton;

        private GridView OutputTable;
        
        

        private static void StartIsa()
        {
            if (!(
                    ParseHelper.ParseDouble(AInput.Text, "A", out double a) &&
                    ParseHelper.ParseDouble(BInput.Text, "B", out double b) &&
                    ParseHelper.ParseLong(NInput.Text, "N", out long n) &&
                    ParseHelper.ParseDouble(DInput.SelectedKey, "D", out double d, "en-US")))
            {
                return;
            }

            if (n > 0) return;
            MessageBox.Show("N should be bigger than 0!", MessageBoxType.Error);
            
            //Bits Number
            var l = (int) Math.Floor( Math.Log( (b - a) / d, 2) + 1.0);
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
                Text = "10",
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

            OutputTable = new GridView()
            {
                DataStore = new ObservableCollection<Specimen>(),
                Width = 800,
                Columns =
                {
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "Num",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.N.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xReal_1",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.xReal1.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xInt_1",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.xInt1.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xBin",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.xBin.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xInt_2",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.xInt2.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xReal_2",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.xReal2.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "F(x)",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.Fx.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                }
            };
            
            Content = new StackLayout
            {
                Padding = 10,
                Orientation = Orientation.Vertical,
                Items =
                {
                    new StackLayout(),
                    new Scrollable
                    {
                        Height = 300,
                        Content = OutputTable
                    }
                }
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
                        }
                    }
                }
            };
        }
    }
}