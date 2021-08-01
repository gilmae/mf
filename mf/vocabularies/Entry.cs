using System;
namespace mf.vocabularies
{
    public record Entry : Vocabulary
    {
        public Entry(Microformat mf) : base(mf) { }

        public object[] Name { get { return GetMember("name"); } set { SetMember("name", value); } }
        public object[] Summary { get { return GetMember("summary"); } set { SetMember("summary", value); } }
        public object[] Content { get { return GetMember("content"); } set { SetMember("content", value); } }
        public object[] Published { get { return GetMember("published"); } set { SetMember("published", value); } }
        public object[] Updated { get { return GetMember("updated"); } set { SetMember("updated", value); } }
        public object[] Author { get { return GetMember("author"); } set { SetMember("author", value); } }
        public object[] Category { get { return GetMember("category"); } set { SetMember("category", value); } }
        public object[] Url { get { return GetMember("url"); } set { SetMember("url", value); } }
        public object[] Uid { get { return GetMember("uid"); } set { SetMember("uid", value); } }
        public object[] Geo { get { return GetMember("geo"); } set { SetMember("geo", value); } }
        public object[] Latitude { get { return GetMember("latitude"); } set { SetMember("latitude", value); } }
        public object[] Longitude { get { return GetMember("longitude"); } set { SetMember("longitude", value); } }
    }
}
