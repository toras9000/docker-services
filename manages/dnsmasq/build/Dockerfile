FROM alpine:latest

RUN apk add --no-cache dnsmasq

RUN sed -i -e 's/^\local-service$/#local-service/' /etc/dnsmasq.conf

EXPOSE 53/tcp 53/udp

CMD ["dnsmasq", "--keep-in-foreground"]
