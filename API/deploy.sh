docker build -t service-matrix .
docker run -p 8080:80  -v /Users/ilfir/service-matrix-data:/app/data service-matrix
