#!/usr/bin/env bash

cd /data

if [ -f .runner ] && [ -d .cache ]; then
    echo 'Already initialized'
    exit 0;
fi

touch .runner
mkdir -p .cache

chown -R 1000:1000 .runner
chown -R 1000:1000 .cache
chmod 775 .runner
chmod 775 .cache
chmod g+s .runner
chmod g+s .cache

