using System;

namespace ClassifiedConsole.Runtime
{
    public struct NumberString
    {
        private const char _0 = '0';
        private const char _1 = '1';
        private const char _2 = '2';
        private const char _3 = '3';
        private const char _4 = '4';
        private const char _5 = '5';
        private const char _6 = '6';
        private const char _7 = '7';
        private const char _8 = '8';
        private const char _9 = '9';
        private const char __ = '-';

        private bool intMin;
        private static int[] intMinArray = new int[]
        {
            2,1,4,7,4,8,3,6,4,8
        };
        private bool intMax;
        private static int[] intMaxArray = new int[]
        {
            2,1,4,7,4,8,3,6,4,7
        };

        public long Number
        {
            get;
            private set;
        }

        private bool Nagetive
        {
            get; set;
        }

        public NumberString(long i)
        {
            this.Number = i;
            this.intMin = this.Number == int.MinValue;
            this.intMax = this.Number == int.MaxValue;
            this.Nagetive = i < 0;
            this._Length = -1;
        }

        private int CulcalateLength(long number)
        {
            bool isMinus = number < 0;
            int length = 0;
            if (number == 0)
            {
                return 1;
            }
            while (number != 0)
            {
                length++;
                number /= 10;
            }
            if (isMinus)
            {
                length++;
            }
            return length;
        }

        private int _Length;
        public int Length
        {
            get
            {
                if (this._Length == -1)
                {
                    this._Length = this.CulcalateLength(this.Number);
                }
                return this._Length;
            }
        }

        /// <summary>
        /// 倒叙
        /// </summary>
        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Length)
                {
                    throw new System.IndexOutOfRangeException($"[NumberString] {index} => {this.Length}");
                }

                long ret = 0;
                if (this.Nagetive && index == this.Length - 1)
                {
                    return __;
                }

                if (intMax)
                {
                    ret = intMaxArray[intMaxArray.Length - index - 1];
                }
                else if (intMin)
                {
                    ret = intMinArray[intMaxArray.Length - index - 1];
                }
                else
                {
                    var pow = (long)this.Number % (long)System.Math.Pow(10, index + 1);
                    ret = (long)pow / (long)System.Math.Pow(10, index);
                    ret = Math.Abs(ret);
                }

                switch (ret)
                {
                    case 0:
                        return _0;
                    case 1:
                        return _1;
                    case 2:
                        return _2;
                    case 3:
                        return _3;
                    case 4:
                        return _4;
                    case 5:
                        return _5;
                    case 6:
                        return _6;
                    case 7:
                        return _7;
                    case 8:
                        return _8;
                    case 9:
                        return _9;
                    default:
                        throw new System.Exception($"{this.Number} => {index} => {this.Length} => {ret}");
                }
            }
        }
    }
}