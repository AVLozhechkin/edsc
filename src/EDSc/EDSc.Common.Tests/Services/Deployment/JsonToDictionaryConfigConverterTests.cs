namespace EDSc.Common.Tests.Services.Deployment
{
    using EDSc.Common.Services.Deployment.Util;
    using NUnit.Framework;
    
    [TestFixture]
    public class JsonToDictionaryConfigConverterTests
    {
        [Test]
        public void ShouldConvertJsonToDictionary()
        {
            // Arrange
            var converter = new JsonToDictionaryConfigConverter();
            var json = "{\"Outer1\": {\"Inner1\": \"Value1\",\"Inner2\": \"Value2\"},\"Outer2\": {\"Inner1\": {\"InnerInner1\": \"Value1\"}}}";

            // Act
            var dic = converter.Convert(json);
            
            // Assert
            Assert.IsTrue(dic.ContainsKey("Outer1_Inner1") && dic["Outer1_Inner1"] == "Value1"
                && dic.ContainsKey("Outer1_Inner2") && dic["Outer1_Inner2"] == "Value2"
                && dic.ContainsKey("Outer2_Inner1_InnerInner1") && dic["Outer2_Inner1_InnerInner1"] == "Value1");
        }
    }
}