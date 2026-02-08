#!/usr/bin/env bash

cd /data

mkdir -p .cache
chown -R 1000:1000 .cache
chmod 775 .cache
chmod g+s .cache

forgejo-runner register "$@"
