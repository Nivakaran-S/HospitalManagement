FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /source 

COPY *.csproj .
RUN dotnet restore 

COPY . . 
RUN dotnet publish -c Release -o /app/publish 

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime 
WORKDIR /app 
COPY --from=build /app/puslish . 

EXPOSE 80 

ENTRYPOINT [ "dotnet", "AppointmentService.dll" ]