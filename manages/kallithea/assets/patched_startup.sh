#!/bin/bash

# script dir
SCRIPT_DIR=$(cd $(dirname $0);pwd)

# python bin
PYTHON_BIN=python3

# packages path
PYTHON_PACKAGES=$(su-exec kallithea:kallithea $PYTHON_BIN -m site --user-site)

# kallithea installation directory
KALLITEHA_INSTALL_DIR=$PYTHON_PACKAGES/kallithea

# apply patch files
find "$SCRIPT_DIR/patch" -mindepth 1 -maxdepth 1 -type f -name *.patch | while read -r patch; do
  patch -d "$KALLITEHA_INSTALL_DIR" -p 2 -i "$patch"
done

# normal startup
cp "$SCRIPT_DIR/startup.sh" /kallithea/startup.sh

# normal startup
exec bash /kallithea/startup.sh

