FROM python:3.11-slim-bullseye

RUN apt-get update && \
    apt-get install -y gcc gpg curl unixodbc-dev libssl1.1 libstdc++6 && \
    curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > /etc/apt/trusted.gpg.d/microsoft.gpg && \
    curl https://packages.microsoft.com/config/debian/11/prod.list | tee /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y msodbcsql17 && \
    pip install pyodbc && \
    rm -rf /var/lib/apt/lists/*

COPY test.py /test.py

CMD ["python", "/test.py"]
