namespace Project.Model.Server.XML_Load
{
    public interface IPurchasable : ILoadXMLBase
    {
        int Cost { get; set; }
    }
}