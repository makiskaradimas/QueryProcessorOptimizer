using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPO.QueryTree
{
    class IntermediateData
    {
        public long numberOfIOs { get; set; }
        public long Tuples { get; set; }
        public long TupleSize { get; set; }
        public long TotalSize { get; set; }
        public Operations.SelectionApproach selApproach { get; set; }
        public Operations.ProjectionApproach prApproach { get; set; }
        public Operations.JoinApproach joinApproach { get; set; }

        public IntermediateData(long numofIOs, long tupleNum, long tplSize, long finalSize)
        {
            numberOfIOs = numofIOs;
            Tuples = tupleNum;
            TupleSize = tplSize;
            TotalSize = finalSize;
        }
    }
}
