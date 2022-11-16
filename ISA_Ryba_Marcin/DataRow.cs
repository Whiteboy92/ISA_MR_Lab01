using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ISA_Ryba_Marcin
{
	public class DataRow
	{
		public List<int> MutatedGenesValue = new();
		
		public static readonly DataRow Empty = new(null, -1);
		
		public DataRow ParentsWith = null;

		public readonly Values OriginalValues;
		public Values SelectionValue;
		
		private readonly long _index;

		public double SelectRandom;
		public double GxValue;
		public double PxValue;
		public double QxValue;
		public double ParentRand;
		public double FinalXRealValue;
		public double FinalFxRealValue;


		public bool IsParent => ParentRand < StaticValues.Pk;
		
		public int? PcValue;
		
		public string ChildXBin;
		public string MutatedChromosomeValue = null;

		public DataRow (Values originalValues, long index)
		{
			OriginalValues = originalValues;
			_index = index;
		}

		public (string, string) N => ("index", _index.ToString());
		
		public (string, string) XReal => ("xReal", OriginalValues?.XReal1.ToString(CultureInfo.CurrentCulture));
		
		public (string, string) Fx => ("F(x)", OriginalValues?.Fx.ToString(CultureInfo.CurrentCulture));

		public (string, string) Gx => ("G(x)", GxValue.ToString(CultureInfo.CurrentCulture));
		
		public (string, string) Px => ("P(x)", PxValue.ToString(CultureInfo.CurrentCulture));
		
		public (string, string) Qx => ("Q(x)", QxValue.ToString(CultureInfo.CurrentCulture));
		
		public (string, string) R1 => ("r", SelectRandom.ToString(CultureInfo.CurrentCulture));

		public (string, string) SelectionXReal => ("x Selection", SelectionValue?.XReal1.ToString(CultureInfo.CurrentCulture));
		public (string, string) SelectionXBin => ("bin Selection", SelectionValue?.XBin);

		public (string, string) FirstParentXBin => ("Parent", IsParent ? SelectionXBin.Item2 : "-");

		public (string, string) Pc => ("Pc", PcValue != null ? PcValue.ToString() : "-");

		public (string, string) Child => ("Child", ChildXBin ?? "-");

		public (string, string) AfterChild => ("Pk x", ChildXBin != null ? ChildXBin.Replace(" | ", "") : SelectionXBin.Item2);

		public (string, string) MutatedGenes => ("Genes", MutatedGenesValue.Count > 0 ? 
			MutatedGenesValue.Aggregate("", (output, gene) => output + "," + gene).Substring(1) : "-");
		
		public (string, string) MutatedChromosome => ("Bin Mutation", MutatedChromosomeValue ?? "-");

		public (string, string) FinalXReal => ("X Mutation", FinalXRealValue.ToString(CultureInfo.InvariantCulture));
		
		public (string, string) FinalFxReal => ("Fx Mutation", FinalFxRealValue.ToString(CultureInfo.CurrentCulture));

    }
}