    public class SearchRequest
    {
        public int MaxLength { get; set; } = 5;
        public int MaxWords { get; set; } = 10;
        public int MinLength { get; set; } = 1;
        public string[] lettersMatrix {get; set;}= new string[0];
    }