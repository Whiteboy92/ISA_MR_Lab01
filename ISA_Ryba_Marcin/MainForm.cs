using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Eto.Forms;
using Gdk;
using Size = Eto.Drawing.Size;

namespace ISA_Ryba_Marcin
{
    public class MainForm : Form
    {
        private readonly TextBox _aInput;
        private readonly TextBox _bInput;
        private readonly TextBox _nInput;

        private readonly Slider _pkSlider;
        private readonly TextBox _pkValue;
        private readonly Slider _pmSlider;
        private TextBox _pmValue;

        private readonly DropDown _dInput;
        private DropDown TargetFunctionDropdown;

        private readonly GridView _outputTable;

        private Scrollable OutputTableScrollable;

        private void SyncPkValueToSlider()
        {
            double value = _pkSlider.Value;
            _pkValue.Text = (value / 100000000.0).ToString("0.00");
        }
        
        private void SyncPmValueToSlider()
        {
            double value = _pmSlider.Value;
            _pmValue.Text = (value / 100000000.0).ToString("0.00");
        }
        
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
            MinimumSize = new Size(1550, 650);

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
                Width = 1520,
                Columns =
                {
                    new GridColumn
                    {
                        Width = 45,
                        HeaderText = "Num",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(values => values.N.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xReal_1",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(values => values.XReal1.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xInt_1",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(values => values.XInt1.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 150,
                        HeaderText = "xBin",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(values => values.XBin.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xInt_2",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(values => values.XInt2.ToString())
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 100,
                        HeaderText = "xReal_2",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(values => values.XReal2.ToString(CultureInfo.CurrentCulture))
                        }
                    },
                    
                    new GridColumn
                    {
                        Width = 155,
                        HeaderText = "F(x)",
                        DataCell = new TextBoxCell
                        {
                            Binding = Binding.Property<Values, string>(values => values.Fx.ToString(CultureInfo.CurrentCulture))
                        }
                    }
                }
            };

            _pkSlider = new Slider()
            {
                MinValue = 0,
                MaxValue = 100000000,
                Value = 50000000,
                Width = 110,
                ToolTip = "Crossing Probability"
            };

            _pkValue = new TextBox()
            {
                Text = (0.5).ToString("0.00"),
                Width = 50,
                ToolTip = "Crossing Probability"
            };

            _pkValue.KeyDown += (sender, eventArgs) =>
            {
                if (eventArgs.Key == Keys.Enter)
                    try
                    {
                        var value = double.Parse(_pkValue.Text);
                        switch (value)
                        {
                            case > 1.0:
                                _pkValue.Text = 1.0.ToString("0.00");
                                value = 1.0;
                                break;
                            case < 0:
                                _pkValue.Text = 0.0.ToString("0.00");
                                value = 0.0;
                                break;
                        }

                        _pkSlider.Value = (int) Math.Round(value * 100000000.0);
                    }
                    catch (Exception exception)
                    {
                        SyncPkValueToSlider();
                    }
            };

            _pkSlider.ValueChanged += (sender, eventArgs) => SyncPkValueToSlider();
            
            _pmSlider = new Slider()
            {
                MinValue = 0,
                MaxValue = 100000000,
                Value = 2000000,
                Width = 110,
            };
            
            _pmValue = new TextBox()
            {
                Text = (0.02).ToString("0.00"),
                Width = 50,
            };

            if (_pmValue != null)
                _pmValue.KeyDown += (sender, eventArgs) =>
                {
                    if (eventArgs.Key == Keys.Enter)
                        try
                        {
                            var value = double.Parse(_pmValue.Text);
                            switch (value)
                            {
                                case > 1.0:
                                    _pmValue.Text = 1.0.ToString("0.00");
                                    value = 1.0;
                                    break;
                                case < 0:
                                    _pmValue.Text = 0.0.ToString("0.00");
                                    value = 0.0;
                                    break;
                            }

                            _pmSlider.Value = (int) Math.Round(value * 100000000.0);
                        }
                        catch (Exception exception)
                        {
                            SyncPkValueToSlider();
                        }
                };

            _pmSlider.ValueChanged += (sender, eventArgs) => SyncPmValueToSlider();
            
            OutputTableScrollable = new Scrollable()
            {
                Height = 550,
                Content = _outputTable,
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
                            
                            new Label
                            {
                                Text = "PK: "
                            },
                            _pmSlider,
                            new Panel
                            {

                                Height = 40
                            },
                            
                            _pkSlider,
                            _pkValue,
                            
                            new Label
                            {
                                Text = "PM: "
                            },
                            _pmSlider,
                            new Panel
                            {

                                Height = 40
                            },
                            
                            _pmSlider,
                            _pmValue,
                            startButton,
                        }
                    },
                    OutputTableScrollable,
                }
            };
        }
    }
}