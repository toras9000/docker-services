ARG TIMETAGGER_VERSION=version-tag
FROM ghcr.io/almarklein/timetagger:${TIMETAGGER_VERSION}

RUN python -m pip install -U ldap3
COPY ./assets/run.py  /root/run.py

CMD ["python", "/root/run.py"]
