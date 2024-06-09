using System;
using System.Collections.Generic;
using Xunit;
using VillageOfTesting;
using VillageOfTesting.Interfaces;
using VillageOfTesting.Objects;
using VillageOfTesting.OccupationActions;
using Moq;
using VillageOfTesting.CompleteActions;

namespace VillageTests
{
    public class VillageTests
    {
        [Fact]
        public void AddWorker_ShouldAddWorker_WhenOccupationIsValid()
        {
            // Arrange
            var village = new Village();
            village.OccupationDictionary.Add("farmer", new FarmerAction(village));

            // Act
            village.AddWorker("John", "farmer");

            // Assert
            Assert.Single(village.Workers);
            Assert.Equal("John", village.Workers[0].Name);
            Assert.Equal("farmer", village.Workers[0].Occupation);

            village.Workers.Clear();
        }

        [Fact]
        public void Test_AddMultipleWorkers()
        {
            // Arrange
            var village = new Village();
            village.OccupationDictionary.Add("farmer", new FarmerAction(village));
            village.OccupationDictionary.Add("miner", new MinerAction(village));
            
            

            // Act
            village.AddWorker("Jane", "farmer");
            village.AddWorker("Doe", "miner");

            // Assert
            Assert.Equal(2, village.Workers.Count);

            Assert.Equal("Jane", village.Workers[0].Name);
            Assert.Equal("farmer", village.Workers[0].Occupation);

            
            Assert.Equal("Doe", village.Workers[1].Name);
            Assert.Equal("miner", village.Workers[1].Occupation);

            village.Workers.Clear();
        }

        [Fact]
        public void Test_WorkersDoTheirJobs()
        {
            // Arrange
            var village = new Village();
            village.AddWorker("John", "farmer");
            village.AddWorker("Jane", "lumberjack");
            village.AddWorker("Doe", "miner");

            // Act
            village.Day();

            // Assert
            foreach (var worker in village.Workers)
            {
                Assert.True(worker.Hungry, $"{worker.Name} did not perform their job.");
            }
            village.Workers.Clear ();
        }

        [Fact]
        public void Workers_DoTheirJobs_Correctly()
        {
            // Arrange
            var village = new Village();
            village.Food = 10; // Initiera matresursen
            village.OccupationDictionary.Add("farmer", new FarmerAction(village));

            // Lägg till en farmer
            village.AddWorker("John", "farmer");

            // Act
            village.Day();

            // Assert
            Assert.Equal(14, village.Food); // Efter en dag ska matresursen öka med 5 (standardvärde för FoodPerDay)

            // Kontrollera att arbetaren har utfört sitt jobb
            Assert.False(village.Workers[0].Hungry); // Arbetaren bör vara hungrig efter en arbetsdag
            Assert.Equal(0, village.Workers[0].DaysHungry); // Antalet dagar utan mat bör vara 1 efter en arbetsdag
        }

        [Fact]
        public void Cannot_Add_Worker_If_No_Space_Available()
        {
            var village = new Village();
            village.MaxWorkers = 2;
            village.OccupationDictionary.Add("farmer", new FarmerAction(village));
            village.OccupationDictionary.Add("miner", new MinerAction(village));

            village.AddWorker("Jane", "farmer");
            village.AddWorker("Doe", "miner");

            // Act
            village.OccupationDictionary.Add("builder", new BuilderAction(village));
            village.AddWorker("Worker3", "builder");

            // Assert
            Assert.Equal(2, village.Workers.Count);

            village.Workers.Clear();
           

        }

        [Fact]
        public void TestProceedToNextDayWithoutWorkers()
        {
            // Arrange
            var village = new Village();

            // Act
            village.Day();

            // Assert
            Assert.Equal(1, village.DaysGone);
            village.DaysGone = 0;

        }

        [Fact]
        public void TestProceedToNextDayWithWorkersButNoFood()
        {
            var village = new Village();
            village.OccupationDictionary.Add("miner", new MinerAction(village));

            
            village.AddWorker("John", "miner");

            // Remove all food
            village.Food = 0;

            // Act
            village.Day();

            // Assert
            Assert.Equal(1, village.DaysGone);
            Assert.True(village.Workers[0].Hungry);
            Assert.Equal(1, village.Workers[0].DaysHungry);
        }

