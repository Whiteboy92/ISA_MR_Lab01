using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;

namespace ISA_Ryba_Marcin
{
	public sealed partial class MainForm : Form
	{
		private readonly TextBox _aInput;
		private readonly TextBox _bInput;
		private readonly DropDown _dInput;
		private readonly TextBox _nInput;
		private readonly GridView _outputTable;
		private readonly Slider _pkSlider;
		private readonly TextBox _pkValue;
		private readonly Slider _pmSlider;
		private readonly TextBox _pmValue;
		private readonly DropDown _targetFunctionDropdown;

		private readonly List<DataRow> _data = new List<DataRow>();

		private void SyncPkValueToSlider()
		{
			double val = _pkSlider.Value;
			_pkValue.Text = (val / 100_000_000.0).ToString("0.00");
		}

		private void SyncPmValueToSlider()
		{
			double val = _pmSlider.Value;
			_pmValue.Text = (val / 100_000_000.0).ToString("0.00");
		}


		private void StartIna()
		{
			if (!(
					FormatChecker.ParseDouble(_aInput.Text, "A", out double a) &&
					FormatChecker.ParseDouble(_bInput.Text, "B", out double b) &&
					FormatChecker.ParseLong(_nInput.Text, "N", out long n) &&
					FormatChecker.ParseDouble(_dInput.SelectedKey, "D", out double d, "en-US")
				)
				)
			{
				return;
			}

			if (n < 0)
			{
				MessageBox.Show("N should be bigger than 0!", MessageBoxType.Error);
			}

			if (_outputTable.DataStore == null)
			{
				MessageBox.Show("Collection is Null!");
				return;
			}

			((ObservableCollection<DataRow>)_outputTable.DataStore).Clear();

			SyncPkValueToSlider();
			SyncPmValueToSlider();

			int l = (int)Math.Floor( Math.Log((b - a) / d, 2) + 1.0);

			StaticValues.Pk = _pkSlider.Value / 100_000_000.0;
			StaticValues.Pm = _pmSlider.Value / 100_000_000.0;
			StaticValues.A = a;
			StaticValues.B = b;
			StaticValues.D = d;
			StaticValues.L = l;

			StaticValues.TargetFunction = _targetFunctionDropdown.SelectedKey switch
			{
				"MAX" => TargetFunction.Max,
				"MIN" => TargetFunction.Min
			};

			_data.Clear();

			
			for (int i = 0; i < n; i++)
			{
				var value = new Values();
				value.XReal1 = StaticValues.RandomXReal();
				value.XInt1 = MathHelper.XRealToXInt(value.XReal1);
				value.XBin = MathHelper.XIntToXBin(value.XInt1);
				value.XInt2 = MathHelper.XBinToXInt(value.XBin);
				value.XReal2 = MathHelper.XIntToXReal(value.XInt2);
				value.Fx = MathHelper.Fx(value.XReal1);

				var dataRow = new DataRow(value, i + 1);
				_data.Add(dataRow);
				((ObservableCollection<DataRow>)_outputTable.DataStore).Add(dataRow);
			}

			CalculateGx();
			CalculatePx();
			CalculateQx();
			Selection();
			Parenting();
			PairParents();
			RandomizePc();
			Fuck();
			Mutate();
			Final();
		}

		private void Final()
		{
			foreach (var row in _data)
			{
				row.FinalXRealValue = MathHelper.XBinToXReal(row.MutatedChromosomeValue);
				row.FinalFxRealValue = MathHelper.Fx(row.FinalXRealValue);
			}
		}

		private void Mutate()
		{
			for (int i = 0; i < _data.Count; i++)
			{
				_data[i].MutatedGenesValue = new List<int>();
				_data[i].MutatedChromosomeValue = "";
				string chromosome = _data[i].AfterChild.Item2;
				for (int bit = 0; bit < StaticValues.L; bit++)
				{
					if (StaticValues.Rand.NextDouble() < StaticValues.Pm)
					{
						_data[i].MutatedGenesValue.Add(bit);
						
						if (chromosome[bit] == '0')
						{
							_data[i].MutatedChromosomeValue += "1";
						}
						else
						{
							_data[i].MutatedChromosomeValue += "0";
						}
					}
					else
					{
						_data[i].MutatedChromosomeValue += chromosome[bit];
					}
				}
			}
		}


		private void Fuck()
		{
			for (int i = 0; i < _data.Count; i++)
			{
				var row = _data[i];
				if (row.ParentsWith == null) continue;

				if (row.PcValue != null)
				{
					string firstParentPart = row.SelectionXBin.Item2.Substring(0, row.PcValue.Value);
					const string spacer = " | ";
					string secondParentPart = row.ParentsWith.SelectionXBin.Item2.Substring(row.PcValue.Value);
					row.ChildXBin = firstParentPart + spacer + secondParentPart;
				}
			}
		}


		private void RandomizePc()
		{
			for (int i = 0; i < _data.Count; i++)
			{
				var row = _data[i];
				if (row.ParentsWith == null || row.PcValue != null) continue;

				int pc = 1 + (int)Math.Round(StaticValues.Rand.NextDouble() * (StaticValues.L - 2));
				row.PcValue = pc;
				row.ParentsWith.PcValue = pc;
				
				_outputTable.Invalidate();
			}
		}

		private void PairParents()
		{
			for (int i = 0; i < _data.Count; i++)
			{
				var row = _data[i];
				if (!row.IsParent || row.ParentsWith != null) continue;
				DataRow pair = null;
				if (i + 1 < _data.Count)
				{
					for (int j = i + 1; j < _data.Count; j++)
					{
						if (!_data[j].IsParent) continue;
						pair = _data[j];
						break;
					}
				}

				if (pair == null) continue;
				row.ParentsWith = pair;
				pair.ParentsWith = row;
			}
		}



