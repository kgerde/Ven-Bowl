using System.Collections.Generic;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// This class lets bowling notation possible without having to worring about the text conversions later.
    /// </summary>
    class BowlingNumber
    {
        #region properties
        /// <summary>
        /// Holds the special chars keyed by their equivlant values. -1 denotes a fault.
        /// </summary>
        private Dictionary<int, string> SpecialBowlingValues = new Dictionary<int, string>()
        {
            { -1, "F" },
            { 0,"G" },
            { 9,"/" },
            { 10,"X" }
        };

        /// <summary>
        /// Create Text for the special ascii conversion required for bowling special chars.
        /// </summary>
        private string _Text;
        public string Text {
            get
            {
                return _Text;
            }
            private set
            {
                _Text = value;
            }
        }

        /// <summary>
        /// Value is the numeric score used for mathmatics.
        /// </summary>
        private int _Value;
        public int Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (SpecialBowlingValues.ContainsKey(value))
                {
                    Text = SpecialBowlingValues[value];
                    //resove the fault -1 case.
                    if(value < 0)
                    {
                        value = 0;
                    }
                    _Value = value;
                }
                else
                {
                    _Text = value.ToString();
                    _Value = value;
                }
            }
        }

        /// <summary>
        /// Image is used for a graphical display of the special chars
        /// <TODO>
        /// Image to be implemneted into V2.
        /// </TODO>
        /// </summary>
        public Image Image;
        #endregion

        #region methods
        public static int Sum(BowlingNumber addend , BowlingNumber addend2)
        {
            return addend.Value + addend2.Value;

        }
        #endregion
    }
}
