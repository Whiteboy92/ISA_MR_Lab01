namespace ISA_Ryba_Marcin
{
    public class DataRow
    {
        public static DataRow Empty = new DataRow(null, -1);

        public Values OriginalValues = null;
        public long Index;
        
        public DataRow(Values originalValues, long index)
        {
            OriginalValues = originalValues;
            Index = index;
        }


    }
}