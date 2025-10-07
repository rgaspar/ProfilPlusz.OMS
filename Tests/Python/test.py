import pyodbc

conn_str = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=sqlserver,1433;"
    "DATABASE=master;"
    "UID=sa;"
    "PWD=Your_password123"
)

try:
    with pyodbc.connect(conn_str, timeout=15) as conn:
        print("Connection successful!")
except Exception as e:
    print("Connection failed 1:", e)
