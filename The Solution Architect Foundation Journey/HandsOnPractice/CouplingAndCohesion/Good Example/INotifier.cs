using System;
using System.Collections.Generic;
using System.Text;

namespace CouplingAndCohesion.Good_Example;

public interface INotifier
{
    public void FilingSubmitted(Filing f);
}
