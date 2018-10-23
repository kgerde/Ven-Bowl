using System;
using System.IO;
using System.Threading;
using VenBowlScoring.Model;

namespace Ven_BowlCMD
{

    /// <summary>
    /// quick and dirty asci cmd line gui.
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
            Input conIn = new Input();
            TextWriter conOut = Console.Out;
            string yesNo = "(Y/N)";
            conOut.WriteLine("Hello Brad.");
            conOut.Flush();
            Thread.Sleep(1000);

            conOut.Flush();
            Thread.Sleep(1000);

            conOut.WriteLine("Would you like to play a game of Chess? " + yesNo);
            if (conIn.ConfirmYes())
            {
                conOut.WriteLine("How about we bowl instead? " + yesNo);
            }
            else
            {
                conOut.WriteLine("How about we bowl? " + yesNo);
            }

            while (true)
            {
                if (conIn.ConfirmYes())
                {

                    conOut.WriteLine("Are you sure you do not want to bowl? " + yesNo);
                }
                else
                {
                    conOut.WriteLine("As you wish. Bye Bye.");
                    Environment.Exit(0);
                }

                Player player = new Player("Brad", "Bot");
                player.HostGame("Brad's Bowling!");
                player.CurrentGame.StartGame();
                conOut.WriteLine(player.ReviewHistory());

                conOut.WriteLine("Would you like to enter your own list of numbers? " + yesNo);

                if (conIn.ConfirmYes())
                {
                    conOut.WriteLine("I will duplicate the numbers to fill the rest of the balls for you.");
                    conOut.WriteLine("(Example: 1, 2, 3, 0, -10, 4, 5, 6)");
                    player.HostGame("Brad's Bowling!");
                    player.CurrentGame.StartGame(conIn.GetNumberList());
                    conOut.Flush();
                    conOut.WriteLine(player.ReviewHistory());


                }
                else
                {
                    conOut.WriteLine("As you wish. Another Game?" + yesNo);
                }
            }
        }
    }
}


