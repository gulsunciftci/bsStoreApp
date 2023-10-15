using Entities.Models;


namespace Entities.LinkModels
{
    /// <summary>
    /// Linklerin organizasyonu gerçekleştiriliyor
    /// </summary>
    public class LinkResponse
    {
        public bool HasLinks { get; set; }
        public List<Entity> ShapedEntities { get; set; }
        public LinkCollectionWrapper<Entity> LinkedEntities { get; set; }
        public LinkResponse()
        {
            ShapedEntities = new List<Entity>();
            LinkedEntities = new LinkCollectionWrapper<Entity>();
        }
    }
}
