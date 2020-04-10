using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML2.CSP
{
    public class Domain
    {
        public List<int> Values { get; protected set; }

        public bool[] Mask { get; protected set; }
        public int MaskedSize { get; protected set; }
        protected bool maskClear;

        public Domain(int lowerBoud, int upperBoud)
        {
            Values = new List<int>();
            Mask = new bool[upperBoud - lowerBoud + 1];
            for (int i = lowerBoud; i <= upperBoud; i++)
            {
                Mask[i] = true;
                Values.Add(i);
            }
            maskClear = true;
            MaskedSize = Values.Count;
        }

        public Domain(List<int> values)
        {
            Values = values;
            Mask = new bool[Values.Count];
            for (int i = 0; i < Values.Count; i++)
            {
                Mask[i] = true;
            }
            maskClear = true;
            MaskedSize = Values.Count;
        }

        public void HideValueByPosition(int pos)
        {
            maskClear = false;
            Mask[pos] = false;
            MaskedSize--;
        }
        public void HideValueByValue(int val)
        {
            for(int i = Values.Count-1; i >= 0; i--)
            {
                if (Values[i] == val)
                {
                    maskClear = false;
                    Mask[i] = false;
                    MaskedSize--;
                    return;
                }
            }
        }

        public void ResetMask()
        {
            if (maskClear)
                return;
            for (int i = Mask.Length - 1; i >= 0; i--)
            {
                Mask[i] = true;
            }
            maskClear = true;
            MaskedSize = Values.Count;
        }

        public void RemoveValueByPosition(int pos)
        {
            Values.RemoveAt(pos);
            ResetMask();
        }

        public override string ToString()
        {

            if (Values.Count > 0)
            {
                string str = $"[{Values[0]}";
                for (int i = 1; i < Values.Count; i++)
                {
                    str += $", {Values[i]}";
                }
                return str + ']';
            }
            return "[]";
        }
    }
}
