#!/usr/bin/bash

# script directory
script_dir=$(cd $(dirname $0);pwd)

# frontend network name
network_name=composes-frontend

# check existing
if [ -z "$(docker network ls --filter "name=^${network_name}$" --format '{{.Name}}')" ]; then
    docker network create "${network_name}" --subnet 172.31.0.0/24
fi
