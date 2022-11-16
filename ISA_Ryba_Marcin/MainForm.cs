using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace ISA_Ryba_Marcin
{
	public sealed class MainForm : Form
	{
		private readonly TextBox _aInput;
		private readonly TextBox _bInput;
		private readonly TextBox _nInput;
		private readonly TextBox _pkInput;
		private readonly TextBox _pmInput;
		
		private readonly GridView _outputTable;
		
		private readonly DropDown _dInput;
		private readonly DropDown _targetFunctionDropdown;
		
		private readonly List<DataRow> _data = new();
		
		//Start Calculations
		private void StartIna()
		{
			if (!(
					FormatChecker.ParseDouble(_aInput.Text, "A", out double a) &&
					FormatChecker.ParseDouble(_bInput.Text, "B", out double b) &&
					FormatChecker.ParseLong(_nInput.Text, "N", out long n) &&
					FormatChecker.ParseDouble(_dInput.SelectedKey, "D", out double d, "en-US") &&
					FormatChecker.ParseDouble(_pkInput.Text, "PK", out double pk) &&
					FormatChecker.ParseDouble(_pmInput.Text, "PM", out double pm) 
					
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

			int l = (int)Math.Floor( Math.Log((b - a) / d, 2) + 1.0);

			StaticValues.Pk = pk;
			StaticValues.Pm = pm;
			StaticValues.A = a;
			StaticValues.B = b;
			StaticValues.D = d;
			StaticValues.L = l;

			StaticValues.TargetFunction = _targetFunctionDropdown.SelectedKey switch
			{
				"MAX" => TargetFunction.Max,
				"MIN" => TargetFunction.Min,
				_ => throw new ArgumentOutOfRangeException()
			};

			_data.Clear();
			
			for (var i = 0; i < n; i++)
			{
				var value = new Values
				{
					XReal1 = StaticValues.RandomXReal()
				};
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
			Select();
			GetParents();
			PairParents();
			RandomizePc();
			MakeBabies();
			Mutate();
			Final();
		}
		
		private void SetMyButtonProperties()
		{
			// Give the button a flat appearance.
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
			foreach (var dataRow in _data)
			{
				dataRow.MutatedGenesValue = new List<int>();
				dataRow.MutatedChromosomeValue = "";
				string chromosome = dataRow.AfterChild.Item2;
				for (var bit = 0; bit < StaticValues.L; bit++)
				{
					if (StaticValues.Rand.NextDouble() < StaticValues.Pm)
					{
						dataRow.MutatedGenesValue.Add(bit);
						
						if (chromosome[bit] == '0')
						{
							dataRow.MutatedChromosomeValue += "1";
						}
						else
						{
							dataRow.MutatedChromosomeValue += "0";
						}
					}
					else
					{
						dataRow.MutatedChromosomeValue += chromosome[bit];
					}
				}
			}
		}


		private void MakeBabies()
		{
			foreach (var row in _data)
			{
				if (row.ParentsWith == null) continue;

				if (row.PcValue == null) continue;
				string firstParentPart = row.SelectionXBin.Item2.Substring(0, row.PcValue.Value);
				const string spacer = " | ";
				string secondParentPart = row.ParentsWith.SelectionXBin.Item2.Substring(row.PcValue.Value);
				row.ChildXBin = firstParentPart + spacer + secondParentPart;
			}
		}
		
		private void RandomizePc()
		{
			foreach (var row in _data)
			{
				if (row.ParentsWith == null || row.PcValue != null) continue;

				int pc = 1 + (int)Math.Round(StaticValues.Rand.NextDouble() * (StaticValues.L - 2));
				row.PcValue = pc;
				row.ParentsWith.PcValue = pc;
				
				_outputTable.Invalidate();
			}
		}

		private void PairParents()
		{
			for (var i = 0; i < _data.Count; i++)
			{
				var row = _data[i];
				if (!row.IsParent || row.ParentsWith != null) continue;
				DataRow pair = null;
				if (i + 1 < _data.Count)
				{
					for (var j = i + 1; j < _data.Count; j++)
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
		
		private void GetParents()
		{
			foreach (var row in _data)
			{
				row.ParentRand = StaticValues.Rand.NextDouble();
				_outputTable.Invalidate();
			}
		}
		
		private void Select()
		{
			foreach (var row in _data)
			{
				row.SelectRandom = StaticValues.Rand.NextDouble();
				var selectedIndex = _data.Count - 1;
				for (var i = 0; i < _data.Count; i++)
				{
					if (!(_data[i].QxValue >= row.SelectRandom)) continue;
					selectedIndex = i;
					break;
				}

				row.SelectionValue = _data[selectedIndex].OriginalValues;
			}
		}

		private void CalculateGx()
		{
			switch (StaticValues.TargetFunction)
			{
				case TargetFunction.Max:
					var min = _data.Min(x => x.OriginalValues.Fx);
					foreach (var dataRow in _data)
					{
						dataRow.GxValue = dataRow.OriginalValues.Fx - min + StaticValues.D;
					}

					break;
				
				case TargetFunction.Min:
					var max = _data.Max(x => x.OriginalValues.Fx);
					foreach (var dataRow in _data)
					{
						dataRow.GxValue = -(dataRow.OriginalValues.Fx - max) + StaticValues.D;
					}
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void CalculatePx()
		{
			var sum = _data.Sum(x => x.GxValue);
			foreach (var dataRow in _data)
			{
				dataRow.PxValue = dataRow.GxValue / sum;
			}
		}

		private void CalculateQx()
		{
			var sum = 0.0;
			foreach (var dataRow in _data)
			{
				sum += dataRow.PxValue;
				dataRow.QxValue = sum;
			}
		}

		//-----------------------------------------------------------
		//Main Form
		public MainForm()
		{
			Title = "ISA_Marcin_Ryba";
			Width = 1650;
			Height = 450;
			MinimumSize = new Size(1650, 450);
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
				Command = new Command((_, _) => StartIna())
				
			};

			_outputTable = _outputTable = new GridView()
			{
				DataStore = new ObservableCollection<DataRow>(),
				Width = 800
			};

			//Make columns
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
				Height = 300,
				Content = _outputTable
			};
			
			_pkInput = new TextBox()
			{
				Text = "0.5"
			};

			_pmInput = new TextBox()
			{
				Text = "0.005"
			};

			//Display
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
                                Height = 45
                            },
                            
                            new Label
                            {
                                Text = "b: "
                            },
                            _bInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 45
                            },
                            
                            new Label
                            {
                                Text = "d: "
                            },
                            _dInput,
                            new Panel
                            {
                                Width = 50,
                                Height = 45
                            },
                            
                            new Label
                            {
                                Text = "n: "
                            },
                            _nInput,
                            new Panel
                            {
	                            Width = 50,
	                            Height = 45
                            },
                            
                            new Label
                            {
                                Text = "PK: "
                            },
                            _pkInput,
                            new Panel
                            {
	                            Width = 50,
	                            Height = 45
                            },

                            new Label
                            {
                                Text = "PM: "
                            },
                            _pmInput,
                            new Panel
                            {
	                            Width = 50,
	                            Height = 45
                            },

                            new Label()
                            {
	                            Text = "Target Func: "
                            },
                            _targetFunctionDropdown,
                            new Panel
                            {
	                            Width = 50,
	                            Height = 45
                            },
                            startButton,
                        }
                    },
                    outputTableScrollable,
                }
            };
            
			SizeChanged += (_, _) =>
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