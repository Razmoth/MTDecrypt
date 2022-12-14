var inputDir = Path.Combine(Directory.GetCurrentDirectory(), "Input");
var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Output");

Directory.CreateDirectory(inputDir);
Directory.CreateDirectory(outputDir);

Console.WriteLine("Place encrypted files in Input folder, then press any key to continue");
Console.ReadKey();

var files = Directory.GetFiles(inputDir);
Console.WriteLine($"Found {files.Length} file(s) !!");
foreach(var file in files)
{
    var fileName = Path.GetFileName(file);
    Console.WriteLine($"Processing {fileName}...");
    try
    {
        var output = Path.Combine(outputDir, fileName);
        var data = MTDecrypt(file);
        File.WriteAllBytes(output, data);
    }
    catch(Exception e)
    {
        Console.WriteLine($"Unable to decrypt file {fileName}: {e.Message}");
    }
}
Console.WriteLine("Done !!");

static byte[] MTDecrypt(string file)
{
    using var fs = File.OpenRead(file);
    using var reader = new BinaryReader(fs);

    var seed = reader.ReadInt32();
    var box = reader.ReadBytes(0x80);

    var mt = new MT19937(seed);
    var xorpad = new byte[0x200];
    for (int i = 0; i < xorpad.Length; i += 4)
    {
        var seedBytes = BitConverter.GetBytes(mt.Int32());

        seedBytes[0] ^= box[i % box.Length];
        seedBytes[1] ^= box[(i + 7) % box.Length];
        seedBytes[2] ^= box[(i + 13) % box.Length];
        seedBytes[3] ^= box[(i + 11) % box.Length];

        Buffer.BlockCopy(seedBytes, 0, xorpad, i, 4);
    }

    var xorpadIndex = mt.Int32();
    var pos = xorpadIndex - reader.BaseStream.Position;

    var data = new byte[reader.BaseStream.Length];
    reader.BaseStream.Position = 0;
    reader.Read(data);
    for (int i = 0; i < data.Length; i++)
    {
        data[i] ^= xorpad[(pos + i) % xorpad.Length];
    }

    return data[0x84..];
}
