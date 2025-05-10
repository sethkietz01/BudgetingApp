FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /BudgetingApp
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /BudgetingApp
COPY ["BudgetingApp.csproj", "."]
RUN dotnet restore "BudgetingApp.csproj"
COPY . .
WORKDIR "/BudgetingApp/"
RUN dotnet build "BudgetingApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BudgetingApp.csproj" -c Release -o /app/publish /p:PublishReadyToRun=true

FROM base AS final
WORKDIR /BudgetingApp
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BudgetingApp.dll"]