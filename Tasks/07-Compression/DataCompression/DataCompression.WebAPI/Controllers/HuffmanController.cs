using DataCompression.Algorithms.Huffman;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.IO;
using System.Text.Json;

namespace DataCompression.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HuffmanController : ControllerBase
    {
        private readonly ILogger<HuffmanController> _logger;

        public HuffmanController(ILogger<HuffmanController> logger)
        {
            _logger = logger;
        }

        [HttpPost("compress")]
        public async Task<IActionResult> HuffmanCompressor(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string fileContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            // Construir el árbol de Huffman y codificar
            var huffmanTree = new HuffmanTree();
            huffmanTree.Build(fileContent);
            BitArray encoded = huffmanTree.Encode(fileContent);

            // Guardar la tabla de frecuencias y los datos comprimidos en memoria
            var frequencies = huffmanTree.Frequencies;
            using (var memoryStream = new MemoryStream())
            {
                // Guardar frecuencias
                var frequenciesJson = JsonSerializer.Serialize(frequencies);
                var frequenciesBytes = System.Text.Encoding.UTF8.GetBytes(frequenciesJson);
                memoryStream.Write(frequenciesBytes, 0, frequenciesBytes.Length);
                memoryStream.WriteByte(0); // Delimitador entre frecuencias y datos

                // Guardar datos comprimidos empaquetados en bytes
                byte currentByte = 0;
                int bitIndex = 0;
                foreach (bool bit in encoded)
                {
                    if (bit)
                    {
                        currentByte |= (byte)(1 << bitIndex);
                    }
                    bitIndex++;
                    if (bitIndex == 8)
                    {
                        memoryStream.WriteByte(currentByte);
                        currentByte = 0;
                        bitIndex = 0;
                    }
                }
                if (bitIndex > 0)
                {
                    memoryStream.WriteByte(currentByte);
                }

                var compressedBytes = memoryStream.ToArray();
                return File(compressedBytes, "application/octet-stream", "compressed.huff");
            }
        }

        [HttpPost("decompress")]
        public async Task<IActionResult> HuffmanDecompressor(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            Dictionary<char, int> loadedFrequencies;
            BitArray loadedBits;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // Leer frecuencias
                var frequenciesBytes = new List<byte>();
                int i = 0;
                for (; i < fileBytes.Length; i++)
                {
                    if (fileBytes[i] == 0)
                        break;
                    frequenciesBytes.Add(fileBytes[i]);
                }

                var frequenciesJson = System.Text.Encoding.UTF8.GetString(frequenciesBytes.ToArray());
                loadedFrequencies = JsonSerializer.Deserialize<Dictionary<char, int>>(frequenciesJson);

                // Leer datos comprimidos empaquetados en bytes
                var dataBytes = fileBytes.Skip(i + 1).ToArray();
                var bits = new List<bool>();
                foreach (byte b in dataBytes)
                {
                    for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                    {
                        bits.Add((b & (1 << bitIndex)) != 0);
                    }
                }
                loadedBits = new BitArray(bits.ToArray());
            }

            // Decodificar
            var huffmanTree = new HuffmanTree();
            string decoded = huffmanTree.Decode(loadedBits, loadedFrequencies);

            // Devolver el archivo descomprimido
            var decompressedBytes = System.Text.Encoding.UTF8.GetBytes(decoded);
            return File(decompressedBytes, "application/octet-stream", "decompressed.txt");
        }
    }
}