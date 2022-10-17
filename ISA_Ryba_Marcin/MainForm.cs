using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Eto.Forms;
using Eto.Drawing;

namespace ISA_Ryba_Marcin
{
    public class MainForm : Form
    {
        private readonly TextBox _aInput;
        private readonly TextBox _bInput;
        private readonly TextBox _nInput;

        private readonly DropDown _dInput;

        private readonly GridView _outputTable;

        private void StartMath()
        {
            if (!(
                    FormatChecker.ParseDouble(_aInput.Text, "A", out double a) &&
                    FormatChecker.ParseDouble(_bInput.Text, "B", out double b) &&
                    FormatChecker.ParseLong(_nInput.Text, "N", out long n) &&
                    FormatChecker.ParseDouble(_dInput.SelectedKey, "D", out double d, "en-US")))
            {
                return;
            }

            if (n < 0)
            {
                MessageBox.Show("N should be bigger than 0!", MessageBoxType.Error);
            }

            ((ObservableCollection<Values>)_outputTable.DataStore).Clear();

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
            var generation = new Values[n];
            
            for (var i = 0; i < n; i++)
            {
                var value = new Values
                {
                    N = i + 1,
                    XReal1 = Math.Round(rand.NextDouble() * (b - a) + a, accuracy)
                };
                value.XInt1 = (long) Math.Round((1.0 / (b - a)) * (value.XReal1 - a) * (Math.Pow(2.0, l) - 1.0));
                value.XBin = Convert.ToString(value.XInt1, 2).PadLeft(l, '0');
                value.XInt2 = Convert.ToInt64(value.XBin, 2);
                value.XReal2 = Math.Round(((b - a) * value.XInt2) / (Math.Pow(2.0,l) - 1.0) + a, accuracy);
                value.Fx = (value.XReal2 % 1.0) *
                              (Math.Cos(20.0 * Math.PI * value.XReal2 - Math.Sin(value.XReal2)));
                
                generation[i] = value;
                ((ObservableCollection<Values>)_outputTable.DataStore).Add(value);
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

            var startButton = new Button
            {
                Text = "Start",
                Command = new Command((sender, eventArgs) => StartMath())
            };

            _outputTable = new GridView
            {
                DataStore = new ObservableCollection<Values>(),
                Width = 800,
                Columns =
                {
                    new GridColumn
                    {
                        Width = 65,
                        HeaderText = "Num",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(specimen => specimen.N.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xReal_1",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(specimen => specimen.XReal1.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xInt_1",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(specimen => specimen.XInt1.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 150,
                        HeaderText = "xBin",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(specimen => specimen.XBin.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xInt_2",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(specimen => specimen.XInt2.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xReal_2",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(specimen => specimen.XReal2.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 155,
                        HeaderText = "F(x)",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(specimen => specimen.Fx.ToString(CultureInfo.CurrentCulture))
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
                                Text = "a: "
                            },
                            _aInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 40
                            },
                            
                            new Label
                            {
                                Text = "b: "
                            },
                            _bInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 40
                            },
                            
                            new Label
                            {
                                Text = "d: "
                            },
                            _dInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 40
                            },
                            
                            new Label
                            {
                                Text = "n: "
                            },
                            _nInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 40
                            },
                            
                            startButton,
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