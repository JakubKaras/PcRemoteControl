using NetworkCommunicator.Api.Interfaces;
using System.Xml;

namespace NetworkCommunicator.Xml
{
    internal class XmlFileStore : IXmlFileStore
    {
        public XmlReader CreateXmlReader(string path) => XmlReader.Create(path);

        public XmlWriter CreateXmlWriter(string path, XmlWriterSettings settings) =>
            XmlWriter.Create(path, settings);

        public void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
