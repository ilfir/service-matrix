using System.Text;

namespace service_matrix.Helpers
{
    public class WordSearchHelper
    {
        private string[,] _arLetters;
       
        private readonly string[,] _arLettersStatic;
        private readonly string[] _arWord;
        private Dictionary<int, Dictionary<string, string>> _foundWord;
        private int _iFirstColumn = 0;
        private int _iFirstRow = 0;
        private StringBuilder _sFoundString;
        private readonly string _sWord;

        public WordSearchHelper(string sWord2, string[,] arLetters2)
        {
            _arLettersStatic = CopyArray(arLetters2);
            _arLetters = CopyArray(_arLettersStatic);
            _foundWord = new Dictionary<int, Dictionary<string, string>>();
            _sFoundString = new StringBuilder();
            _arWord = sWord2.ToCharArray().Select(c => c.ToString()).ToArray();
            _sWord = sWord2;
        }
        
        public List<(int, int)> FindLetterLocations()
        {
            string letter = _sWord.ToCharArray()[0].ToString();
            List<(int, int)> locations = new List<(int, int)>();
            for (int i = 0; i < _arLetters.GetLength(0); i++)
            {
                for (int j = 0; j < _arLetters.GetLength(1); j++)
                {
                    if (_arLetters[i, j] == letter)
                    {
                        locations.Add((i, j));
                    }
                }
            }
            return locations;
        }
        
        public bool Search()
        {
            var startPositions = FindLetterLocations();

            foreach (var startPosition in startPositions)
            { 
                _sFoundString = new StringBuilder();
                 var isWordFound = FindWord(0, startPosition.Item1, startPosition.Item2);
                 if (isWordFound)
                 {
                     return true;
                 }
            }
            return false;
        }

        public Dictionary<int, Dictionary<string, string>> GetFoundWord()
        {
            return _foundWord;
        }

        public string GetFoundString()
        {
            return _sFoundString.ToString();
        }
        
