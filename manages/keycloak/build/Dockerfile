ARG KEYCLOAK_VERSION=keycloak-version
FROM quay.io/keycloak/keycloak:${KEYCLOAK_VERSION} AS builder

WORKDIR /opt/keycloak
RUN /opt/keycloak/bin/kc.sh build


ARG KEYCLOAK_VERSION
FROM quay.io/keycloak/keycloak:${KEYCLOAK_VERSION}

COPY --from=builder /opt/keycloak/ /opt/keycloak/

ENTRYPOINT []
CMD ["/opt/keycloak/bin/kc.sh", "start"]
