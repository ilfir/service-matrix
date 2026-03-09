docker build --no-cache -t service-matrix .
docker run --name service-matrix -p 8080:8080 service-matrix &