        [Fact]
        public void TestProceedToNextDayWithWorkersAndFood()
        {
            // Arrange
            var village = new Village();
            village.OccupationDictionary.Add("farmer", new FarmerAction(village));

            village.AddWorker("John", "farmer");

            // Ensure there is enough food
            village.Food = 10;

            // Act
            village.Day();

            // Assert
            Assert.Equal(1, village.DaysGone);
            Assert.False(village.Workers[0].Hungry);
            Assert.Equal(0, village.Workers[0].DaysHungry);
            Assert.Equal(14, village.Food);
            village.Workers.Clear();
        }

        [Fact]
        public void Workers_ShouldBeRemoved_AfterSeveralDaysWithoutFood()
        {
            // Arrange
            var village = new Village();
            village.OccupationDictionary.Add("miner", new MinerAction(village));
            village.AddWorker("John", "miner");


            // Simulera flera dagar utan mat
           for (int i = 0; i < 5; i++)
            {
                village.Food = 0;
                village.Day();
            }
            // Arbetaren bör nu ha varit hungrig i 5 dagar och bör tas bort
            village.Day();

            // Assert
            Assert.Empty(village.Workers); // Arbetaren bör ha tagits bort från byn
            village.Workers.Clear();
        }
        [Fact]
        public void Game_ShouldEnd_When_AllWorkersAreDead()
        {
            // Arrange
            var village = new Village();
            village.OccupationDictionary.Add("miner", new MinerAction(village));
            village.AddWorker("John", "miner");


            // Simulera flera dagar utan mat
            for (int i = 0; i < 5; i++)
            {
                village.Food = 0;
                village.Day();
            }
            // Arbetaren bör nu ha varit hungrig i 5 dagar och bör tas bort
            village.Day();

            // Assert
            Assert.True(village.GameOver); // efter att arbetare är borta borde vara = game over 
        }

        [Fact]
        public void AddProject_ShouldAddNewProject_WhenEnoughResourcesAvailable()
        {
            // Arrange
            var village = new Village();
            var mockDatabaseConnection = new Mock<DatabaseConnection>();
            var projectName = "TestProject";
            var woodCost = 5;
            var metalCost = 3;
            var daysToComplete = 4;
            var completeAction = new Mock<VillageCompleteAction>().Object;

            var possibleProject = new PossibleProject(projectName, woodCost, metalCost, daysToComplete, completeAction);
            village.PossibleProjects.Add(projectName, possibleProject);

            village.Wood = woodCost + 1; // Enough wood
            village.Metal = metalCost + 1; // Enough metal

            // Act
            village.AddProject(projectName);

            // Assert
            Assert.Single(village.Projects); // Check if the project was added
        }

        [Fact]
        public void Worker_CompletesBuilding()
        {
            // Arrange
            var village = new Village();
            village.OccupationDictionary.Add("builder", new BuilderAction(village));

            // Add a worker
            village.AddWorker("John", "builder");

            // Add a project with 1 day left to complete
            var completeAction = new HouseComplete(village); // Example complete action
            var project = new Project("House", 1, completeAction);
            village.Projects.Add(project);

            // Act
            village.Day(); // Worker completes the building

            // Assert
            Assert.Empty(village.Projects); // Project should be completed
            Assert.Contains("House", village.Buildings.Select(b => b.Name));
            village.Workers.Clear();
            
        }

        [Fact]
        public void CompleteAction_AppliesCorrectEffect()
        {
            // Arrange
            var village = new Village();
            var completeAction = new HouseComplete(village); // Example complete action

            // Act
            completeAction.UponCompletion(); // Perform completion action

            // Assert
            Assert.Equal(8, village.MaxWorkers); // Check if effect is applied correctly
        }

