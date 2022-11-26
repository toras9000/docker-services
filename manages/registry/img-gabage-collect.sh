#!/usr/bin/bash

# If terminal is mintty, use winpty as adapter.
if [ "${TERM_PROGRAM,,}" = "mintty" ]; then
    adapter_tty=winpty
fi

$adapter_tty docker-compose exec app registry garbage-collect /etc/docker/registry/config.yml
