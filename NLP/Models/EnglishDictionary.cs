using NLP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP
{



    public class DictionarySampleTest
    {

        public static void RunTests()
        {
            var d = new EnglishDictionary();
            var e = new DictionaryEntry("program");
            e.Word = "program";
            e.Variants = new List<DictionaryVariant>();


            var data = new[]
            {
                new { R = (int) PartOfSpeech.n, S = (int)NounSubset.Cognition, D = "a series of steps to be carried out or goals to be accomplished" },
                new { R = (int) PartOfSpeech.n, S = (int)NounSubset.Cognition, D = "a system of projects or services intended to meet a public need" },
                new { R = (int) PartOfSpeech.n, S = (int)NounSubset.Communication, D = "a radio or television show" },
                new { R = (int) PartOfSpeech.n, S = (int)NounSubset.Communication, D = "a document stating the aims and principles of a political party" },
                new { R = (int) PartOfSpeech.n, S = (int)NounSubset.Communication, D = "an announcement of the events that will occur as part of a theatrical or sporting event" },
                new { R = (int) PartOfSpeech.n, S = (int)NounSubset.Communication, D = "an integrated course of academic studies" },
                new { R = (int) PartOfSpeech.n, S = (int)NounSubset.Communication, D = "(computer science) a sequence of instructions that a computer can interpret and execute" },
                new { R = (int) PartOfSpeech.n, S = (int)NounSubset.Act, D = "a performance (or series of performances) at a public presentation" },
                new { R = (int) PartOfSpeech.v, S = (int)VerbSubset.Communication, D = "arrange a program of or for" },
                new { R = (int) PartOfSpeech.v, S = (int)VerbSubset.Creation, D = "write a computer program" },
            };


            DictionaryVariant vn = (PartOfSpeech.n, NounSubset.Cognition, "a series of steps to be carried out or goals to be accomplished");
            DictionaryVariant vv = (PartOfSpeech.n, VerbSubset.Unknown, "a series of steps to be carried out or goals to be accomplished");
            DictionaryVariant vadv = (PartOfSpeech.n, AdverbSubset.Unknown, "a series of steps to be carried out or goals to be accomplished");
            DictionaryVariant vadj = (PartOfSpeech.n, AdjectiveSubset.Unknown, "a series of steps to be carried out or goals to be accomplished");

            var e2 = new DictionaryEntry("program", new[]
            {
               new DictionaryVariant() { PartOfSpeech= (int)PartOfSpeech.n, PartOfSpeechSubset = (int)NounSubset.Cognition, Definition = "a series of steps to be carried out or goals to be accomplished" },
               (PartOfSpeech.n, NounSubset.Cognition, "a series of steps to be carried out or goals to be accomplished"),
               (PartOfSpeech.n, VerbSubset.Unknown, "a series of steps to be carried out or goals to be accomplished"),

            });

            DictionaryEntry e3 = ("program", new DictionaryVariant[]
            {
                (PartOfSpeech.n, NounSubset.Cognition, "a series of steps to be carried out or goals to be accomplished"),
                (PartOfSpeech.n, VerbSubset.Unknown, "a series of steps to be carried out or goals to be accomplished"),
                (PartOfSpeech.n, AdverbSubset.Unknown, "a series of steps to be carried out or goals to be accomplished"),
                (PartOfSpeech.n, AdjectiveSubset.Unknown, "a series of steps to be carried out or goals to be accomplished")
            });


            DictionaryEntry e35 = ("program", new []
             {
                    ((int)PartOfSpeech.n, (int)NounSubset.Cognition, "a series of steps to be carried out or goals to be accomplished"),
                    ((int)PartOfSpeech.n, (int)VerbSubset.Unknown, "a series of steps to be carried out or goals to be accomplished"),
             });

            DictionaryEntry e4 = ("program", new DictionaryVariant[]
            {
                (PartOfSpeech.n, NounSubset.Cognition, "a series of steps to be carried out or goals to be accomplished"),
            });


            var entryVariants = Enumerable.Range(0, data.Length)
                .Select(index =>
                {
                    var src = data[index];
                    return new DictionaryVariant()
                    {
                        //TODO: Id =0
                        //TODO: EntryId = 0;
                        VariantIndex = index,
                        PartOfSpeech = src.R,
                        PartOfSpeechSubset = src.S,
                        Definition = src.D
                    };
                })
                .ToList();
            e.Variants.AddRange(entryVariants);

        }
    }

    // singular, plural, objectify, genderizae.
    public enum PartOfSpeechFull
    {
        Unknown = 0,
        Noun = 1,
        Verb = 2,
        Adverb = 3,
        Adjective = 4
    }

    public enum NounSubset
    {
        Unknown = 0,
        Cognition = 1,
        Communication = 2,
        Act = 3,
    }

    public static class PartOfSpeecExtensions
    {
        static Dictionary<string, PartOfSpeechFull> partofSpeechFull = GetEnumDict<PartOfSpeechFull>();
        static Dictionary<string, PartOfSpeech> partofSpeech = GetEnumDict<PartOfSpeech>();
        static Dictionary<string, NounSubset> nounSubsets = GetEnumDict<NounSubset>();
        static Dictionary<string, VerbSubset> verbSubsets = GetEnumDict<VerbSubset>();
        static Dictionary<string, AdverbSubset> adverbSubsets = GetEnumDict<AdverbSubset>();
        static Dictionary<string, AdjectiveSubset> adjectiveSubset = GetEnumDict<AdjectiveSubset>();
        private static Dictionary<string, T> GetEnumDict<T>() where T : struct
        {
            var result = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                Enum.TryParse(name, out T enumResult);
                result.Add(name, enumResult);
            }
            return result;
        }
        public static NounSubset ParseNounSubset(this string value)
        {
            if (nounSubsets.ContainsKey(value)) return nounSubsets[value];
            return default;
        }

        public static VerbSubset ParseVerbSubset(this string value)
        {
            if (verbSubsets.ContainsKey(value)) return verbSubsets[value];
            return default;
        }
        public static AdverbSubset ParseAdverbSubset(this string value)
        {
            if (adverbSubsets.ContainsKey(value)) return adverbSubsets[value];
            return default;
        }
        public static AdjectiveSubset ParseAdjectiveSubset(this string value)
        {
            if (adjectiveSubset.ContainsKey(value)) return adjectiveSubset[value];
            return default;
        }
        public static PartOfSpeechFull ParsePartOfSpeech(this string value)
        {
            if (partofSpeechFull.ContainsKey(value)) return partofSpeechFull[value];
            if (partofSpeech.ContainsKey(value)) return (PartOfSpeechFull)partofSpeech[value];
            return default;
        }
    }


    public enum VerbSubset
    {
        Unknown = 0,
        Act = 1,
        Communication = 2,
        Creation = 3,
    }
    public enum AdjectiveSubset
    {
        Unknown = 0,
    }
    public enum AdverbSubset
    {
        Unknown = 0,
    }
    public enum PartOfSpeech
    {
        u = 0,
        n = 1,
        v = 2,
        adv = 3,
        adj = 4
    }
    public class EnglishDictionary
    {

        private static WordNetCrawler crawler = new WordNetCrawler();

        private static Dictionary<string, DictionaryEntry> entries = new Dictionary<string, DictionaryEntry>();
        public static DictionaryEntry Lookup(string word)
        {
            DictionaryEntry result = null;
            if (!entries.ContainsKey(word))
            {
                try
                {
                    var def = crawler.Search(word);
                    result = DictionaryEntry.FromDefinition(def);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed looking up '{word}' - {ex.Message}");
                }
                entries[word] = result;
            }
            else
            {
                result = entries[word];
            }
            return result;
        }
    }


    public class DictionaryEntry
    {
        public DictionaryEntry(string word, IEnumerable<DictionaryVariant> variants = null)
        {
            Word = word;
            this.Variants = variants?.ToList() ?? new List<DictionaryVariant>();
        }

        public int Id { get; set; }
        public string Word { get; set; }
        public List<DictionaryVariant> Variants { get; set; }

        internal static DictionaryEntry FromDefinition(Definition def)
        {
            var result = new DictionaryEntry(def.Word);


            foreach (var item in def.Definitions.SelectMany(x => x.Value))
            {
                var variant = new DictionaryVariant();

                variant.Definition = item.Definition;
                variant.VariantIndex = item.Index;
                variant.PartOfSpeech = (int)item.RootType.ParsePartOfSpeech();
                switch (variant.PartOfSpeech)
                {
                    case (int)PartOfSpeechFull.Noun:
                        variant.PartOfSpeechSubset = (int)item.SubType.ParseNounSubset();
                        break;
                    case (int)PartOfSpeechFull.Verb:
                        variant.PartOfSpeechSubset = (int)item.SubType.ParseVerbSubset();
                        break;
                    case (int)PartOfSpeechFull.Adjective:
                        variant.PartOfSpeechSubset = (int)item.SubType.ParseAdjectiveSubset();
                        break;
                    case (int)PartOfSpeechFull.Adverb:
                        variant.PartOfSpeechSubset = (int)item.SubType.ParseAdverbSubset();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Invalid root type: '{item.RootType}'");
                }
                result.Variants.Add(variant);
            }

            return result;
        }

        //public static implicit operator DictionaryEntry((string word, (PartOfSpeech partOfSpeech, VerbSubset partOfSpeechSubset, string definition)[] variants) tuple)
        //{
        //    return new DictionaryEntry(tuple.word, tuple.variants.Cast<DictionaryVariant>());
        //}

        public static implicit operator DictionaryEntry((string word, DictionaryVariant[] variants) tuple)
        {
            return new DictionaryEntry(tuple.word, tuple.variants);
        }

        public static implicit operator DictionaryEntry((string word, (int partOfSpeech, int partOfSpeechSubset, string definition)[] variants) tuple)
        {
            return new DictionaryEntry(tuple.word, tuple.variants.Select(x => (DictionaryVariant)x).ToList());
        }
    }

    public class DictionaryVariant
    {

        public DictionaryVariant() { }
        public DictionaryVariant(int partOfSpeech, int partOfSpeechSubset, string definition)
        {
            PartOfSpeech = partOfSpeech;
            PartOfSpeechSubset = partOfSpeechSubset;
            Definition = definition;
        }

        public int Id { get; set; }
        public int EntryId { get; set; }
        public int VariantIndex { get; set; }
        public int PartOfSpeech { get; set; }
        public int PartOfSpeechSubset { get; set; }
        public string Definition { get; set; }

        public static implicit operator DictionaryVariant((PartOfSpeech partOfSpeech, NounSubset partOfSpeechSubset, string definition) tuple)
        {
            return new DictionaryVariant((int)tuple.partOfSpeech, (int)tuple.partOfSpeechSubset, tuple.definition);
        }

        public static implicit operator DictionaryVariant((PartOfSpeech partOfSpeech, VerbSubset partOfSpeechSubset, string definition) tuple)
        {
            return new DictionaryVariant((int)tuple.partOfSpeech, (int)tuple.partOfSpeechSubset, tuple.definition);
        }

        public static implicit operator DictionaryVariant((PartOfSpeech partOfSpeech, AdjectiveSubset partOfSpeechSubset, string definition) tuple)
        {
            return new DictionaryVariant((int)tuple.partOfSpeech, (int)tuple.partOfSpeechSubset, tuple.definition);
        }

        public static implicit operator DictionaryVariant((PartOfSpeech partOfSpeech, AdverbSubset partOfSpeechSubset, string definition) tuple)
        {
            return new DictionaryVariant((int)tuple.partOfSpeech, (int)tuple.partOfSpeechSubset, tuple.definition);
        }


        public static implicit operator DictionaryVariant((int partOfSpeech, int partOfSpeechSubset, string definition) tuple)
        {
            return new DictionaryVariant(tuple.partOfSpeech, tuple.partOfSpeechSubset, tuple.definition);
        }
    }
}
