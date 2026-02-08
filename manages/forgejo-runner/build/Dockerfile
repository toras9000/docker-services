ARG RUNNER_VERSION=latest
FROM code.forgejo.org/forgejo/runner:${RUNNER_VERSION}

USER root
COPY --from=certs root_ca.crt  /tmp/root_ca.crt
RUN cat /tmp/root_ca.crt >> /etc/ssl/certs/ca-certificates.crt
USER 1000:1000
