FROM node:lts-alpine AS builder

# Set environment variables for non-interactive npm installs
ENV NPM_CONFIG_LOGLEVEL=warn
ENV CI=true

RUN apk add --no-cache git

ARG IT_TOOLS_VERSION=main

WORKDIR /work/repo
RUN git init
RUN git remote add origin https://github.com/CorentinTh/it-tools.git
RUN git fetch --depth 1 origin "${IT_TOOLS_VERSION}"
RUN git checkout FETCH_HEAD

COPY ./assets/patches /work/patches
RUN git apply -p1 /work/patches/*

WORKDIR /work/app
RUN cp /work/repo/package.json    .
RUN cp /work/repo/pnpm-lock.yaml  .

RUN npm install -g pnpm && pnpm i --frozen-lockfile
RUN cp -RT /work/repo  .
RUN pnpm build


FROM nginx:stable-alpine AS production

COPY --from=builder /work/app/dist         /usr/share/nginx/html
COPY --from=builder /work/repo/nginx.conf  /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
