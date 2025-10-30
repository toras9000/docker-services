#!/usr/bin/with-contenv bash

# Add CA cert
cat /work/certs/ca/ca.crt >> /etc/ssl/certs/ca-certificates.crt

# Copy the theme template at first startup.
if [ -d /assets/template/themes/my-theme ] && [ ! -e /config/www/themes/my-theme ]; then
    echo Copy theme template
    mkdir -p /config/www/themes
    cp -RT /assets/template/themes/my-theme    /config/www/themes/my-theme
fi
