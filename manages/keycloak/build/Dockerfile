ARG KEYCLOAK_VERSION=keycloak-version
FROM quay.io/keycloak/keycloak:${KEYCLOAK_VERSION} AS builder

ARG KC_HEALTH_ENABLED
ARG KC_METRICS_ENABLED
ARG KC_DB

WORKDIR /opt/keycloak
RUN /opt/keycloak/bin/kc.sh build


ARG KEYCLOAK_VERSION
FROM quay.io/keycloak/keycloak:${KEYCLOAK_VERSION}

ARG KC_HEALTH_ENABLED
ARG KC_METRICS_ENABLED
ARG KC_DB

ENV KC_HEALTH_ENABLED=$KC_HEALTH_ENABLED
ENV KC_METRICS_ENABLED=$KC_METRICS_ENABLED
ENV KC_DB=$KC_DB

COPY --from=builder /opt/keycloak/ /opt/keycloak/

ENTRYPOINT []
CMD ["/opt/keycloak/bin/kc.sh", "start", "--optimized"]
