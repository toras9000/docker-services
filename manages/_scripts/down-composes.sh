#!/usr/bin/bash

# script directory
script_dir=$(cd $(dirname $0);pwd)

# down function
compose-down() {
    echo "Down $1"
    compose_path=${script_dir}/../$1/${2:-docker-compose.yml}
    docker-compose --file "${compose_path}" down --remove-orphans
}

# Down services

compose-down proxy

compose-down bookstack
compose-down kallithea
compose-down baget
compose-down excalidraw
compose-down registry

compose-down ldap
#compose-down dnsmasq
