# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ["SkillSwap.csproj", "./"]
RUN dotnet restore "SkillSwap.csproj"

# Copiar todo el código y compilar
COPY . .
RUN dotnet build "SkillSwap.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "SkillSwap.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Exponer el puerto (Render usa variable PORT)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Entrypoint
ENTRYPOINT ["dotnet", "SkillSwap.dll"]
