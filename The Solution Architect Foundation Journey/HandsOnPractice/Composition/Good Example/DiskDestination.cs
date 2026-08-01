using System;
using System.Collections.Generic;
using System.Text;

namespace Composition.Good_Example;

public class DiskDestination : IExportDestination
{
    public void Send(string content, string fileName)
    {
        throw new NotImplementedException();
    }
}
