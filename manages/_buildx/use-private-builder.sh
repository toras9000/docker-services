#!/usr/bin/bash

# Script directory
script_dir=$(cd $(dirname $0);pwd)

# If terminal is mintty, use winpty as adapter.
if [ "${TERM_PROGRAM,,}" = "mintty" ]; then
    adapter_tty=winpty
fi

# Builder name
builder_name=private-builder

# Check if a build has already been created. It is assumed that it does not exist if an inspect error occurs.
if [ "$(docker buildx inspect "$builder_name" > /dev/null 2>&1; echo $?)" = "0" ]; then
    echo Already created.
else
    # The relative path described in toml seems to be the current directory reference at the time of creation.
    # Move the current to match the relative from the file and the relative at the time of creation.
    cd "$script_dir/configs"
    # Create builder.
    echo Create private builder
    $adapter_tty docker buildx create \
        --name "$builder_name" \
        --platform linux/amd64,linux/arm64,linux/arm/v7 \
        --config "buildkit.toml"
fi

# Use builder
$adapter_tty docker buildx use "$builder_name"
if [ $? -gt 0 ]; then
    read -p "Press [Enter] key to exit."
fi
