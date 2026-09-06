using System.Xml;

namespace NetworkCommunicator.Api.Interfaces
{
    public interface IXmlFileStore
    {
        XmlReader CreateXmlReader(string path);

        XmlWriter CreateXmlWriter(string path, XmlWriterSettings settings);

        void Delete(string path);
    }
}
