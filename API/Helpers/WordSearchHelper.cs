using System.Text;
using System.Collections;
using System.Diagnostics;

namespace service_matrix.Helpers
{
    /// <summary>
    /// Optimized word search helper with backtracking and memoization
    /// </summary>
    public class WordSearchHelper
    {
        private readonly string[,] _arLettersStatic;
        private readonly string[] _arWord;
        private readonly string _sWord;
        private readonly int _matrixSize;
        private readonly HashSet<(int, int)> _visited;
        private readonly StringBuilder _sFoundString;
        private readonly Dictionary<string, bool> _memoCache;
        private readonly List<(int, int)> _currentPath;
        private readonly List<(int, int)> _bestPath;
        private readonly int _maxDepth;
        private readonly int _maxIterations;
        private int _iterationsCount = 0;
        private bool _found = false;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private Dictionary<int, Dictionary<string, string>> _foundWord;

        /// <summary>
        /// Initialize the word search helper with optimized algorithms
        /// </summary>
        /// <param name="sWord2">The word to search for</param>
        /// <param name="arLetters2">The letter matrix</param>
        public WordSearchHelper(string sWord2, string[,] arLetters2)
        {
            _arLettersStatic = CopyArray(arLetters2);
            _arWord = sWord2.ToCharArray().Select(c => c.ToString()).ToArray();
            _sWord = sWord2;
            _matrixSize = arLetters2.GetLength(0);
            _visited = new HashSet<(int, int)>();
            _sFoundString = new StringBuilder();
            _memoCache = new Dictionary<string, bool>();
            _currentPath = new List<(int, int)>();
            _bestPath = new List<(int, int)>();
            _maxDepth = _arWord.Length;
            _maxIterations = _matrixSize * _matrixSize * 10; // Limit iterations
            _stopwatch.Start();
        }
        
        /// <summary>
        /// Find all locations of the first letter in the matrix
        /// </summary>
        /// <returns>List of coordinates where the first letter appears</returns>
        public List<(int, int)> FindLetterLocations()
        {
            string firstLetter = _arWord[0];
            var locations = new List<(int, int)>();
            
            for (int i = 0; i < _matrixSize; i++)
            {
                for (int j = 0; j < _matrixSize; j++)
                {
                    if (string.Equals(_arLettersStatic[i, j], firstLetter, StringComparison.OrdinalIgnoreCase))
                    {
                        locations.Add((i, j));
                    }
                }
            }
            return locations;
        }
        
        /// <summary>
        /// Optimized search using backtracking algorithm
        /// </summary>
        /// <returns>True if word is found, false otherwise</returns>
        public bool Search()
        {
            _iterationsCount = 0;
            _found = false;
            _bestPath.Clear();
            _currentPath.Clear();
            _memoCache.Clear();
            _visited.Clear();
            
            var startPositions = FindLetterLocations();

            foreach (var startPosition in startPositions)
            {
                if (_found) break;
                
                _currentPath.Add(startPosition);
                _visited.Add(startPosition);
                _sFoundString.Append(_arLettersStatic[startPosition.Item1, startPosition.Item2]);
                
                if (BacktrackSearch(1, startPosition.Item1, startPosition.Item2))
                {
                    _found = true;
                    _bestPath.AddRange(_currentPath);
                    break;
                }
                
                _currentPath.RemoveAt(_currentPath.Count - 1);
                _visited.Remove(startPosition);
                _sFoundString.Clear();
            }
            
            _stopwatch.Stop();
            return _found;
        }

        /// <summary>
        /// Get the word locations found during search
        /// </summary>
        /// <returns>Dictionary mapping word indices to their matrix coordinates</returns>
        public Dictionary<int, Dictionary<string, string>> GetFoundWord()
        {
            var result = new Dictionary<int, Dictionary<string, string>>();
            foreach (var position in _bestPath)
            {
                int index = _bestPath.IndexOf(position);
                string charAtPos = _arLettersStatic[position.Item1, position.Item2];
                result[index] = new Dictionary<string, string> { { charAtPos, $"{position.Item1} {position.Item2}" } };
            }
            return result;
        }

        /// <summary>
        /// Get the string formed by the search path
        /// </summary>
        /// <returns>The concatenated string from the search path</returns>
        public string GetFoundString()
        {
            var result = new StringBuilder();
            foreach (var position in _bestPath)
            {
                result.Append(_arLettersStatic[position.Item1, position.Item2]);
            }
            return result.ToString();
        }
        
