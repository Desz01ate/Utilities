# Utilities
Some heavily repetitive code block packed together as utilities tools including
- SQL execution for both DML/DDL on SQL Server (planing to expand to another database like Oracle, MySQL as well as other NoSQL)
- HTTP Request execution
- Basic Regular Expression pattern matching
- Basic Machine Learning wrapper on Microsoft.ML

# How do I get it
- Machine Learning : https://www.nuget.org/packages/Deszolate.MachineLearning/
- Utilities : https://www.nuget.org/packages/Deszolate.Utilities/
- Utilities.Lite (Lite version of Utilities) : https://www.nuget.org/packages/Deszolate.Utilities.Lite/

# How to use - Utilities, Utilities.Lite
A lot of methods do not need an instantiate as it is already a static method, exclude SQL Module which is 
primary design to hold the connection until you decide to Dispose the connection.

example of SQL module consumption
```
using(var connection = new Utlities.SQL.SQLServer("your connection string")){
  /** you can access execution method via connection instant **/
}
```

example of SQL module custom implementation by using DatabaseConnector<T1,T2> 
which T1 is a DbConnection and T2 is a DbParameter
You can use some exists driver like System.Data.SqlClient, MySql.Data.MySqlClient
or implement them yourself

```
public class SQLServer : DatabaseConnector<SqlConnection,SqlParameter>{
  public SQLServer(string connectionString) : base(connectionString)
  {
    /** do some custom initialization **/
  }
}
```

and DatabaseConnector also implement IDatabaseConnector which give you an oppotunity to implement the connector yourself like
```
class MyCustomConnector : Utilities.Interfaces.IDatabaseConnector<T1,T2>{
  /** implement the interface down there **/
}
```

# How to use - MachineLearning
```
coming soon.
```
