
namespace Booski.Lib.Common {
    public class QueryParam {
        public string Key { get; set; }
        public string[] Values { get; set; }
        public string QueryParamString { get; }

        public QueryParam(
            string key, params string[] values
        ) {
            Key = key;
            Values = values;
            QueryParamString = "";

            if(Values != null) {
                foreach(var value in Values) {
                    if(!String.IsNullOrEmpty(value)) {
                        QueryParamString += $"{key}={value}&";
                    }
                }
            }
        }
    }
}