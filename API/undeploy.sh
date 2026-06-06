#!/bin/bash
# Undeploy the service matrix Docker container
docker rm -f service-matrix 2>/dev/null || echo "Container not running"
echo "Service matrix undeployed."