#!/bin/sh
set -eu

if [ -f "/var/www/html/cron.php" ]; then
    exec busybox crond -f -L /dev/stdout &
fi

exec apache2-foreground
