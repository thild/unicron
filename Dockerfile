
# Sample contents of Dockerfile
# Stage 1
FROM microsoft/aspnetcore-build:2.1.300-preview1 AS builder

# set up node
ENV NODE_VERSION 8.9.4
ENV NODE_DOWNLOAD_SHA 21fb4690e349f82d708ae766def01d7fec1b085ce1f5ab30d9bda8ee126ca8fc
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz

ENV BUILD_DEPS='xz-utils bzip2'

RUN set -x \
    && apt-get update && apt-get install -y $BUILD_DEPS --no-install-recommends \
    && rm -rf /var/lib/apt/lists/* \
    && curl -SL "$NODE_DOWNLOAD_URL" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && npm install -g bower gulp \
    && echo '{ "allow_root": true }' > /root/.bowerrc \
    && apt-get purge -y --auto-remove $BUILD_DEPS

WORKDIR /source

# caches restore result by copying csproj file separately
COPY *.csproj .
RUN dotnet restore

# copies the rest of your code
COPY . .
RUN dotnet publish --output /app/ --configuration Release

RUN mkdir /app/Data
COPY /Data/*.* /app/Data/

# Stage 2
FROM microsoft/aspnetcore:2.1.0-preview1

# set up node
ENV NODE_VERSION 8.9.4
ENV NODE_DOWNLOAD_SHA 21fb4690e349f82d708ae766def01d7fec1b085ce1f5ab30d9bda8ee126ca8fc
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz

ENV BUILD_DEPS='xz-utils bzip2'

RUN set -x \
    && apt-get update && apt-get install -y $BUILD_DEPS --no-install-recommends \
    && rm -rf /var/lib/apt/lists/* \
    && curl -SL "$NODE_DOWNLOAD_URL" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && npm install -g bower gulp \
    && echo '{ "allow_root": true }' > /root/.bowerrc \
    && apt-get purge -y --auto-remove $BUILD_DEPS
    

ENV DEBIAN_FRONTEND noninteractive 
RUN apt-get update && apt-get install -y locales

## Set LOCALE to UTF8 and TIMEZONE to America/Sao_Paulo
RUN echo "America/Sao_Paulo" > /etc/timezone && \
    dpkg-reconfigure -f noninteractive tzdata && \
    sed -i -e 's/# en_US.UTF-8 UTF-8/en_US.UTF-8 UTF-8/' /etc/locale.gen && \
    sed -i -e 's/# pt_BR.UTF-8 UTF-8/pt_BR.UTF-8 UTF-8/' /etc/locale.gen && \
    echo 'LANG="pt_BR.UTF-8"'>/etc/default/locale && \
    dpkg-reconfigure --frontend=noninteractive locales && \
    update-locale LANG=pt_BR.UTF-8

ENV LANG pt_BR.UTF-8
ENV LANGUAGE pt_BR.UTF-8
ENV LC_ALL pt_BR.UTF-8

WORKDIR /app
COPY --from=builder /app .

RUN ls -la .

ENTRYPOINT ["dotnet", "unicron.dll"]
