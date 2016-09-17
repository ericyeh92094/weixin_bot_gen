using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace weixin_bot
{

    public class Intent : IComparable<Intent>
    {
        public string intent { get; set; }
        public float score { get; set; }

        int IComparable<Intent>.CompareTo(Intent other)
        {
            if (other.score > score)
                return -1;
            else if (other.score == score)
                return 0;
            else
                return 1;
        }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }

    public class LUISModel
    {
        public const int MAX_INTENT = 20;
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
    }

    public class KBDoc : TableEntity
    {
        public KBDoc() { }

        public string Description { get; set; }
        public string IconURL { get; set; }
        public string URL { get; set; }
    }

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
    public class RootObject
    {
        public string _type { get; set; }
        public WebPages webPages { get; set; }
        public RelatedSearches relatedSearches { get; set; }
        public RankingResponse rankingResponse { get; set; }
    }
    public class WebPages
    {
        public string webSearchUrl { get; set; }
        public int totalEstimatedMatches { get; set; }
        public List<Value> value { get; set; }
    }
    public class Value
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
        public List<About> about { get; set; }
        public string displayUrl { get; set; }
        public string snippet { get; set; }
        public List<DeepLink> deepLinks { get; set; }
        public string dateLastCrawled { get; set; }
        public List<SearchTag> searchTags { get; set; }
    }
    public class About
    {
        public string name { get; set; }
    }
}