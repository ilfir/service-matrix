using System.Text;
using System.Collections;
using System.Diagnostics;

namespace service_matrix.Helpers
{
    /// <summary>
    /// Optimized word search helper using precomputed indexes, 
    /// char-based matrix storage, and array-based visited tracking.
    /// </summary>
    public class WordSearchHelper : IWordSearchHelper
    {
        // Store original cell values for GetFoundString/GetFoundWord output
        private readonly string[,] _originalMatrix;
        // Store uppercased chars for fast case-insensitive matching
        private readonly char[,] _upperMatrix;
        private readonly int _rows;
        private readonly int _cols;
        private readonly char[] _wordUpperChars;
        private readonly Dictionary<char, List<_Loc>> _letterIndex;
        private readonly _Loc[][] _precomputedNeighbors;
        private readonly bool[,] _visited;
        private readonly List<_Loc> _bestPath;
        private readonly List<_Loc> _currentPath;
        private readonly int _maxIterations;
        private int _iterationsCount;
        private bool _found;

        private struct _Loc
          {
             public readonly int Row;
             public readonly int Col;
             public _Loc(int row, int col) { Row = row; Col = col; }
          }

           /// <summary>
           /// Initialize the word search helper with optimized algorithms.
           /// </summary>
           /// <param name="word">The word to search for.</param>
           /// <param name="lettersMatrix">The letter matrix.</param>
        public WordSearchHelper(string word, string[,] lettersMatrix)
           {
               _rows = lettersMatrix.GetLength(0);
               _cols = lettersMatrix.GetLength(1);
               _originalMatrix = new string[_rows, _cols];
               _upperMatrix = new char[_rows, _cols];

               // Build uppercased matrix and letter index in a single pass
               _letterIndex = new Dictionary<char, List<_Loc>>(26);
            for (int i = 0; i < _rows; i++)
               {
                for (int j = 0; j < _cols; j++)
                   {
                    var cell = lettersMatrix[i, j];
                       _originalMatrix[i, j] = cell;
                       _upperMatrix[i, j] = string.IsNullOrEmpty(cell) ? '\0' : char.ToUpperInvariant(cell[0]);

                    if (!string.IsNullOrEmpty(cell))
                       {
                        var upperChar = char.ToUpperInvariant(cell[0]);
                        if (!_letterIndex.TryGetValue(upperChar, out var list))
                           {
                            list = new List<_Loc>();
                               _letterIndex[upperChar] = list;
                           }
                        list.Add(new _Loc(i, j));
                       }
                   }
               }

               // Store word as uppercased char array for fast matching
               _wordUpperChars = word.ToUpperInvariant().ToCharArray();

               // Precompute neighbors for every cell to eliminate per-call allocations
            int totalCells = _rows * _cols;
               _precomputedNeighbors = new _Loc[totalCells][];
            for (int i = 0; i < _rows; i++)
               {
                for (int j = 0; j < _cols; j++)
                   {
                    var neighbors = new List<_Loc>();
                    for (int dRow = -1; dRow <= 1; dRow++)
                       {
                        for (int dCol = -1; dCol <= 1; dCol++)
                           {
                            if (dRow == 0 && dCol == 0) continue;
                            int newRow = i + dRow;
                            int newCol = j + dCol;
                            if (newRow >= 0 && newRow < _rows && newCol >= 0 && newCol < _cols)
                               {
                                neighbors.Add(new _Loc(newRow, newCol));
                               }
                           }
                       }
                       _precomputedNeighbors[i * _cols + j] = neighbors.ToArray();
                   }
               }

               // Use bool array for visited tracking (faster than HashSet)
               _visited = new bool[_rows, _cols];
               _bestPath = new List<_Loc>();
               _currentPath = new List<_Loc>();
               _maxIterations = _rows * _cols * (_wordUpperChars.Length + 1) * 5;
           }

           /// <summary>
           /// Find all locations of the first letter in the matrix using the precomputed index.
           /// </summary>
           /// <returns>List of coordinates where the first letter appears.</returns>
        public List<(int, int)> FindLetterLocations()
           {
            if (_wordUpperChars.Length == 0) return new List<(int, int)>();

            var firstChar = _wordUpperChars[0];
            if (_letterIndex.TryGetValue(firstChar, out var locations))
               {
                return locations.ConvertAll(p => (p.Row, p.Col));
               }
            return new List<(int, int)>();
           }

           /// <summary>
           /// Optimized search using precomputed indexes and array-based backtracking.
           /// </summary>
           /// <returns>True if word is found, false otherwise.</returns>
        public bool Search()
           {
               _iterationsCount = 0;
               _found = false;
               _bestPath.Clear();
               _currentPath.Clear();
            Array.Clear(_visited, 0, _visited.Length);

            if (_wordUpperChars.Length == 0) return false;

            var startPositions = FindLetterLocations();

            foreach (var pos in startPositions)
               {
                if (_found) break;

                   _currentPath.Add(new _Loc(pos.Item1, pos.Item2));
                   _visited[pos.Item1, pos.Item2] = true;

                if (BacktrackSearch(1, pos.Item1, pos.Item2))
                   {
                       _found = true;
                       _bestPath.AddRange(_currentPath);
                    break;
                   }

                   _currentPath.RemoveAt(_currentPath.Count - 1);
                   _visited[pos.Item1, pos.Item2] = false;
               }

            return _found;
           }

