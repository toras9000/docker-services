#!/usr/bin/bash

# If terminal is mintty, use winpty as adapter.
if [ "${TERM_PROGRAM,,}" = "mintty" ]; then
    adapter_tty=winpty
fi

read -p "USERNAME:" HTUSER
$adapter_tty docker run -it --rm httpd:2 htpasswd -nB "$HTUSER"

read -p "Press [Enter] key to exit."