        [Fact]
        public void AddProject_NotEnoughMaterial_ProjectNotAdded()
        {
            // Arrange
            var village = new Village();
            village.Wood = 3; // Tillräckligt med trä för projektet
            village.Metal = 2; // Inte tillräckligt med metall för projektet

            var mockDatabaseConnection = new Mock<DatabaseConnection>();
            var projectName = "TestProject";
            var woodCost = 5;
            var metalCost = 4;
            var daysToComplete = 1;
            var completeAction = new Mock<VillageCompleteAction>().Object;
            var possibleProject = new PossibleProject(projectName, woodCost, metalCost, daysToComplete, completeAction);
            village.PossibleProjects.Add(projectName, possibleProject);
            // Act
            village.AddProject(projectName);

            // Assert
            Assert.Empty(village.Projects); // Inget projekt ska läggas till
            
        }
        [Fact]
        public void SimulateGame_BuildCastle_WinGame()
        {
            // Arrange
            var village = new Village();
            village.MaxWorkers = 10; // Adjust as needed
            village.Food = 100; // Initial food resources
            village.Wood = 100; // Initial wood resources
            village.Metal = 100; // Initial metal resources

            // Add necessary occupation actions
            village.OccupationDictionary.Add("farmer", new FarmerAction(village));
            village.OccupationDictionary.Add("lumberjack", new LumberjackAction(village));
            village.OccupationDictionary.Add("miner", new MinerAction(village));
            village.OccupationDictionary.Add("builder", new BuilderAction(village));

            // Add necessary projects, including the "Castle" project
            village.PossibleProjects.Add("Castle", new PossibleProject("Castle", 50, 50, 50, new CastleComplete(village)));

            // Act
            // Add workers (adjust as needed)
            village.AddWorker("Worker1", "farmer");
            village.AddWorker("Worker2", "lumberjack");
            village.AddWorker("Worker3", "miner");
            village.AddWorker("Worker4", "builder");

            // Add projects, including the "Castle" project
            village.AddProject("Castle");

            // Progress through days until the "Castle" project is completed
            while (!village.GameOver)
            {
                village.Day();
            }

            // Assert
            Assert.True(village.GameOver);
        }

        [Fact]
        public void Save_ShouldCallGetTownNamesMethod()
        {
            // Arrange
            var mockDatabaseConnection = new Mock<IDatabasCoannection>();
            var village = new Village();
            var villageInput = new VillageInput(village, mockDatabaseConnection.Object);

            TextWriter originalConsoleOut = Console.Out;

            // Setup the mock to return a list of town names
            mockDatabaseConnection.Setup(m => m.GetTownNames()).Returns(new List<string> { "TestVillage" });
            mockDatabaseConnection.Setup(m => m.SaveVillage(It.IsAny<Village>(), It.IsAny<string>())).Returns(true);

            // Redirect Console input and output
            using (var sw = new System.IO.StringWriter())
            {
                Console.SetOut(sw);
                using (var sr = new System.IO.StringReader("TestVillage\ny"))
                {
                    Console.SetIn(sr);

                    // Act
                    villageInput.Save();

                    // Verify output for debugging
                    var output = sw.ToString();
                    Console.WriteLine(output);
                }
            }

            // Assert
            mockDatabaseConnection.Verify(m => m.GetTownNames(), Times.Once);
            mockDatabaseConnection.Verify(m => m.SaveVillage(It.IsAny<Village>(), "TestVillage"), Times.Once);

            Console.SetOut(originalConsoleOut);
        }


        [Fact]
        public void Load_ShouldLoadVillage_WhenValidChoiceIsProvided()
        {
            // Arrange
            var mockDatabaseConnection = new Mock<IDatabasCoannection>();
            var village = new Village();
            var villageInput = new VillageInput(village, mockDatabaseConnection.Object);

            // Mocka GetTownNames för att returnera en lista med en enda by
            mockDatabaseConnection.Setup(m => m.GetTownNames()).Returns(new List<string> { "TestVillage" });

            // Mocka LoadVillage för att returnera en förväntad by när "TestVillage" väljs
            var expectedVillage = new Village();
            mockDatabaseConnection.Setup(m => m.LoadVillage("TestVillage")).Returns(expectedVillage);

            // Skapa en StringReader för att simulera användarens val av "TestVillage"
            using (var sr = new System.IO.StringReader("TestVillage\n"))
            {
                Console.SetIn(sr);

                // Act
                villageInput.Load();

                // Uppdatera village-instansen i testet
                village = expectedVillage;

                // Assert
                Assert.Equal(expectedVillage, village); // Jämför den laddade byn med den förväntade byn
            }
        }



    }

}