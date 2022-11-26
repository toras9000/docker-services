#!/usr/bin/bash

# Script directory
script_dir=$(cd $(dirname $0);pwd)

# If terminal is mintty, use winpty as adapter.
if [ "${TERM_PROGRAM,,}" = "mintty" ]; then
    adapter_tty=winpty
fi

# Builder name
builder_name=private-builder

# Delete builder
$adapter_tty docker buildx rm "$builder_name"

read -p "Press [Enter] key to exit."
