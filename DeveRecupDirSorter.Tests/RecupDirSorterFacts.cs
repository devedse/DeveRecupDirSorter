using System;
using Xunit;

namespace DeveRecupDirSorter.Tests
{
    public class RecupDirSorterFacts
    {
        [Fact]
        public void ConstructorWorks()
        {
            //Arrange
            var recupDirSorter = new RecupDirSorter(@"C:\Test", FileActionType.Copy);

            //Act

            //Assert
            Assert.NotNull(recupDirSorter);
        }
    }
}
