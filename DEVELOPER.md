# Developer Notes

## Environment Setup
While in development, a good idea is to keep `ASPNETCORE_ENVIRONMENT` set as `Development`.
If this is your case, then keep in mind to create a new SQL database and not rely on the  
default one (e.g. `master` if you are on SQL Server) as it will most likely make `EnsureDeleted()`  
fail, resulting in an application shutdown/missing tables.

## Notes on libraries
By default, the SQL driver that underlies EntityFramework is `Microsoft.EntityFramework.SqlServer`, so  
in case of any other DBMS, install the package you need but while asking a PR, make sure the commits does not
contain any reference to the new driver.

I know it may seem strange, but this project is designed on Microsoft stack.
