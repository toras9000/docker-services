FROM golang:bullseye AS builder

ENV DEBIAN_FRONTEND=noninteractive 
RUN apt-get update
RUN apt-get install -y git

WORKDIR /work
RUN git clone https://github.com/iij/ngx_auth_mod.git
WORKDIR /work/ngx_auth_mod
RUN bash ./build.sh


FROM debian

COPY --from=builder /work/ngx_auth_mod/bin /opt/ngx_auth_mod

CMD ["/opt/ngx_auth_mod/ngx_ldap_auth", "/opt/ngx_auth_mod/conf.d/ngx_ldap_auth.conf"]
