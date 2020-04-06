using System.Text.RegularExpressions;

namespace Rosalyn.Data.Models
{
    public class BlacklistFilter
    {
        public int Id { get; set; }
        public ulong ServerId { get; set; }
        public string Content { get; set; }

        // The compiled filter
        private Regex _compiledFilter;
        
        // Returns either the compiled filter or if null, compiles it then returns it
        public Regex Compiled
            => _compiledFilter ??= new Regex(Content, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}