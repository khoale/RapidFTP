namespace RapidFTP.Chilkat.Tests.Utilities
{
    using System.Diagnostics;

    using RapidFTP.Models;
    using RapidFTP.Utilities;

    using Xunit;
    using Xunit.Extensions;

    public class UnixPathTest
    {
        [InlineData("/lv1", 1)]
        [InlineData("/lv1/lv2", 2)]
        [InlineData("/lv1/lv2/", 2)]
        [InlineData("/lv1/lv2/lv3", 3)]
        [InlineData("/lv1/lv2/lv3/lv4", 4)]
        [InlineData("/lv1/lv2/lv3/lv4/lv5", 5)]
        [Theory]
        public void GetRelatedDirectories_GivenPath_ShouldSuccess(string path, int length)
        {
            var relatedDirectories = UnixPath.GetRelatedDirectories(path);

            Assert.Equal(length, relatedDirectories.Length);
            foreach (var directory in relatedDirectories)
            {
                Trace.WriteLine(directory);
            }
        }

        [InlineData("/", true)]
        [InlineData("/lv1", true)]
        [InlineData("/lv1/lv2", true)]
        [InlineData("/lv1/lv2/", true)]
        [InlineData("/lv1/lv2/lv_3", true)]
        [InlineData("/lv1/lv2/lv-3", true)]
        [InlineData("lv1", false)]
        [InlineData("lv1/lv2", false)]
        [InlineData("", false)]
        [InlineData("/lv1/*", false)]
        [Theory]
        public void IsWellFormed(string path, bool expected)
        {
            var result = UnixPath.IsWellFormed(path, ItemType.Directory);
            Assert.Equal(expected, result);
        }
    }
}