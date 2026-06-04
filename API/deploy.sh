#!/bin/bash
# Deploy the service matrix Docker container
GIT_SHA=${1:-$(git rev-parse HEAD)}
docker build --build-arg GIT_SHA="$GIT_SHA" -t service-matrix .
docker rm -f service-matrix 2>/dev/null || true
docker run --name service-matrix -p 8080:8080 -e GIT_SHA="$GIT_SHA" service-matrix &