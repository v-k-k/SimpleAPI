

namespace SimpleAPI
{
    public struct Item
    {
        public string field1;
        public string field2;
        public bool field3;
        public int id;

        public string ToJson(bool includeId=false)
        {
            if (includeId)
            {
                return $"{{" +
                $"\"id\": \"{id}\"," +
                $"\"field1\": \"{field1}\"," +
                $"\"field2\": \"{field2}\"," +
                $"\"field3\": {field3.ToString().ToLower()}" +
                $"}}";
            }
            return $"{{" +
                $"\"field1\": \"{field1}\"," +
                $"\"field2\": \"{field2}\"," +
                $"\"field3\": {field3.ToString().ToLower()}" +
                $"}}";
        }

    }
}
