using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
namespace NLP.Models
{
    public class Definition
    {
        public Dictionary<string, List<EntryDto>> Definitions = new Dictionary<string, List<EntryDto>>();

        private string currentType;
        public string Word { get; private set; }

        public Definition(string word)
        {
            this.Word = word;
        }

        public void AddRootType(string type)
        {
            Definitions.Add((currentType = type), new List<EntryDto>());
        }

        public void AddEntry(EntryDto dto)
        {
            Definitions[currentType].Add(dto);
        }
    }
    public class EntryList
    {
        public string Root;
        public List<EntryDto> Entries;
    }
    public class EntryDto
    {
        public int Index;
        public string RootType;
        public string SubType;
        public string Definition;

        public EntryDto(string index, string rootType, string subType, string definition)
        {
            this.Index = int.Parse(index);
            RootType = rootType;
            SubType = subType;
            Definition = definition;
        }
    }
    public class WordNetCrawler
    {
        public WordNetCrawler() : this("http://wordnetweb.princeton.edu/perl/webwn")
        {

        }
        public WordNetCrawler(string address)
        {
            this.BaseAddress = address;

        }
        public string BaseAddress;


        public ParamBuilder builder = new ParamBuilder();
        //public WebClient Client = new WebClient();
        public HtmlWeb Client = new HtmlWeb();
        public Encoding enc = System.Text.Encoding.UTF8;
        public Definition Search(string word)
        {
            var Definition = new Definition(word);

            var query = builder.GetQuery(word);
            var url = BaseAddress + query;
            HtmlDocument doc = null;
            try
            {
                doc = Client.Load(url);
            }
            catch { }
            if (doc != null)
            {


                //doc.OptionOutputAsXml = true;
                var result = doc.DocumentNode.OuterHtml;
                var formDiv = (HtmlNode)doc.QuerySelector(".form");
                if (formDiv != null)
                {


                    var l = new List<HtmlNode>();
                    foreach (HtmlNode child in formDiv.ChildNodes)
                    {
                        if (child.Name != "h3" && child.Name != "ul")
                            l.Add(child);
                    }
                    foreach (HtmlNode childNode in l)
                    {
                        formDiv.RemoveChild(childNode, false);
                    }



                    var type = "";
                    foreach (HtmlNode child in formDiv.ChildNodes)
                    {
                        if (child.Name == "h3")
                        {
                            type = child.InnerText;
                            Definition.AddRootType(type);
                        }
                        else
                        {
                            var listItems = child.QuerySelectorAll("li");
                            foreach (HtmlNode li in listItems)
                            {
                                var liChildren = li.ChildNodes;

                                if (liChildren.Count > 0)
                                {

                                    try
                                    {
                                        var typeNode = liChildren.First(x => x.Name == "#text");
                                        var posType = typeNode.InnerText;
                                        posType = posType.Substring(posType.IndexOf(";") + 1);
                                        posType = posType.Substring(0, posType.IndexOf("&"));

                                        var posIdx = posType.IndexOf('.');
                                        var rootType = posType.Substring(0, posIdx);
                                        var subType = posType.Substring(posIdx + 1);

                                        var bNode = liChildren.FirstOrDefault(x => x.Name == "b");
                                        var index = "1";
                                        if (bNode != null)
                                        {
                                            var bText = bNode.InnerText.Trim();
                                            var sFullName = bText.Substring(0, bText.IndexOf(" "));
                                            index = sFullName.Substring(sFullName.IndexOf('#') + 1);
                                        }
                                        var defNode = liChildren.Last(x => x.Name == "#text");
                                        var definition = defNode.InnerText.Trim().Substring(1).TrimEnd(')');





                                        var dto = new EntryDto(index, rootType, subType, definition);
                                        Definition.AddEntry(dto);
                                    }
                                    catch (Exception ex)
                                    {
                                        string bp = ex.Message;
                                        Console.WriteLine($"Error parsing definition: {ex.Message}");

                                    }


                                }

                            }
                        }
                    }
                }
            }
            return Definition;
        }

        public class ParamBuilder
        {
            public string s;
            public string sub = "Search WordNet";
            public string o2 = "1";
            public string o0 = "1";
            public string o8 = "1";
            public string o1 = "1";
            public string o7 = "1";
            public string o5 = "1";
            public string o9 = "1";
            public string o6 = "1";
            public string o3 = "1";
            public string o4 = "1";
            public string h = "0000000000";
            NameValueCollection dict;
            public ParamBuilder()
            {
                var nvc = new NameValueCollection();
                nvc.Add(nameof(s), s);
                nvc.Add(nameof(sub), sub);
                nvc.Add(nameof(o2), o2);
                nvc.Add(nameof(o0), o0);
                nvc.Add(nameof(o8), o8);
                nvc.Add(nameof(o1), o1);
                nvc.Add(nameof(o7), o7);
                nvc.Add(nameof(o5), o5);
                nvc.Add(nameof(o9), o9);
                nvc.Add(nameof(o6), o6);
                nvc.Add(nameof(o3), o3);
                nvc.Add(nameof(o4), o4);
                nvc.Add(nameof(h), h);

                dict = nvc;
            }

            public NameValueCollection GetSearchParams(string search)
            {
                dict[nameof(s)] = search;
                return dict;
            }
            public string GetQuery(string search)
            {
                dict[nameof(s)] = search;
                return $"?{nameof(s)}={dict[nameof(s)]}&sub=Search+WordNet&o2=1&o0=1&o8=1&o1=1&o7=1&o5=1&o9=&o6=1&o3=1&o4=1&h=0000000000";
            }
        }
        public static void Test()
        {

            var crawler = new WordNetCrawler("http://wordnetweb.princeton.edu/perl/webwn");
            var result = crawler.Search("program");
        }
    }
}
