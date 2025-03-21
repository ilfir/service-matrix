# Service Matrix

Service Matrix is an ASP.NET Core web API project that provides a word search functionality.

## Project Structure

```
.
├── .gitignore
├── appsettings.Development.json
├── appsettings.json
├── Dockerfile
├── Program.cs
├── service-matrix.csproj
├── service-matrix.sln
├── WeatherForecast.cs
├── Controllers/
│   ├── WordSearchController.cs
│   └── DTO/
│       └── SearchRequest.cs
├── Properties/
│   └── launchSettings.json
└── bin/
└── obj/
```

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/get-started) (optional, for containerization)

### Building and Running the Project

1. Clone the repository:
    ```sh
    git clone <repository-url>
    cd service-matrix
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

3. Build the project:
    ```sh
    dotnet build
    ```

4. Run the project:
    ```sh
    dotnet run
    ```

5. Open your browser and navigate to `http://localhost:8080` to view the Swagger UI.

### Running with Docker

1. Build the Docker image:
    ```sh
    docker build -t service-matrix .
    ```

2. Run the Docker container:
    ```sh
    docker run -p 8080:80 service-matrix
    ```

3. Open your browser and navigate to `http://localhost:8080` to view the Swagger UI.

### Running with Docker on a Remote Host

1. Create a tar file of your project:
    ```sh
    tar -cvf service-matrix.tar .
    ```

2. Copy the tar file to the remote Docker host:
    ```sh
    scp service-matrix.tar <remote-user>@<remote-docker-host>:/path/to/destination
    ```

3. SSH into the remote Docker host:
    ```sh
    ssh <remote-user>@<remote-docker-host>
    ```

4. Extract the tar file:
    ```sh
    tar -xvf service-matrix.tar -C ./service-matrix
    cd ./service-matrix
    ```

5. Build the Docker image:
    ```sh
    docker build -t service-matrix .
    ```

6. Run the Docker container:
    ```sh
    docker run -p 8080:80 service-matrix
    ```

7. Open your browser and navigate to `http://<remote-docker-host>:8080` to view the Swagger UI.

Replace `<remote-user>`, `<remote-docker-host>`, and `/path/to/destination` with the appropriate values for your remote Docker host.

Here's the updated section in your `README.md`:

````markdown
### Running with Docker on a Remote Host

1. Create a tar file of your project:
    ```sh
    tar -cvf service-matrix.tar .
    ```

2. Copy the tar file to the remote Docker host:
    ```sh
    scp service-matrix.tar <remote-user>@<remote-docker-host>:/path/to/destination
    ```

3. SSH into the remote Docker host:
    ```sh
    ssh <remote-user>@<remote-docker-host>
    ```

4. Extract the tar file:
    ```sh
    tar -xvf /path/to/destination/service-matrix.tar -C /path/to/extract
    cd /path/to/extract/service-matrix
    ```

5. Build the Docker image:
    ```sh
    docker build -t service-matrix .
    ```

6. Run the Docker container:
    ```sh
    docker run -p 8080:80 service-matrix
    ```

7. Open your browser and navigate to `http://<remote-docker-host>:8080` to view the Swagger UI.

## API Endpoints

### Word Search

- **POST /WordSearch**
    - Request Body: `SearchRequest`
    - Response: `IEnumerable<string>`

Example request body:
```json
{
    "MaxLength": 5,
    "MaxWords": 10,
    "MinLength": 1,
    "lettersMatrix": ["a", "b", "c", "d", "e"]
}
```

## Configuration

Configuration settings are located in the `appsettings.json` and `appsettings.Development.json` files.

## License

This project is licensed under the MIT License.