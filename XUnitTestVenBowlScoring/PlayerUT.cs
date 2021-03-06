﻿using System;
using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Model;
using Xunit;
using VenBowlScoring.Exceptions;

namespace XUnitTestVenBowlScoring
{
    public class PlayerUT
    {
        [Fact]
        public void CreatePlayer()
        {
            Player player1 = new Player();

            Player player = new Player("Bob Johnson", "Human");

            Assert.True(player1.Name == "Player 1" && player1.PlayerType == "Bot");

            Assert.True(player.Name == "Bob Johnson" && player.PlayerType == "Human");
        }

        [Fact]
        public void HavePlayerHostAGame()
        {
            Player player1 = new Player();
            player1.HostGame("First Game");
            Assert.True(player1.Name == "Player 1" && player1.PlayerType == "Bot");
            Assert.True(null !=player1.CurrentGame);
            Assert.True(player1.CurrentGame.Name == "First Game");
        }

        [Fact]
        public void ConfirmPlayer1JoinedTheGame()
        {
            Player player1 = new Player();
            player1.HostGame("First Game");
            Assert.True(player1.Name == "Player 1" && player1.PlayerType == "Bot");
            Assert.True(null != player1.CurrentGame);
            Assert.True(player1.CurrentGame.Name == "First Game");
            Assert.Contains(player1, player1.CurrentGame.JoinedPlayers);
        }



        [Fact]
        public void ConfirmPlayerCanNotPlayGameWhenInSetupPhase()
        {
            Player player1 = new Player();
            player1.HostGame("First Game");
            try
            {
                player1.CurrentGame.Play();
            }
            catch (GameNotStartedException ex)
            {
                Assert.True(null != ex);

            }
            Assert.True(player1.Name == "Player 1" && player1.PlayerType == "Bot");
            Assert.True(null != player1.CurrentGame);
            Assert.True(player1.CurrentGame.Name == "First Game");
            Assert.Contains(player1, player1.CurrentGame.JoinedPlayers);
            Assert.True(player1.CurrentGame.CurrentPhase.CurrentPhase == "Setup");
            
        }


        [Fact]
        public void ConfirmPlayerCanPlayGameWhenInPlayPhase()
        {
            Player player1 = new Player();
            player1.HostGame("First Game");
            Assert.True(player1.CurrentGame.CurrentPhase.CurrentPhase == "Setup");
            player1.CurrentGame.StartGame();
            try
            {
                player1.CurrentGame.Play();
            }
            catch (GameNotStartedException ex)
            {
                Assert.True(null != ex);

            }
            Assert.True(player1.Name == "Player 1" && player1.PlayerType == "Bot");
            Assert.True(null != player1.CurrentGame);
            Assert.True(player1.CurrentGame.Name == "First Game");
            Assert.Contains(player1, player1.CurrentGame.JoinedPlayers);

        }

        [Fact]
        public void ConfirmPlayerCanScorePoints()
        {
            Player player1 = new Player();
            player1.HostGame("First Game");
            Assert.True(player1.CurrentGame.CurrentPhase.CurrentPhase == "Setup");
            player1.CurrentGame.StartGame();

            Assert.True(player1.Name == "Player 1" && player1.PlayerType == "Bot");
            Assert.True(null != player1.CurrentGame);
            Assert.True(player1.CurrentGame.Name == "First Game");
            Assert.Contains(player1, player1.CurrentGame.JoinedPlayers);
        }


        [Fact]
        public void ConfirmSampleTestCase()
        {

            Player player = new Player();
            player.HostGame("Brad's Bowling!");
            player.CurrentGame.StartGame(new List<int>() { 4,3,7,3,5,2,8,1,4,6,2,4,8,0,8,0,8,2,10,1,7 });
            string b = player.ReviewHistory();
            Assert.Contains("Score: 110", b);
        }
    }
}
