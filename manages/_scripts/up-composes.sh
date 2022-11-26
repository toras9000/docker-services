#!/usr/bin/bash

# script directory
script_dir=$(cd $(dirname $0);pwd)

# up function
compose-up() {
    name=$1
    yaml=${2:-docker-compose.yml}
    echo "Up $name"
    compose_path=${script_dir}/../$name/$yaml
    docker-compose --file "${compose_path}" up -d --wait
}

# print hyperlink
print-link() {
    url=$1
    label=${2:-${url}}
    printf "  \\e]8;;${url}\\e\\\\${label}\\e]8;;\\e\\\\\n"
}

# Prepare

bash ready-frontend-nw.sh

# Up services

#compose-up dnsmasq
compose-up ldap

compose-up registry
compose-up excalidraw
compose-up baget
compose-up kallithea
compose-up bookstack

compose-up proxy

echo
echo Service address
print-link https://bookstack.myserver.home
print-link https://kallithea.myserver.home
print-link https://baget.myserver.home
print-link https://excalidraw.myserver.home

echo
read -p "press enter to exit"
