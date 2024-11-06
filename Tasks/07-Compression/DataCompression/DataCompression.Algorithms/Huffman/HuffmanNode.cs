namespace DataCompression.Algorithms.Huffman
{
    public class HuffmanNode
    {
        public char Symbol { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public List<bool> Traverse(char symbol, List<bool> data)
        {
            if (Right == null && Left == null)
            {
                return symbol.Equals(Symbol) ? data : null;
            }
            else
            {
                List<bool> left = null;
                List<bool> right = null;

                if (Left != null)
                {
                    var leftPath = new List<bool>(data) { false };
                    left = Left.Traverse(symbol, leftPath);
                }

                if (Right != null)
                {
                    var rightPath = new List<bool>(data) { true };
                    right = Right.Traverse(symbol, rightPath);
                }

                return left ?? right;
            }
        }
    }
}