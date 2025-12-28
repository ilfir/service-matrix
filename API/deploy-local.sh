docker build --no-cache -t service-matrix .
docker run -p 8080:80  service-matrix