        public bool FindWord(int? iWordIndex, int? iMatrixStart, int? jMatrixStart)
        {
            if (iMatrixStart == null)
            {
                iMatrixStart = 0;
            }
            if (jMatrixStart == null)
            {
                jMatrixStart = 0;
            }
            if (iWordIndex == null)
            {
                iWordIndex = 0;
            }
            for (int w = iWordIndex.Value; w < _arWord.Length && iWordIndex.Value <= _foundWord.Count; w++)
            {
                iWordIndex = w;
                string sSearchChar = _arWord[w];
                bool bFound = false;
                for (int i = iMatrixStart.Value; i < _arLetters.GetLength(0) && !bFound; i++)
                {
                    for (int j = jMatrixStart.Value; j < _arLetters.GetLength(1) && !bFound; j++)
                    {
                        if (sSearchChar.Equals(_arLetters[i, j], StringComparison.OrdinalIgnoreCase) && IsNeighborToPrevLetter(i, j, iWordIndex.Value, sSearchChar))
                        {
                            if (IsNeighborToNextLetter(i, j, _arWord, iWordIndex.Value, CopyArray(_arLetters)))
                            {
                                var hLetLoc = new Dictionary<string, string>
                                {
                                    { sSearchChar, $"{i} {j}" }
                                };
                                _foundWord[iWordIndex.Value] = hLetLoc;
                                _sFoundString.Append(_arLetters[i, j]);
                                _arLetters[i, j] = "*";
                                bFound = true;
                                iMatrixStart = iMatrixStart == 0 ? iMatrixStart : i-1;
                                jMatrixStart = jMatrixStart == 0 ? jMatrixStart : j-1;
                                if (string.Equals(_sFoundString.ToString(), _sWord, StringComparison.OrdinalIgnoreCase))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            int tempCol = _iFirstColumn;
            int tempRow = _iFirstRow;
            UpdateNextFirstLetterStartPos();
            if (!(tempCol == _iFirstColumn && tempRow == _iFirstRow) && GetNextFirstLetter(_iFirstColumn, _iFirstRow))
            {
                return FindWord(1, null, null);
            }
            return false;
        }

        public bool GetNextFirstLetter(int iMatrixStart, int jMatrixStart)
        {
            int i = _iFirstColumn;
            int i2 = _iFirstRow;
            string sSearchChar = _arWord[0];
            bool bFound = false;
            _arLetters = CopyArray(_arLettersStatic);
            _foundWord = new Dictionary<int, Dictionary<string, string>>();
            _sFoundString = new StringBuilder();
            for (int i3 = iMatrixStart; i3 < _arLetters.GetLength(0) && !bFound; i3++)
            {
                int j = jMatrixStart;
                while (true)
                {
                    if (j >= _arLetters.GetLength(1) || bFound)
                    {
                        break;
                    }
                    if (sSearchChar.Equals(_arLetters[i3, j]))
                    {
                        if (IsNeighborToNextLetter(i3, j, _arWord, 0, CopyArray(_arLetters)))
                        {
                            _sFoundString.Append(sSearchChar);
                            var hLetLoc = new Dictionary<string, string>
                            {
                                { sSearchChar, $"{i3} {j}" }
                            };
                            _foundWord[0] = hLetLoc;
                            _arLetters[i3, j] = "*";
                            bFound = true;
                            break;
                        }
                    }
                    j++;
                }
                jMatrixStart = 0;
            }
            return bFound;
        }

        public string[,] CopyArray(string[,] source)
        {
            string[,] copy = new string[source.GetLength(0), source.GetLength(1)];
            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    copy[i, j] = source[i, j];
                }
            }
            return copy;
        }

        private void UpdateNextFirstLetterStartPos()
        {
            int tempCol = _iFirstColumn;
            int tempRow = _iFirstRow;
            if (_iFirstColumn < 5 && _iFirstRow < 5)
            {
                _iFirstRow++;
                if (_iFirstRow > 4)
                {
                    _iFirstRow %= 4;
                    _iFirstColumn++;
                }
                if (_iFirstColumn > 4 || _iFirstRow > 4)
                {
                    _iFirstColumn = tempCol;
                    _iFirstRow = tempRow;
                }
            }
        }

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

        private bool IsNeighborToPrevLetter(int iCol, int iRow, int iWordIndex, string sLet)
        {
            if (iWordIndex == 0)
            {
                _iFirstColumn = iCol;
                _iFirstRow = iRow;
                return true;
            }
            if (!_foundWord.TryGetValue(iWordIndex - 1, out var hLetterIdx))
            {
                return false;
            }
            string value = "";
            foreach (var k in hLetterIdx.Keys)
            {
                value = hLetterIdx[k];
                break;
            }
            string[] sIndex = value.Split(' ');
            int iPrevRow = int.Parse(sIndex[1]);
            int xDelta = Math.Abs(Math.Abs(int.Parse(sIndex[0])) - Math.Abs(iCol));
            int yDelta = Math.Abs(Math.Abs(iPrevRow) - Math.Abs(iRow));
            return xDelta < 2 && yDelta < 2;
        }
        
        /// <summary>
        /// Determine whether all letter of the given word are in the matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="wholeWord"></param>
        /// <returns></returns>
        public bool IsAllLettersInMatrix(String[,] matrix, String wholeWord) {
            
            List<char>? _allArrayLetters = new();
            var word = wholeWord.ToCharArray();
        
            _allArrayLetters = new List<char>();
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
            
            foreach (var c in word)
            {
                if (!_allArrayLetters.Contains(c)) {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<string> CleanWords(IEnumerable<string> input)
        {
            var output = new List<string>();
            foreach (var word in input)
            {
                if (word.Length < 8 || word.Contains(' ') || word.Contains('-'))
                {
                    continue;
                }
                output.Add(word);
            }
            output = output.OrderByDescending(word => word.Length).ToList();
            return output;
        }

    }
}