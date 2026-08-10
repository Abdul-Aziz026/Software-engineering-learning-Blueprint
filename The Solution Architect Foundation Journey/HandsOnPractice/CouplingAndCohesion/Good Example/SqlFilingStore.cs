using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CouplingAndCohesion.Good_Example;

public class SqlFilingStore : IFilingStore
{
    private readonly SqlConnection _db;

    public void Save(Filing f, decimal tax)
    {
        _db.open();
        _db.executeQuery();
        _db.close();
    }
}
