FROM mcr.microsoft.com/mssql/server:2022-latest

# Accept EULA
ENV ACCEPT_EULA=Y
# Set SA password (users should override this via -e MSSQL_SA_PASSWORD)
ENV MSSQL_SA_PASSWORD=YourStrong!Passw0rd
# Enable SQL Server Agent
ENV MSSQL_AGENT_ENABLED=true

# SQL Server runs as mssql user by default
USER mssql

EXPOSE 1433

# Health check
HEALTHCHECK --interval=15s --timeout=3s --start-period=30s --retries=3 \
  CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -Q "SELECT 1" || exit 1
