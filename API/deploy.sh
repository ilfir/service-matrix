#!/bin/bash

cd "$(dirname "$0")"

docker build -t service-matrix -f Dockerfile .

docker run --name service-matrix -p 8080:8080 service-matrix &
