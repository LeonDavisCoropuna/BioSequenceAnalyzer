using Xunit;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class FastaSequenceTests
{
    [Fact]
    public void IdentifySequenceType_FromMultipleFastaFiles_ValidProteinSequences()
    {
        var fastaFolder = Path.Combine(Directory.GetCurrentDirectory(), "TestData");

        var fastaFiles = Directory.GetFiles(fastaFolder, "*.fasta");

        foreach (var filePath in fastaFiles)
        {
            var sequence = ReadSingleFastaSequence(filePath);

            var result = BioSequenceAnalyzer.BiologicalSequenceAnalyzer.IdentifySequenceType(sequence);

            Assert.Equal(BioSequenceAnalyzer.BiologicalSequenceAnalyzer.SequenceType.Protein, result);
        }
    }

    private string ReadSingleFastaSequence(string path)
    {
        var lines = File.ReadAllLines(path);
        return string.Join("", lines.Where(line => !line.StartsWith(">")).Select(line => line.Trim()));
    }
}