		private void Parenting()
		{
			foreach (var row in _data)
			{
				row.ParentRandom = StaticValues.Rand.NextDouble();
				_outputTable.Invalidate();
			}
		}


		private void Selection()
		{
			foreach (var row in _data)
			{
				row.SelectionRandom = StaticValues.Rand.NextDouble();
				int selectedIndex = _data.Count - 1;
				for (int i = 0; i < _data.Count; i++)
				{
					if (!(_data[i].QxValue >= row.SelectionRandom)) continue;
					selectedIndex = i;
					break;
				}

				row.SelectionValue = _data[selectedIndex].OriginalSpecimen;
			}
		}

		private void CalculateGx()
		{
			switch (StaticValues.TargetFunction)
			{
				case TargetFunction.Max:
					double min = _data.Min(x => x.OriginalSpecimen.Fx);
					foreach (var dataRow in _data)
					{
						dataRow.GxValue = dataRow.OriginalSpecimen.Fx - min + StaticValues.D;
					}

					break;
				
				case TargetFunction.Min:
					double max = _data.Max(x => x.OriginalSpecimen.Fx);
					foreach (var dataRow in _data)
					{
						dataRow.GxValue = -(dataRow.OriginalSpecimen.Fx - max) + StaticValues.D;
					}
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void CalculatePx()
		{
			double sum = _data.Sum(x => x.GxValue);
			foreach (var dataRow in _data)
			{
				dataRow.PxValue = dataRow.GxValue / sum;
			}
		}

		private void CalculateQx()
		{
			double sum = 0.0;
			foreach (var dataRow in _data)
			{
				sum += dataRow.PxValue;
				dataRow.QxValue = sum;
			}
		}

		public MainForm()
		{
			Title = "ISA_Marcin_Ryba";
			Width = 1200;
			Height = 400;
			MinimumSize = new Size(1200, 400);
			Resizable = true;

			_aInput = new TextBox()
			{
				Text = "-4"
			};
			_bInput = new TextBox()
			{
				Text = "12"
			};
			_dInput = new DropDown()
			{
				Items = { "1", "0.1", "0.01", "0.001" },
				SelectedIndex = 3
			};
			_targetFunctionDropdown = new DropDown()
			{
				Items = { "MAX", "MIN" },
				SelectedIndex = 0
			};

			_nInput = new TextBox()
			{
				Text = "10"
			};
			var startButton = new Button()
			{
				Text = "Start",
				Command = new Command((sender, e) => StartIna())
			};

			_outputTable = _outputTable = new GridView()
			{
				DataStore = new ObservableCollection<DataRow>(),
				Width = 800
			};

			foreach (var property in typeof(DataRow).GetProperties())
			{
				if (property.PropertyType != typeof((string, string))) continue;

				_outputTable.Columns.Add(new GridColumn()
				{
					HeaderText = (((string, string))property.GetValue(DataRow.Empty)).Item1,
					DataCell = new TextBoxCell()
					{
						Binding = Binding.Property<DataRow, string>(x => (((string, string))property.GetValue(x)).Item2)
					}
				});
			}


			var outputTableScrollable = new Scrollable()
			{
				Height = 250,
				Content = _outputTable
			};

			_pkSlider = new Slider()
			{
				MinValue = 0,
				MaxValue = 100_000_000,
				Value = 50_000_000,
				Width = 100
			};
			_pkValue = new TextBox()
			{
				Text = (0.5).ToString("0.00"),
				Width = 50
			};

			_pkValue.KeyDown += (sender, args) =>
			{
				if (args.Key == Keys.Enter)
				{
					try
					{
						double val = double.Parse(_pkValue.Text);
						if (val > 1.0)
						{
							_pkValue.Text = 1.0.ToString("0.00");
							val = 1.0;
						}
						else if (val < 0)
						{
							_pkValue.Text = 0.0.ToString("0.00");
							val = 0.0;
						}
						_pkSlider.Value = (int)Math.Round(val * 100_000_000.0);
					}
					catch (Exception e)
					{
						SyncPkValueToSlider();
					}
				}
			};

			_pkSlider.ValueChanged += (sender, args) => SyncPkValueToSlider();

			_pmSlider = new Slider()
			{
				MinValue = 0,
				MaxValue = 100_000_000,
				Value = 2_000_000,
				Width = 100
			};
			_pmValue = new TextBox()
			{
				Text = 0.02.ToString("0.00"),
				Width = 50
			};

			_pmValue.KeyDown += (sender, args) =>
			{
				if (args.Key != Keys.Enter) return;
				try
				{
					double val = double.Parse(_pmValue.Text);
					switch (val)
					{
						case > 1.0:
							_pmValue.Text = 1.0.ToString("0.00");
							val = 1.0;
							break;
						case < 0:
							_pmValue.Text = 0.0.ToString("0.00");
							val = 0.0;
							break;
					}
					_pmSlider.Value = (int)Math.Round(val * 100_000_000.0);
				}
				catch (Exception e)
				{
					SyncPmValueToSlider();
				}
			};
			_pmSlider.ValueChanged += (sender, args) => SyncPmValueToSlider();



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
                            
                            new Label()
                            {
	                            Text = "Target Func"
                            },
                            _targetFunctionDropdown,
                            startButton,
                        }
                    },
                    outputTableScrollable,
                }
            };


			this.SizeChanged += (sender, args) =>
			{
				if (_outputTable != null)
				{
					_outputTable.Width = Width - 42;
				}

				outputTableScrollable.Width = Width - 40;
				outputTableScrollable.Height = Height - 100;
			};
		}
	}
}