           /// <summary>
           /// Get the word locations found during search.
           /// </summary>
           /// <returns>Dictionary mapping word indices to their matrix coordinates.</returns>
        public Dictionary<int, Dictionary<string, string>> GetFoundWord()
           {
            var result = new Dictionary<int, Dictionary<string, string>>();
            foreach (var position in _bestPath)
               {
                int index = _bestPath.IndexOf(position);
                string charStr = _originalMatrix[position.Row, position.Col] ?? "";
                result[index] = new Dictionary<string, string> { { charStr, $"{position.Row} {position.Col}" } };
               }
            return result;
           }

           /// <summary>
           /// Get the string formed by the search path.
           /// </summary>
           /// <returns>The concatenated string from the search path.</returns>
        public string GetFoundString()
           {
            if (_bestPath.Count == 0) return string.Empty;

            var sb = new StringBuilder(_bestPath.Count);
            foreach (var position in _bestPath)
               {
                string cell = _originalMatrix[position.Row, position.Col];
                if (!string.IsNullOrEmpty(cell))
                    sb.Append(cell);
               }
            return sb.ToString();
           }

           /// <summary>
           /// Optimized backtracking search using precomputed neighbors and char matrix.
           /// </summary>
        private bool BacktrackSearch(int currentIndex, int row, int col)
           {
               _iterationsCount++;
            if (_iterationsCount > _maxIterations)
                return false;

            if (currentIndex >= _wordUpperChars.Length)
                return true;

            char targetChar = _wordUpperChars[currentIndex];
            var neighbors = _precomputedNeighbors[row * _cols + col];

            for (int i = 0; i < neighbors.Length; i++)
               {
                var neighbor = neighbors[i];

                if (_visited[neighbor.Row, neighbor.Col])
                    continue;

                if (_upperMatrix[neighbor.Row, neighbor.Col] != targetChar)
                    continue;

                   _visited[neighbor.Row, neighbor.Col] = true;
                   _currentPath.Add(neighbor);

                if (BacktrackSearch(currentIndex + 1, neighbor.Row, neighbor.Col))
                    return true;

                   _currentPath.RemoveAt(_currentPath.Count - 1);
                   _visited[neighbor.Row, neighbor.Col] = false;
               }

            return false;
           }

           /// <summary>
           /// Create a deep copy of the letter matrix.
           /// </summary>
           /// <param name="source">The source matrix to copy.</param>
           /// <returns>A new matrix with the same values.</returns>
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
           /// Check if a character exists at the given position in the matrix.
           /// </summary>
        private bool HasCharacterAt(int row, int col, string character)
           {
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
                return false;
            var cell = _originalMatrix[row, col];
            return string.Equals(cell, character, StringComparison.OrdinalIgnoreCase);
           }

           /// <summary>
           /// Check if the next letter in the word is a neighbor of the current position.
           /// </summary>
        public bool IsNeighborToNextLetter(int iCurrentX, int iCurrentY, string[] arWord2, int iWordIndex, string[,] arLettersLoc)
           {
            if (iWordIndex == arWord2.Length - 1 || iWordIndex == 0)
                return true;

            string sNextLetter = arWord2[iWordIndex + 1];
            for (int dX = 1; dX <= 3; dX++)
               {
                for (int dY = 1; dY <= 3; dY++)
                   {
                    int neighborX = (iCurrentX + dX) - 2;
                    int neighborY = (iCurrentY + dY) - 2;
                    if (!(neighborX == iCurrentX && neighborY == iCurrentY)
                           && neighborX >= 0 && neighborX <= 4
                           && neighborY >= 0 && neighborY <= 4
                           && sNextLetter.Equals(arLettersLoc[neighborX, neighborY]))
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
           /// Get the current search path.
           /// </summary>
           /// <returns>List of coordinates in the current search path.</returns>
        public List<(int, int)> GetCurrentPath()
           {
            return _currentPath.ConvertAll(p => (p.Row, p.Col));
           }

           /// <summary>
           /// Determine whether all letters of the given word are in the matrix.
           /// </summary>
        public static bool IsAllLettersInMatrix(String[,] matrix, String wholeWord)
           {
            var allArrayLetters = new HashSet<char>();
            for (int i = 0; i < matrix.GetLength(0); i++)
               {
                for (int j = 0; j < matrix.GetLength(1); j++)
                   {
                    var charArray = matrix[i, j].ToCharArray();
                    if (charArray.Length > 0)
                        allArrayLetters.Add(charArray[0]);
                    else
                        allArrayLetters.Add('*');
                   }
               }

            foreach (var c in wholeWord)
               {
                if (!allArrayLetters.Contains(c))
                    return false;
               }
            return true;
           }

           /// <summary>
           /// Clean words by filtering and sorting.
           /// </summary>
        public static IEnumerable<string> CleanWords(IEnumerable<string> input)
           {
            return input
                  .Where(word => word.Length >= 8 && word.Length <= 24 && !word.Contains(' ') && !word.Contains('-'))
                  .OrderByDescending(word => word.Length)
                   .ToList();
           }
    }
}
