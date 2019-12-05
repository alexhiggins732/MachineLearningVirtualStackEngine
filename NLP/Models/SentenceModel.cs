using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP.Models
{
    public class SentenceModelTests
    {
        public class SentenceGUI
        {
            //TODO: Auto complete rtf text editor that classifies words and sequences. hyperlinks to ID, relations, code references via context menu.
            //Enables learning and teaching in real-time.
        }
        public static void RunTest()
        {
            var rawText = @"Zero is the default numerical value for numeric data types. As a whole number it is represented as 0. As a decimal it is represented as 0.0.
			The standard built in data types for working with whole numbers are SByte, Byte, Int16, UInt16, Int32, Uint32, Int64 and Uint64. The standard 
			data types for working with floating-point decimal values are Single, Double and Decimal. Big Integer is the standard type for working with very large whole numbers.
			To work with very large floating point numbers a third party libary is required. By default, the Int32 data type is typically used to work with whole numerical values.";

            // given this raw text, test contexts are set to approriate topics.
            //  -> correct defintions for zero, default, numerical, value, data types, whole number, decimal, represented, standard, built in, data types, floating point,
            //      working with, third party libary
            // -> literals 0, 0.0, SByte, Byte, Int16, UInt16, Int32, Uint32, Int64, Uint64, Single, Double and Decimal, Big Integer 
            NLPDocumentModel documentModel = NLPDocumentModel.Parse(rawText);

            // map clr types=> System.SByte, System.Byte, System.Int16, System.UInt16, System.Int32, System.Uint32, System.Int64, System.Uint64.  
            // -> while Single, Double and Decimal, Big Integer  map mapp to dictionary definitions, dictionary should be extended with gloss
            //  given the context and perhaps casing.

            // Default model will only have dictionary lookups. Types extracted from the paragraph represent moves for vm stacks.
            //  algorithm will learn to first resolve system types from string representations of their names.
            // Once resolving the system type, create a default of each -> watch for built-in OpCode vs invoking method default(type).
            //  It will learn the opcode first, which is correct going forward however at this point we are trying to teach it to instantaite types
            //  using the CLR model as opposed to a direct opcode. This will allow it to default(DateTime|DateTimeOffset) and other structs
            //  as well as lead to learning instantaition of classes, including arrays, lists for which type and constructor arguments will be needed.


        }
    }

    /// <summary>
    /// A document is a member of a corpus, whose sentences, based on part of speech, subset and sequence provide the context for resolving default
    /// contextuals information using clustering and other methods.
    /// </summary>
    public class NLPDocumentModel
    {
        public List<SentenceModel> Sentences { get; protected set; }
        public List<IWordSense> WordSenses { get; protected set; } = new List<IWordSense>();
        public List<NounWordSense> NounSenses => WordSenses.Where(x => x.WordSenseType == WordSenseType.Noun).Cast<NounWordSense>().ToList();
        public List<VerbWordSense> VerbSenses => WordSenses.Where(x => x.WordSenseType == WordSenseType.Verb).Cast<VerbWordSense>().ToList();
        public NLPDocumentModel(List<SentenceModel> sentences)
        {
            this.Sentences = sentences;
        }

        public static NLPDocumentModel Parse(string rawText)
        {
            List<SentenceModel> sentences = SentenceModel.GetSentences(rawText);
            return new NLPDocumentModel(sentences);
        }

        private bool IsWordSenseDirty = false;

        private List<IWordSense> classifiedWordSenses;
        public List<IWordSense> ClassifiedWordSenses
        {
            get
            {
                if (IsWordSenseDirty)
                {
                    classifiedWordSenses = Classify(WordSenses);
                    IsWordSenseDirty = false;
                }
                return classifiedWordSenses
            }
        }

        private List<IWordSense> Classify(List<IWordSense> wordSenses)
        {
            //TODO: this will need to be implemented to rank individual token values to the document and potentially to the corpus.
            return wordSenses.OrderBy(x => x.WordSenseType).ThenBy(x => x.Name).ToList();

        }
    }


    //TODO: Is this redundant for part of speech?
    public enum WordSenseType
    {
        Unknown = 0,
        Noun = 1,
        Verb = 2
    }

    /*

All synsets are connected to other synsets by means of semantic relations. These relations, which are not all shared by all lexical categories, include: 
Nouns 
hypernyms: Y is a hypernym of X if every X is a (kind of) Y (canine is a hypernym of dog)
hyponyms: Y is a hyponym of X if every Y is a (kind of) X (dog is a hyponym of canine)
coordinate terms: Y is a coordinate term of X if X and Y share a hypernym (wolf is a coordinate term of dog, and dog is a coordinate term of wolf)
meronym: Y is a meronym of X if Y is a part of X (window is a meronym of building)
holonym: Y is a holonym of X if X is a part of Y (building is a holonym of window)


 */
    public interface IWordSense
    {
        int Id { get; set; }
        int ParentId { get; set; }
        string Name { get; set; }
        WordSenseType WordSenseType { get; }
    }

    public class NounWordSense : IWordSense
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public WordSenseType WordSenseType { get; } = WordSenseType.Noun;


        /// <summary>
        /// hypernyms: the verb Y is a hypernym of the verb X if the activity X is a (kind of) Y (to perceive is an hypernym of to listen)
        /// </summary>
        public List<int> Hypernyms { get; set; } = new List<int>();

        /// <summary>
        /// hyponyms: Y is a hyponym of X if every Y is a (kind of) X (dog is a hyponym of canine)
        /// </summary>
        public List<int> Hyponyms { get; set; } = new List<int>();
        /// <summary>
        /// coordinate terms: Y is a coordinate term of X if X and Y share a hypernym (wolf is a coordinate term of dog, and dog is a coordinate term of wolf)
        /// </summary>
        public List<int> CoordinateTerms { get; set; } = new List<int>();

        /// <summary>
        /// meronym: Y is a meronym of X if Y is a part of X (window is a meronym of building)
        /// </summary>
        public List<int> Meronyms { get; set; } = new List<int>();

        /// <summary>
        /// holonym: Y is a holonym of X if X is a part of Y (building is a holonym of window)
        /// </summary>
        public List<int> Holonym { get; set; } = new List<int>();
    }

    /*
Verbs 
hypernym: the verb Y is a hypernym of the verb X if the activity X is a (kind of) Y (to perceive is an hypernym of to listen)
troponym: the verb Y is a troponym of the verb X if the activity Y is doing X in some manner (to lisp is a troponym of to talk)
entailment: the verb Y is entailed by X if by doing X you must be doing Y (to sleep is entailed by to snore)
coordinate terms: those verbs sharing a common hypernym (to lisp and to yell) 
     */
    public class VerbWordSense : IWordSense
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }

        public WordSenseType WordSenseType { get; } = WordSenseType.Verb;

        public VerbSubset VerbSubset { get; set; }


        /// <summary>
        /// hypernym: the verb Y is a hypernym of the verb X if the activity X is a (kind of) Y (to perceive is an hypernym of to listen)
        /// </summary>
        public List<int> Hypernyms { get; set; } = new List<int>();

        /// <summary>
        /// troponym: the verb Y is a troponym of the verb X if the activity Y is doing X in some manner (to lisp is a troponym of to talk)
        /// </summary>
        /// 
        public List<int> Troponyms { get; set; } = new List<int>();

        /// <summary>
        /// entailment: the verb Y is entailed by X if by doing X you must be doing Y (to sleep is entailed by to snore)
        /// </summary>
        public List<int> Entailment { get; set; } = new List<int>();

        /// <summary>
        /// coordinate terms: those verbs sharing a common hypernym (to lisp and to yell) 
        /// </summary>
        public List<int> CoordinateTerms { get; set; } = new List<int>();


    }


    public static class StringExt
    {
        public static string[] Split2(this string value, char c, StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries)
        {
            return value.Split(new[] { c }, splitOptions);
        }
    }
    public class SentenceModel
    {
        public int SentenceIndex { get; protected set; }
        public IEnumerable<SentenceNode> SentenceNodes { get; protected set; }

        public SentenceModel() { }
        public SentenceModel(int sentenceIndex, IEnumerable<SentenceNode> sentenceNodes)
        {
            this.SentenceIndex = sentenceIndex;
            this.SentenceNodes = sentenceNodes;
        }
        public SentenceModel(int sentenceIndex, string rawSentence) :
            this(sentenceIndex, GetSentenceNodes(rawSentence))
        {

        }

        private static IEnumerable<SentenceNode> GetSentenceNodes(string rawSentence)
        {
            int index = 0;
            return rawSentence.Split2(' ').Select(x => new SentenceNode(++index, x));
        }

        public List<SentenceNode> Nodes { get; set; } = new List<SentenceNode>();

        public static List<SentenceModel> GetSentences(string rawText)
        {
            int testId = 0;
            int sentenceIndex = 0;
            return rawText
                .Split2(' ')
                .Select(rawSentence => new SentenceModel(++sentenceIndex, rawSentence))
                .ToList();
        }
    }

    public class SentenceNode
    {
        public int Id;
        public string Word { get; set; }
        public SentenceNode() { }
        public SentenceNode(int id, string word) { this.Id = id; this.Word = word; }
    }
}
