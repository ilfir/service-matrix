# Service Matrix - Project Context

## Project Overview
**Service Matrix** is an ASP.NET Core web API (net10.0) that provides word search functionality using a letter matrix. The project demonstrates efficient word searching with backtracking algorithms and memoization.

## Technology Stack
- **Framework**: .NET 10.0 (Web API)
- **API Documentation**: Swagger/OpenAPI (Swashbuckle 10.1.4)
- **Key Features**: Command/Query pattern, Validation, File-based data storage, Docker support
- **Target Audience**: Word search games, dictionary applications

## Project Structure

### API Layer
```
API/
├── Controllers/
│   └── WordSearchController.cs  # Main API endpoints (7 endpoints)
├── Commands/
│   ├── WordSearchCommand.cs      # Command for word search
│   ├── UpdateWordsCommand.cs     # Command for updating words
│   └── MergeWordsCommand.cs      # Command for merging dictionaries
├── Queries/
│   ├── GetWordsQuery.cs          # Query for retrieving words list
│   └── LookupWordQuery.cs        # Query for looking up words
├── QueryHandlers/
│   ├── GetWordsQueryHandler.cs
│   ├── LookupWordQueryHandler.cs
│   ├── WordSearchCommandHandler.cs
│   ├── UpdateWordsCommandHandler.cs
│   └── MergeWordsCommandHandler.cs (note: typo in "Megre")
├── DTO/
│   ├── SearchRequest.cs          # Request for word search
│   ├── UpdateWordsRequest.cs      # Request for updating words
│   ├── MergeResponse.cs          # Response for merge operation
│   └── LookupResultResponseItem.cs # Response for lookup
├── Validators/
│   └── RequestValidator.cs       # Request validation logic
├── Helpers/
│   ├── WordSearchHelper.cs       # Core word search algorithm
│   └── FileHelper.cs             # File I/O operations
├── data/
│   ├── include.txt              # Included words list
│   └── exclude.txt              # Excluded words list
├── resources/
│   ├── definitions.txt          # Main dictionary (backup: definitions_backup_01APR2025.txt)
│   └── merged.txt              # Merged dictionary (cleaned with hyphens/spaces removed)
└── appsettings.json / Development.json  # Configuration files

### Test Layer
```
Tests/service-matrix-tests/
├── CommandTests.cs
├── ControllerTests.cs
├── DtoTests.cs
├── FileHelperTests.cs
├── QueryHandlerTests.cs
├── ValidatorTests.cs
├── WordSearchHelperTests.cs
├── WordSearchHelperPerformanceTests.cs
└── service-matrix-tests.csproj

### Project Files
├── service-matrix.sln           # Solution file
├── Dockerfile                   # Docker configuration
├── deploy.sh                    # Deployment script
├── .gitignore                   # Git ignore rules
├── improvement_plan.md          # Feature improvement tracking
├── performance_optimization_history.md
└── validation_improvement_history.md

## Core Features

### 7 API Endpoints

#### 1. POST /Words/Search
- **Purpose**: Run word search for a given matrix
- **Request**: SearchRequest { MaxLength, MinLength, MaxWords, LettersMatrix }
- **Response**: Dictionary<int, Dictionary<string, string>> - Matrix positions for found words
- **Example**: Matrix 5x5, searching for words between 5-8 letters

#### 2. POST /Words/Update
- **Purpose**: Update include/exclude word lists
- **Request**: UpdateWordsRequest { Words, Include } (Include = true for include, false for exclude)
- **Response**: MergeResponse - Updated dictionary

#### 3. GET /Words/List?include=true
- **Purpose**: Get list of included or excluded words
- **Request**: Query parameter `include` (true for included, false for excluded)
- **Response**: List<string> - Words matching the filter

#### 4. POST /Words/Merge
- **Purpose**: Merge dictionary words with include and exclude lists
- **Request**: None
- **Response**: MergeResponse - Count of new words added

#### 5. GET /Words/CleanMerge
- **Purpose**: Clean merged.txt by removing words with hyphens or spaces, and words <0 or > 24 letters
- **Request**: None
- **Response**: String - "BEFORE: X AFTER: Y"

#### 6. GET /Words/LookupWord?word=xxx&exactMatch=false
- **Purpose**: Lookup word or part of word in all dictionaries
- **Request**: Query params: word, exactMatch (boolean)
- **Response**: LookupResultResponseItem - Word lookup results

## Data Flow Architecture

```
Client Request
       ↓
API Controller (Validation)
       ↓
Command/Query Pattern
       ↓
Handler/Validator
       ↓
Business Logic (WordSearchHelper, FileHelper)
       ↓
File I/O (data/, resources/)
```

