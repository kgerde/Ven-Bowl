using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Constants;
using VenBowlScoring.Exceptions;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// This class lets bowling notation possible without having to worring about the text conversions later.
    /// </summary>
    public class BowlingNumber
    {
        private const int NUMBER_OF_PINS = Constant.NUMBER_OF_PINS;
        #region properties
        /// <summary>
        /// Holds the special chars keyed by their equivlant values. -1 denotes a fault.
        /// </summary>
        private Dictionary<int, string> _specialBowlingValues = new Dictionary<int, string>()
        {
            { Constant.FAULT_VALUE, "F" },
            { 0,"G" },
            //spare has to be handled at the frame level.
            { 10,"X" }
        };

        /// <summary>
        /// Create Text for the special ascii conversion required for bowling special chars. Note: Spares are a frame concept so they must be handled at the frame level.
        /// </summary>
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            private set
            {
                _text = value;
            }
        }

        /// <summary>
        /// Value is the numeric score used for mathmatics.
        /// </summary>
        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value > NUMBER_OF_PINS && value < Constant.FAULT_VALUE)
                {
                    StringBuilder errorMessage = new StringBuilder();
                    errorMessage.AppendFormat("Bowling scores must be between {0} and {1}. {2} can be used in the case of a Fault. The score provided is outside this range", 0, Constant.NUMBER_OF_PINS, Constant.FAULT_VALUE);
                    throw new InvalidBallScoreException(errorMessage.ToString());
                }
                if (_specialBowlingValues.ContainsKey(value))
                {
                    Text = _specialBowlingValues[value];
                    //resove the fault -1 case.
                    if (value < 0)
                    {
                        value = 0;
                    }
                    _value = value;
                }
                else
                {
                    _text = value.ToString();
                    _value = value;
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
        /// <summary>
        /// Sums up the values found in two bowling number objects and returns that value.
        /// </summary>
        /// <param name="addend">first bowling number object to add.</param>
        /// <param name="addend2">second bowling number object to add.</param>
        /// <returns>the sum of the values of the two inputs.</returns>
        public static int Sum(BowlingNumber addend, BowlingNumber addend2)
        {
            return addend.Value + addend2.Value;

        }

        /// <summary>
        /// confirms a strike. Also known as all pins are down with only one ball thrown.
        /// </summary>
        /// <returns>true for all pins down and false for any other value.</returns>
        public bool IsStrike()
        {

            return BowlingNumber.Sum(this, this) / 2 == Constant.NUMBER_OF_PINS;
        }

        /// <summary>
        /// confirms a Fault. Also known as a failure to fallow the rules resulting in a 0 score.
        /// </summary>
        /// <returns>true for a fault value and false for any other value.</returns>
        public bool IsFault()
        {
            return Text == _specialBowlingValues[Constant.FAULT_VALUE];
        }

        #endregion
    }
}
