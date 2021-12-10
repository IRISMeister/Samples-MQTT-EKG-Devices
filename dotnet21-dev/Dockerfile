FROM mcr.microsoft.com/dotnet/core/sdk:2.1

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
ENV ISC_PACKAGE_INSTALLDIR /usr/irissys
ENV GWLIBDIR lib
ENV ISC_LIBDIR ${ISC_PACKAGE_INSTALLDIR}/dev/dotnet/bin/Core21

WORKDIR /source
COPY --from=store/intersystems/iris-community:2020.4.0.547.0 $ISC_LIBDIR/*.nupkg $GWLIBDIR/
COPY build.sh .
