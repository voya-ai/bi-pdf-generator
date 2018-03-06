FROM microsoft/dotnet:1.1-sdk

COPY VoyaReporting /app/VoyaReporting
WORKDIR /app/VoyaReporting
RUN ["dotnet", "restore"]

EXPOSE 5000

ENTRYPOINT ["dotnet", "run"]
