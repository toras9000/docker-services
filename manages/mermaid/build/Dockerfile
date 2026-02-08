FROM node:24-alpine AS builder

RUN apk --no-cache add build-base git python3
RUN corepack enable pnpm

WORKDIR /work/app
RUN git clone --depth 1 https://github.com/mermaid-js/mermaid-live-editor.git  .
RUN pnpm install

ARG MERMAID_RENDERER_URL=
ARG MERMAID_KROKI_RENDERER_URL=
ARG MERMAID_ANALYTICS_URL=
ARG MERMAID_DOMAIN=
ARG MERMAID_IS_ENABLED_MERMAID_CHART_LINKS=

RUN pnpm build


FROM nginx:stable-alpine

COPY --from=builder /work/app/nginx.conf   /etc/nginx/conf.d/default.conf
COPY --from=builder /work/app/docs         /usr/share/nginx/html
