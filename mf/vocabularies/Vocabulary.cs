using System;
using System.Dynamic;
using mf.vocabularies;

namespace mf
{
    public abstract record Vocabulary
    {
        protected Microformat _mf;
        protected Vocabulary(Microformat mf)
        {
            _mf = mf;
        }

        protected void SetMember(string name, object value)
        {
            _mf.AddProperty(name, value);
        }

        protected object[] GetMember(string name)
        {
            bool success = _mf.Properties.TryGetValue(name, out object[] val);
            return success?val:null;
        }
    }

}