## Key Algorithms

### WordSearchHelper.cs
- **Algorithm**: Backtracking with memoization
- **Features**:
  - Finds all occurrences of words in letter matrix
  - 8-directional movement (including diagonals)
  - Iteration limiting to prevent infinite loops
  - Memoization cache to avoid redundant searches
  - Path tracking for found words
    - Uses 8 directions: up, down, left, right, and 4 diagonals
  - Matrix size validation and bounds checking

### RequestValidator.cs
- **SearchRequest Validation**:
  - MaxLength: 1-100, MinLength: 1-100, MaxWords >= 1
  - LettersMatrix: Must be non-empty, all rows must have equal length
  
- **UpdateWordsRequest Validation**:
  - Words: Non-empty, no duplicates, max 100 chars each
  - Include: Required boolean

## Configuration

### Environment Variables & AppSettings
- **Port**: 8080 (configured in launchSettings.json)
- **CORS**: AllowAllOrigins policy enabled
- **Swagger**: Enabled at root path, XML comments included2

### Data Files
- **include.txt**: Words to include in search (default)
- **exclude.txt**: Words to exclude from search0

## Performance Characteristics

- **Optimization**: Memoization, iteration limits, efficient backtracking
- **Tested**: Performance tests exist for WordSearchHelper
- **Scalability**: Limited by file I/O and matrix size

## Development & Deployment

### Build Commands
```bash
dotnet restore
dotnet build
dotnet run
```

### Docker Usage
```bash
docker build -t service-matrix .
docker run -p 8080:80 -v /Users/ilfir2/service-matrix-data:/app/data service-matrix
```

## Dependencies

- **Swashbuckle.AspNetCore**: 10.1.4 - API documentation and Swagger UI

## Notable Details

- **Typo**: "MegreWordsCommandHandler" should be "MergeWordsCommandHandler"
- **Documentation**: XML comments enabled, Swagger uses XML docs
- **Database**: No database - all data stored in files
- **CORS**: Enabled for all origins (development/testing)
- **Language**: Records used for Commands (immutable data)
- **Null checks**: Extensive null checking in validation layer

## Files of Interest

### For Understanding Core Logic
- `API/Helpers/WordSearchHelper.cs` - The main search algorithm (367 lines)
- `API/Controllers/WordSearchController.cs` - API endpoints (108 lines)
- `API/Validators/RequestValidator.cs` - Input validation (148 lines)

### For Testing
- `Tests/service-matrix-tests/ValidatorTests.cs` - Request validation tests
- `Tests/service-matrix-tests/WordSearchHelperTests.cs` - Algorithm tests
- `Tests/service-matrix-tests/WordSearchHelperPerformanceTests.cs` - Performance tests

### For Data Management
- `API/data/include.txt` - Included words (for reference)
- `API/data/exclude.txt` - Excluded words (for reference)
- `API/resources/definitions.txt` - Main dictionary (for reference3

## Git Information
- **Repository**: https://github.com/ilfir/service-matrix.git
- **Latest Commit**: 997dfd903edfb5417c8e7c97793ccc05de9184aa

## Quick Reference

### Word Search Algorithm Steps
1. Find all occurrences of first letter in matrix
2. For each occurrence, use backtracking to find the complete word
3. Memoize visited paths to avoid redundant searches
4. Stop when all words found or iteration limit reached
5. Return path information for each found word

### Data Validation Rules
- Matrix rows must all have the same length
- Word lengths constrained (8-24 letters after cleaning)
- No duplicate words allowed in update lists

## Maintenance Notes
- Add new words to `data/include.txt` or `data/exclude.txt` for persistence
- Update `resources/definitions.txt` to add new dictionary words
- Run `CleanMerge` endpoint to clean merged.txt after data updates
- All file paths are relative to /app/data or /app/resources when running in Docker

## Task Dependencies
- Search functionality relies on WordSearchHelper algorithm
- Update functionality depends on FileHelper for persistence
- Merge functionality integrates include.txt, exclude.txt, and definitions.txt

## Future Potential (Based on Code Structure)
- Could add database backend for better persistence
- Could optimize with trie structure for faster dictionary lookups
- Could add caching layer for frequently accessed words
- Could implement pagination for large word lists

## Summary
This is a well-structured .NET web API project demonstrating:
- Clean separation of concerns (Controller, Command, Query, Handler, Validator)
- Comprehensive input validation
- Efficient algorithms with memoization
- Docker containerization support
- Extensive test coverage
- RESTful API design with Swagger documentation

**Key Strength**: The WordSearchHelper implementation shows good algorithmic thinking with backtracking, memoization, and iteration limiting for performance.