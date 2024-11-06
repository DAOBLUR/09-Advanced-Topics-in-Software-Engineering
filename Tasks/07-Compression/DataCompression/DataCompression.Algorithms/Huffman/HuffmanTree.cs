using System.Collections;
using System.Text;

namespace DataCompression.Algorithms.Huffman
{
    public class HuffmanTree
    {
        private List<HuffmanNode> nodes = new List<HuffmanNode>();
        public HuffmanNode Root { get; set; }
        public Dictionary<char, int> Frequencies = new Dictionary<char, int>();

        public void Build(string source)
        {
            foreach (char character in source)
            {
                if (!Frequencies.ContainsKey(character))
                {
                    Frequencies.Add(character, 0);
                }
                Frequencies[character]++;
            }

            foreach (KeyValuePair<char, int> symbol in Frequencies)
            {
                nodes.Add(new HuffmanNode() { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (nodes.Count > 1)
            {
                List<HuffmanNode> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList();
                if (orderedNodes.Count >= 2)
                {
                    var taken = orderedNodes.Take(2).ToList();
                    var parent = new HuffmanNode()
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }
                Root = nodes.FirstOrDefault();
            }
        }

        public BitArray Encode(string source)
        {
            var encodedSource = new List<bool>();
            foreach (char character in source)
            {
                encodedSource.AddRange(Root.Traverse(character, new List<bool>()));
            }
            return new BitArray(encodedSource.ToArray());
        }

        public string Decode(BitArray bits, Dictionary<char, int> frequencies)
        {
            BuildTreeFromFrequencies(frequencies);
            var current = Root;
            var decoded = new StringBuilder();

            foreach (bool bit in bits)
            {
                current = bit ? current.Right : current.Left;
                if (current.Left == null && current.Right == null)
                {
                    decoded.Append(current.Symbol);
                    current = Root;
                }
            }
            return decoded.ToString();
        }

        private void BuildTreeFromFrequencies(Dictionary<char, int> frequencies)
        {
            nodes.Clear();
            foreach (var symbol in frequencies)
            {
                nodes.Add(new HuffmanNode() { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (nodes.Count > 1)
            {
                var orderedNodes = nodes.OrderBy(node => node.Frequency).ToList();
                if (orderedNodes.Count >= 2)
                {
                    var taken = orderedNodes.Take(2).ToList();
                    var parent = new HuffmanNode()
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };
                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }
                Root = nodes.FirstOrDefault();
            }
        }
    }
}