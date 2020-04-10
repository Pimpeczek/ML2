using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2.CSP
{
    public class Variable
    {
        public int R { get; protected set; }
        public int C { get; protected set; }

        public bool Set { get; set; }

        public int DomainSize
        {
            get
            {
                return Domain.Values.Count;
            }
        }
        public Domain Domain { get; protected set; }

        public Variable(int r, int c, Domain domain)
        {
            R = r;
            C = c;
            Domain = domain;
        }

        public override string ToString()
        {
            return $"RC=[{R},{C}]|D={Domain}";
        }
    }
}
