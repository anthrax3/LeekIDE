using Newtonsoft.Json;

namespace LeekIDE.Autocompletion
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CodeSnippet
    {
        [JsonConstructor]
        public CodeSnippet(string shortenedcalling, string code)
        {
            ShortenedCalling = shortenedcalling;
            Code = code;
        }
        public string ShortenedCalling { get; set; }
        public string Code { get; set; }
    }
}