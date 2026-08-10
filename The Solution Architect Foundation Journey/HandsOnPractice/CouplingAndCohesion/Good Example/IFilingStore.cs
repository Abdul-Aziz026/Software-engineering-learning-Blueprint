using System;
using System.Collections.Generic;
using System.Text;

namespace CouplingAndCohesion.Good_Example;

public interface IFilingStore
{
    public void Save(Filing f, decimal tax);
}
