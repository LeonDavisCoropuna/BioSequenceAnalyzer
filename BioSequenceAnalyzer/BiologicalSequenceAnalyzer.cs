using System;
using System.Linq;
using System.Collections.Generic;

namespace BioSequenceAnalyzer
{
  public class BiologicalSequenceAnalyzer
  {
    public enum SequenceType { DNA, RNA, Protein, Unknown }
    public static SequenceType IdentifySequenceType(string sequence)
    {
      if (string.IsNullOrWhiteSpace(sequence))
        return SequenceType.Unknown;

      string upperSequence = sequence.ToUpperInvariant();
      bool containsT = upperSequence.Contains('T');
      bool containsU = upperSequence.Contains('U');

      var validDnaChars = new HashSet<char> { 'A', 'C', 'G', 'T' };
      var validRnaChars = new HashSet<char> { 'A', 'C', 'G', 'U' };
      var validProteinChars = new HashSet<char> {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'Y', 'Z'
        };

      // 1) Rechaza mezclas T+U
      if (containsT && containsU)
        return SequenceType.Unknown;

      // 2) ADN puro
      if (upperSequence.All(c => validDnaChars.Contains(c)))
        return SequenceType.DNA;

      // 3) ARN puro
      if (upperSequence.All(c => validRnaChars.Contains(c)))
        return SequenceType.RNA;

      // 4) Proteína (sin U)
      if (upperSequence.All(c => validProteinChars.Contains(c)))
        return SequenceType.Protein;

      // 5) Cualquier otro caso
      return SequenceType.Unknown;
    }


    public static string IdentifyAminoAcid(string codon)
    {
      if (codon == null || codon.Length != 3)
        return "Invalid codon";

      var codonUpper = codon.ToUpper();

      var geneticCode = new Dictionary<string, string>
            {
                {"TTT", "Phe"}, {"TTC", "Phe"}, {"TTA", "Leu"}, {"TTG", "Leu"},
                {"CTT", "Leu"}, {"CTC", "Leu"}, {"CTA", "Leu"}, {"CTG", "Leu"},
                {"ATT", "Ile"}, {"ATC", "Ile"}, {"ATA", "Ile"}, {"ATG", "Met"},
                {"GTT", "Val"}, {"GTC", "Val"}, {"GTA", "Val"}, {"GTG", "Val"},
                {"TCT", "Ser"}, {"TCC", "Ser"}, {"TCA", "Ser"}, {"TCG", "Ser"},
                {"CCT", "Pro"}, {"CCC", "Pro"}, {"CCA", "Pro"}, {"CCG", "Pro"},
                {"ACT", "Thr"}, {"ACC", "Thr"}, {"ACA", "Thr"}, {"ACG", "Thr"},
                {"GCT", "Ala"}, {"GCC", "Ala"}, {"GCA", "Ala"}, {"GCG", "Ala"},
                {"TAT", "Tyr"}, {"TAC", "Tyr"}, {"TAA", "STOP"}, {"TAG", "STOP"},
                {"CAT", "His"}, {"CAC", "His"}, {"CAA", "Gln"}, {"CAG", "Gln"},
                {"AAT", "Asn"}, {"AAC", "Asn"}, {"AAA", "Lys"}, {"AAG", "Lys"},
                {"GAT", "Asp"}, {"GAC", "Asp"}, {"GAA", "Glu"}, {"GAG", "Glu"},
                {"TGT", "Cys"}, {"TGC", "Cys"}, {"TGA", "STOP"}, {"TGG", "Trp"},
                {"CGT", "Arg"}, {"CGC", "Arg"}, {"CGA", "Arg"}, {"CGG", "Arg"},
                {"AGT", "Ser"}, {"AGC", "Ser"}, {"AGA", "Arg"}, {"AGG", "Arg"},
                {"GGT", "Gly"}, {"GGC", "Gly"}, {"GGA", "Gly"}, {"GGG", "Gly"}
            };

      var rnaCodon = codonUpper.Replace('U', 'T');
      if (geneticCode.TryGetValue(rnaCodon, out string aminoAcid))
      {
        return aminoAcid;
      }

      return "Invalid codon";
    }

    public static string TranslateDnaToProtein(string dnaSequence, bool includeStop = false)
    {
      if (string.IsNullOrWhiteSpace(dnaSequence))
        return string.Empty;

      dnaSequence = dnaSequence.ToUpper();
      var proteinSequence = new List<string>();

      for (int i = 0; i < dnaSequence.Length - 2; i += 3)
      {
        string codon = dnaSequence.Substring(i, 3);
        string aminoAcid = IdentifyAminoAcid(codon);

        if (aminoAcid == "STOP")
        {
          if (includeStop)
          {
            proteinSequence.Add("STOP");
          }
          break;
        }

        if (aminoAcid == "Invalid codon")
        {
          break;
        }

        proteinSequence.Add(aminoAcid);
      }

      return string.Join("-", proteinSequence);
    }
  }
}