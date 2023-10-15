namespace Entities.LinkModels
{
    //Bir kaynağa ait linklerin organizasyonunu yapacak bu ifade
    public class LinkResourceBase
    {
        public LinkResourceBase()
        {

        }


        public List<Link> Links { get; set; } = new List<Link>();
    }
   
        
}
