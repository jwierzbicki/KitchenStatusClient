using LiteDB;

namespace KitchenStatusServer.Models
{
    public class StatusUpdate
    {
        public StatusUpdate()
        {
        }

        [BsonId]
        [BsonField("_id")]
        public ObjectId Id { get; set; }
        [BsonField("user")]
        public string User { get; set; }
        [BsonField("changes")]
        public StatusChange[] Changes { get; set; }
    }

    public class StatusChange
    {
        public StatusChange()
        {
        }

        [BsonField("name")]
        public string Name { get; set; }
        [BsonField("quantity")]
        public double Quantity { get; set; }
    }
}
