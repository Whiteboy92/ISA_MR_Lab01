using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ISA_Ryba_Marcin
{
	public class DataRow
	{
		public static readonly DataRow Empty = new DataRow(null, -1);

		public readonly Values OriginalSpecimen = null;
		private readonly long _index;

		public double SelectionRandom;
		public double GxValue = 0.0;
		public double PxValue = 0.0;
		public double QxValue = 0.0;
		public double ParentRandom;
		
		public Values SelectionValue;

		public bool IsParent => ParentRandom < StaticValues.Pk;
		public DataRow ParentsWith = null;
		public int? PcValue;
		public string ChildXBin;
		public List<int> MutatedGenesValue = new();
		
		public string MutatedChromosomeValue = null;
		public double FinalXRealValue;
		public double FinalFxRealValue;

		public DataRow(Values originalSpecimen, long index)
		{
			OriginalSpecimen = originalSpecimen;
			_index = index;
		}

		public (string, string) N => ("LP", _index.ToString());
		public (string, string) XReal => ("xReal", OriginalSpecimen?.XReal1.ToString(CultureInfo.CurrentCulture));
		public (string, string) Fx => ("F(x)", OriginalSpecimen?.Fx.ToString(CultureInfo.CurrentCulture));

		public (string, string) Gx => ("G(x)", GxValue.ToString(CultureInfo.CurrentCulture));
		public (string, string) Px => ("P(x)", PxValue.ToString(CultureInfo.CurrentCulture));
		public (string, string) Qx => ("Q(x)", QxValue.ToString(CultureInfo.CurrentCulture));
		public (string, string) R1 => ("r", SelectionRandom.ToString(CultureInfo.CurrentCulture));

		public (string, string) SelectionXReal => ("Selection x", SelectionValue?.XReal1.ToString(CultureInfo.CurrentCulture));
		public (string, string) SelectionXBin => ("Selection bin", SelectionValue?.XBin);

		public (string, string) FirstParentXBin => ("Parent", IsParent ? SelectionXBin.Item2 : "-");

		public (string, string) Pc => ("pc", PcValue != null ? PcValue.ToString() : "-");

		public (string, string) Child => ("Child", ChildXBin ?? "-");

		public (string, string) AfterChild => ("PK x", ChildXBin != null ? ChildXBin.Replace(" | ", "") : SelectionXBin.Item2);

		public (string, string) MutatedGenes => ("Genes", 
			MutatedGenesValue.Count > 0 ? 
			MutatedGenesValue.Aggregate("", (output, gene) => output + "," + gene).Substring(1) : "-");
		public (string, string) MutatedChromosome => ("Mutation Bin", MutatedChromosomeValue ?? "-");

		public (string, string) FinalXReal => ("Mutation X", FinalXRealValue.ToString(CultureInfo.InvariantCulture));
		public (string, string) FinalFxReal => ("Mutation Fx", FinalFxRealValue.ToString(CultureInfo.CurrentCulture));

    }
}