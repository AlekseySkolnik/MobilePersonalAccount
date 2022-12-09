FROM mcr.microsoft.com/dotnet/aspnet:7.0 as base
ENV DOTNET_ReadyToRun=0

WORKDIR /
COPY ./webapi /webapi
COPY ./consumers /consumers
COPY ./migrator /migrator