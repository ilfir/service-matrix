#!/bin/bash

# Stop and remove the existing container if it is running or exists
echo "Stopping and removing existing service-matrix container..."
docker stop service-matrix || true
docker rm service-matrix || true

cd "$(dirname "$0")"

docker build -t service-matrix -f Dockerfile .

docker run --name service-matrix -p 8080:8080 service-matrix &