        /// <summary>
        /// Optimized backtracking search with memoization
        /// </summary>
        /// <param name="currentIndex">Current position in the word being searched</param>
        /// <param name="currentRow">Current row in the matrix</param>
        /// <param name="currentCol">Current column in the matrix</param>
        /// <returns>True if word is found from this position</returns>
        private bool BacktrackSearch(int currentIndex, int currentRow, int currentCol)
        {
            _iterationsCount++;
            if (_iterationsCount > _maxIterations)
            {
                return false;
            }
            
            if (currentIndex == _arWord.Length)
            {
                return true;
            }
            
            string targetChar = _arWord[currentIndex];
            
            var neighbors = GetNeighbors(currentRow, currentCol);
            
            foreach (var neighbor in neighbors)
            {
                if (_visited.Contains(neighbor))
                {
                    continue;
                }
                
                string cellValue = _arLettersStatic[neighbor.Item1, neighbor.Item2];
                if (!string.Equals(cellValue, targetChar, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                
                var cacheKey = $"{currentIndex}-{neighbor.Item1}-{neighbor.Item2}";
                if (_memoCache.ContainsKey(cacheKey))
                {
                    continue;
                }
                
                _visited.Add(neighbor);
                _currentPath.Add(neighbor);
                _sFoundString.Append(cellValue);
                
                if (BacktrackSearch(currentIndex + 1, neighbor.Item1, neighbor.Item2))
                {
                    return true;
                }
                
                _visited.Remove(neighbor);
                _currentPath.RemoveAt(_currentPath.Count - 1);
                _sFoundString.Length -= 1;
                _memoCache[cacheKey] = false;
            }
            
            return false;
        }

        /// <summary>
        /// Get neighboring cells in the matrix (diagonal and orthogonal)
        /// </summary>
        /// <param name="row">Current row</param>
        /// <param name="col">Current column</param>
        /// <returns>List of neighbor coordinates</returns>
        private List<(int, int)> GetNeighbors(int row, int col)
        {
            var neighbors = new List<(int, int)>();
            
            for (int dRow = -1; dRow <= 1; dRow++)
            {
                for (int dCol = -1; dCol <= 1; dCol++)
                {
                    if (dRow == 0 && dCol == 0)
                    {
                        continue;
                    }
                    
                    int newRow = row + dRow;
                    int newCol = col + dCol;
                    
                    if (newRow >= 0 && newRow < _matrixSize && newCol >= 0 && newCol < _matrixSize)
                    {
                        neighbors.Add((newRow, newCol));
                    }
                }
            }
            
            return neighbors;
        }

        /// <summary>
        /// Create a deep copy of the letter matrix
        /// </summary>
        /// <param name="source">The source matrix to copy</param>
        /// <returns>A new matrix with the same values</returns>
        public static string[,] CopyArray(string[,] source)
        {
            int rows = source.GetLength(0);
            int cols = source.GetLength(1);
            string[,] copy = new string[rows, cols];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    copy[i, j] = source[i, j];
                }
            }
            return copy;
        }



        /// <summary>
        /// Check if a character exists at the given position in the matrix
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="col">Column index</param>
        /// <param name="character">Character to check</param>
        /// <returns>True if character matches</returns>
        private bool HasCharacterAt(int row, int col, string character)
        {
            if (row < 0 || row >= _matrixSize || col < 0 || col >= _matrixSize)
            {
                return false;
            }
            return string.Equals(_arLettersStatic[row, col], character, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if the next letter in the word is a neighbor of the current position
        /// </summary>
        /// <param name="iCurrentX">Current row position</param>
        /// <param name="iCurrentY">Current column position</param>
        /// <param name="arWord2">The word being searched</param>
        /// <param name="iWordIndex">Current index in the word</param>
        /// <param name="arLettersLoc">The letter matrix</param>
        /// <returns>True if the next letter is a neighbor</returns>
        public bool IsNeighborToNextLetter(int iCurrentX, int iCurrentY, string[] arWord2, int iWordIndex, string[,] arLettersLoc)
        {
            if (iWordIndex == arWord2.Length - 1 || iWordIndex == 0)
            {
                return true;
            }
            string sNextLetter = arWord2[iWordIndex + 1];
            for (int dX = 1; dX <= 3; dX++)
            {
                for (int dY = 1; dY <= 3; dY++)
                {
                    int neighborX = (iCurrentX + dX) - 2;
                    int neighborY = (iCurrentY + dY) - 2;
                    if (!(neighborX == iCurrentX && neighborY == iCurrentY) && neighborX >= 0 && neighborX <= 4 && neighborY >= 0 && neighborY <= 4 && sNextLetter.Equals(arLettersLoc[neighborX, neighborY]))
                    {
                        bool secondNextNeighbor = true;
                        if (arWord2.Length > iWordIndex + 2)
                        {
                            string[,] arLettersLocTemp = CopyArray(arLettersLoc);
                            arLettersLocTemp[iCurrentX, iCurrentY] = "*";
                            secondNextNeighbor = IsNeighborToNextLetter(neighborX, neighborY, arWord2, iWordIndex + 1, arLettersLocTemp);
                        }
                        if (secondNextNeighbor)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Get the current search path
        /// </summary>
        /// <returns>List of coordinates in the current search path</returns>
        public List<(int, int)> GetCurrentPath()
        {
            return new List<(int, int)>(_currentPath);
        }
        
        /// <summary>
        /// Determine whether all letter of the given word are in the matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="wholeWord"></param>
        /// <returns></returns>
        public static bool IsAllLettersInMatrix(String[,] matrix, String wholeWord) {
            var _allArrayLetters = new HashSet<char>();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    var charArray = matrix[i, j].ToCharArray();
                    if(charArray.Length > 0)
                        _allArrayLetters.Add(charArray[0]);
                    else
                        _allArrayLetters.Add('*');
                }
            }
            
            foreach (var c in wholeWord)
            {
                if (!_allArrayLetters.Contains(c)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Clean words by filtering and sorting
        /// </summary>
        /// <param name="input">Collection of words to clean</param>
        /// <returns>Filtered and sorted list of words</returns>
        public static IEnumerable<string> CleanWords(IEnumerable<string> input)
        {
            return input
                .Where(word => word.Length >= 8 && word.Length <= 24 && !word.Contains(' ') && !word.Contains('-'))
                .OrderByDescending(word => word.Length)
                .ToList();
        }
    }
}