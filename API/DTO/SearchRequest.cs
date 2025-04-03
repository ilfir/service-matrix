    namespace service_matrix.DTO;

    /// <summary>
    /// 
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// Max Len
        /// </summary>
        public int MaxLength { get; set; } = 5;
        /// <summary>
        /// Max words returned
        /// </summary>
        public int MaxWords { get; set; } = 10;
        /// <summary>
        /// Min Len
        /// </summary>
        public int MinLength { get; set; } = 1;
        /// <summary>
        /// 
        /// </summary>
        public List<List<string>>? LettersMatrix {get; set;}
    }