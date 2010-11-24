using System;
using System.Xml;
using NUnit.Framework;

namespace XmlToObjectParser.Tests
{
    [TestFixture]
    public class XmlToObjectParserTest
    {
        private string _sampleXml = String.Empty;
        
        [SetUp]
        public void Setup()
        {
            _sampleXml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?>
                            <catalog>
                              <cd country=""USA"">
                                <title>Empire Burlesque</title>
                                <artist>Bob Dylan</artist>
                                <price>10.90</price>
                              </cd>
                              <cd country=""UK"">
                                <title>Hide your heart</title>
                                <artist>Bonnie Tyler</artist>
                                <price>10.0</price>
                              </cd>
                              <cd country=""USA"">
                                <title>Greatest Hits</title>
                                <artist>Dolly Parton</artist>
                                <price>9.90</price>
                              </cd>
                            </catalog>";
        }
        
        [Test]
        public void classic_xml_handling()
        {            
            var catalog = new XmlDocument();
            catalog.LoadXml(_sampleXml);

            var numberOfCDsinCatalog = catalog["catalog"].ChildNodes.Count;
            var titleFromUKCD = catalog.SelectSingleNode("/catalog/cd[2]/title").InnerText;
            
            Assert.That(numberOfCDsinCatalog == 3);
            Assert.That(titleFromUKCD == "Hide your heart");
        }

        [Test]
        public void xml_to_object_parsing()
        {
            var catalog = DynamicXmlParser.ParseFromXml(_sampleXml);

            var numberOfCDsinCatalog = catalog.catalog.cd.Count;
            var titleFromUKCD = catalog.catalog.cd[1].title;            

            Assert.That(numberOfCDsinCatalog == 3);
            Assert.That(titleFromUKCD == "Hide your heart");
        }
    }
}
