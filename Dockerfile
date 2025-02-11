ARG IMAGE=store/intersystems/iris-community:2021.2.0.617.0
FROM $IMAGE

USER root

# Japanese language pack 
RUN apt -y update \
 && DEBIAN_FRONTEND=noninteractive apt -y install language-pack-ja-base language-pack-ja ibus-mozc 

# installing vim just for convenience
RUN apt -y update \
 && DEBIAN_FRONTEND=noninteractive apt -y install build-essential vim mosquitto-clients \
 && apt clean

RUN mkdir /opt/irisbuild && chown irisowner:irisowner /opt/irisbuild

WORKDIR /opt/irisbuild

USER irisowner
COPY ./src ./src/
COPY ./Installer.cls ./
COPY iris.script iris.script

RUN iris start IRIS \
	&& iris session IRIS < iris.script \
    && iris stop IRIS quietly

#COPY ./c-src /home/irisowner/c-src
