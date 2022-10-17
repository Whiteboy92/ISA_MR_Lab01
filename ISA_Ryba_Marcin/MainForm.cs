using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Eto.Forms;
using Eto.Drawing;

namespace ISA_Ryba_Marcin
{
    public class MainForm : Form
    {
        private TextBox _aInput;
        private TextBox _bInput;
        private TextBox _nInput;

        private DropDown _dInput;

        private Button _startButton;

        private GridView _outputTable;
        
        

        private void StartIsa()
        {
            if (!(
                    ParseHelper.ParseDouble(_aInput.Text, "A", out double a) &&
                    ParseHelper.ParseDouble(_bInput.Text, "B", out double b) &&
                    ParseHelper.ParseLong(_nInput.Text, "N", out long n) &&
                    ParseHelper.ParseDouble(_dInput.SelectedKey, "D", out double d, "en-US")))
            {
                return;
            }

            if (n < 0)
            {
                MessageBox.Show("N should be bigger than 0!", MessageBoxType.Error);
            }

            ((ObservableCollection<Specimen>)_outputTable.DataStore).Clear();

            var rand = new Random();

            var accuracy = d switch
            {
                1.0 => 0,
                0.1 => 1,
                0.01 => 2,
                0.001 => 3,
                _ => throw new ArgumentOutOfRangeException()
            };

            //Bits Number
            var l = (int) Math.Floor( Math.Log( (b - a) / d, 2) + 1.0);
            var generation = new Specimen[n];
            
            for (var i = 0; i < n; i++)
            {
                var specimen = new Specimen
                {
                    N = i + 1,
                    XReal1 = Math.Round(rand.NextDouble() * (b - a) + a, accuracy)
                };
                specimen.XInt1 = (long) Math.Round((1.0 / (b - a)) * (specimen.XReal1 - a) * (Math.Pow(2.0, l) - 1.0));
                specimen.XBin = Convert.ToString(specimen.XInt1, 2).PadLeft(l, '0');
                specimen.XInt2 = Convert.ToInt64(specimen.XBin, 2);
                specimen.XReal2 = Math.Round(((b - a) * specimen.XInt2) / (Math.Pow(2.0,l) - 1.0) + a, accuracy);
                specimen.Fx = (specimen.XReal2 % 1.0) *
                              (Math.Cos(20.0 * Math.PI * specimen.XReal2 - Math.Sin(specimen.XReal2)));
                
                generation[i] = specimen;
                ((ObservableCollection<Specimen>)_outputTable.DataStore).Add(specimen);
            }
        }
        
        public MainForm()
        {
            Title = "INA Marcin Ryba";
            MinimumSize = new Size(840, 650);

            _aInput = new TextBox
            {
                Text = "-4",
            };
            
            _bInput = new TextBox
            {
                Text = "12",
            };
            
            _nInput = new TextBox
            {
                Text = "10",
            };

            _dInput = new DropDown
            {
                Items = { "1", "0.1", "0.01", "0.001" },
                SelectedIndex = 3
            };

            _startButton = new Button
            {
                Text = "Start",
                Command = new Command((sender, eventArgs) => StartIsa())
            };

            _outputTable = new GridView
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
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.XReal1.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xInt_1",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.XInt1.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xBin",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.XBin.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xInt_2",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.XInt2.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xReal_2",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Specimen, string>(specimen => specimen.XReal2.ToString(CultureInfo.CurrentCulture))
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
                    }
                }
            };
            
            
            Content = new StackLayout
            {
                Padding = 10,
                Orientation = Orientation.Vertical,
                Items =
                {
                    new StackLayout
                    {
                        VerticalContentAlignment = VerticalAlignment.Center,
                        AlignLabels = true,
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            new Label
                            {
                                Text = "A: "
                            },
                            _aInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 40
                            },
                            
                            new Label
                            {
                                Text = "B: "
                            },
                            _bInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 40
                            },
                            
                            new Label
                            {
                                Text = "D: "
                            },
                            _dInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 40
                            },
                            
                            new Label
                            {
                                Text = "N: "
                            },
                            _nInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 40
                            },
                            
                            _startButton,
                        }
                    },
                    new StackLayoutItem(),
                    new Scrollable()
                    {
                        Height = 550,
                        Content = _outputTable,
                    }
                }
            };
        }
    }
}