using System;
using System.Collections.Generic;

namespace VoyaReporting.Models
{
    public class ChildGroupedDomoData : Dictionary<string,
                Tuple<
                    Dictionary<string, string>,
                    List<Dictionary<string, string>>>>
    {
    }
}