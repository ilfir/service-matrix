docker build --no-cache -t service-matrix .
docker run -p 8080:8080  -v /Users/ilfir/service-matrix-data:/app/data service-matrix
