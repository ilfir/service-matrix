docker build --no-cache -t service-matrix .
docker run -p 8080:80  -v /Users/ilfir2/service-matrix-data:/app/data service-matrix
