FROM microsoft/dotnet:2.0-sdk as build
WORKDIR /docker
COPY ./src .
RUN dotnet build Integration.MicrosoftGraph.sln
RUN dotnet publish Integration.MicrosoftGraph.Service/Integration.MicrosoftGraph.Service.csproj --output ../www

FROM microsoft/aspnetcore:2.0 as deploy
WORKDIR /webapi
COPY --from=build /docker/www .
ENV ASPNETCORE_URLS=http://+:80/
ENV ASPNETCORE_ENVIRONMENT=string
EXPOSE 80
CMD [ "dotnet", "Integration.MicrosoftGraph.Service.dll" ]
