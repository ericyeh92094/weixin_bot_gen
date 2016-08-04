using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarCaringBot
{
        public class DeepLink
        {
            public string name { get; set; }
            public string url { get; set; }
            public string snippet { get; set; }
        }

        public class SearchTag
        {
            public string name { get; set; }
            public string content { get; set; }
        }
        public class Value2
        {
            public string text { get; set; }
            public string displayText { get; set; }
            public string webSearchUrl { get; set; }
        }

        public class RelatedSearches
        {
            public string id { get; set; }
            public List<Value2> value { get; set; }
        }

        public class Value3
        {
            public string id { get; set; }
        }

        public class Item
        {
            public string answerType { get; set; }
            public int resultIndex { get; set; }
            public Value3 value { get; set; }
        }

        public class Mainline
        {
            public List<Item> items { get; set; }
        }

        public class RankingResponse
        {
            public Mainline mainline { get; set; }
        }
        public class RootObject {
            public string _type { get; set; }
            public WebPages webPages { get; set; }
            public RelatedSearches relatedSearches { get; set; }
            public RankingResponse rankingResponse { get; set; }
        }
        public class WebPages {
            public string webSearchUrl { get; set; }
            public int totalEstimatedMatches { get; set; }
            public List<Value> value { get; set; }
        }
        public class Value {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public List<About> about { get; set; }
            public string displayUrl { get; set; }
            public string snippet { get; set; }
            public List<DeepLink> deepLinks { get; set; }
            public string dateLastCrawled { get; set; }
            public List<SearchTag> searchTags { get; set; }
        }
        public class About {
            public string name { get; set; }
        }
 
}