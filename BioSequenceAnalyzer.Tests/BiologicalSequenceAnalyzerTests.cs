using Xunit;
using BioSequenceAnalyzer;

namespace BioSequenceAnalyzer.Tests
{
    public class BiologicalSequenceAnalyzerTests
    {
        #region IdentifySequenceType Tests
        
        [Theory]
        [InlineData("ATGCGTA", BiologicalSequenceAnalyzer.SequenceType.DNA)]
        [InlineData("ATCGATCG", BiologicalSequenceAnalyzer.SequenceType.DNA)]
        [InlineData("AUCGAUCG", BiologicalSequenceAnalyzer.SequenceType.RNA)]
        [InlineData("AUUGACUA", BiologicalSequenceAnalyzer.SequenceType.RNA)]
        [InlineData("ACDEFGHIKLMNPQRSTVWY", BiologicalSequenceAnalyzer.SequenceType.Protein)]
        [InlineData("MAVLD", BiologicalSequenceAnalyzer.SequenceType.Protein)] // Secuencia proteica corta
        [InlineData("ATGCGU", BiologicalSequenceAnalyzer.SequenceType.Unknown)] // Mezcla ADN/ARN
        [InlineData("ATGCXTA", BiologicalSequenceAnalyzer.SequenceType.Unknown)] // Carácter inválido
        [InlineData("", BiologicalSequenceAnalyzer.SequenceType.Unknown)]
        [InlineData(null, BiologicalSequenceAnalyzer.SequenceType.Unknown)]
        public void IdentifySequenceType_VariousSequences_ReturnsCorrectType(string sequence, BiologicalSequenceAnalyzer.SequenceType expected)
        {
            var result = BiologicalSequenceAnalyzer.IdentifySequenceType(sequence);
            Assert.Equal(expected, result);
        }

        // [Fact]
        // public void IdentifySequenceType_ProteinWithSelenocysteine_ReturnsProtein()
        // {
        //     // Secuencia proteica que contiene selenocisteína (U) y otros aminoácidos
        //     var sequence = "ACDEFGHIKLMNPQRSTUVWY";
        //     var result = BiologicalSequenceAnalyzer.IdentifySequenceType(sequence);
        //     Assert.Equal(BiologicalSequenceAnalyzer.SequenceType.Protein, result);
        // }

        #endregion

        #region IdentifyAminoAcid Tests

        [Theory]
        [InlineData("ATG", "Met")] // ADN
        [InlineData("AUG", "Met")] // ARN
        [InlineData("TTT", "Phe")]
        [InlineData("TTC", "Phe")]
        [InlineData("TAA", "STOP")] // Codón de parada
        [InlineData("TGA", "STOP")] // Codón de parada
        [InlineData("GGG", "Gly")]
        [InlineData("CAT", "His")]
        [InlineData("GAC", "Asp")]
        public void IdentifyAminoAcid_ValidCodons_ReturnsCorrectAminoAcid(string codon, string expected)
        {
            var result = BiologicalSequenceAnalyzer.IdentifyAminoAcid(codon);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("XYZ")] // Codón inválido
        [InlineData("AU")] // Muy corto
        [InlineData("AUGX")] // Muy largo
        [InlineData(null)]
        public void IdentifyAminoAcid_InvalidCodons_ReturnsInvalidCodon(string codon)
        {
            var result = BiologicalSequenceAnalyzer.IdentifyAminoAcid(codon);
            Assert.Equal("Invalid codon", result);
        }

        #endregion

        #region TranslateDnaToProtein Tests

        [Fact]
        public void TranslateDnaToProtein_ValidDnaSequence_ReturnsCorrectProtein()
        {
            var dna = "ATGGAAGTATTTAAAGCGCCACCTATTGGGATATAAG";
            var expected = "Met-Glu-Val-Phe-Lys-Ala-Pro-Pro-Ile-Gly-Ile";
            var result = BiologicalSequenceAnalyzer.TranslateDnaToProtein(dna);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TranslateDnaToProtein_SequenceWithStopCodon_StopsTranslation()
        {
            var dna = "ATGGATTGATAGCCGTA"; // Contiene TAG (STOP)
            var expected = "Met-Asp-STOP";
            var result = BiologicalSequenceAnalyzer.TranslateDnaToProtein(dna, true);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TranslateDnaToProtein_ShortSequence_ReturnsPartialProtein()
        {
            var dna = "ATGGAT"; // Solo 2 codones completos
            var expected = "Met-Asp";
            var result = BiologicalSequenceAnalyzer.TranslateDnaToProtein(dna);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TranslateDnaToProtein_EmptySequence_ReturnsEmptyString()
        {
            var dna = "";
            var result = BiologicalSequenceAnalyzer.TranslateDnaToProtein(dna);
            Assert.Equal("", result);
        }

        [Fact]
        public void TranslateDnaToProtein_InvalidCharacters_ReturnsValidPart()
        {
            var dna = "ATGGXTATTAA"; // Contiene X inválido
            var expected = "Met"; // Solo traduce el primer codón válido
            var result = BiologicalSequenceAnalyzer.TranslateDnaToProtein(dna);
            Assert.Equal(expected, result);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void IdentifySequenceType_LowerCaseInput_ReturnsCorrectType()
        {
            var sequence = "atgcgta";
            var result = BiologicalSequenceAnalyzer.IdentifySequenceType(sequence);
            Assert.Equal(BiologicalSequenceAnalyzer.SequenceType.DNA, result);
        }

        [Fact]
        public void TranslateDnaToProtein_LowerCaseInput_ReturnsCorrectProtein()
        {
            var dna = "atggaagtatttaaagcgccacctattgggatataag";
            var expected = "Met-Glu-Val-Phe-Lys-Ala-Pro-Pro-Ile-Gly-Ile";
            var result = BiologicalSequenceAnalyzer.TranslateDnaToProtein(dna);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IdentifyAminoAcid_LowerCaseCodon_ReturnsCorrectAminoAcid()
        {
            var codon = "aug";
            var result = BiologicalSequenceAnalyzer.IdentifyAminoAcid(codon);
            Assert.Equal("Met", result);
        }

        #endregion
    }
}