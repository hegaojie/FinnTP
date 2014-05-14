using System;

namespace FinnTorget
{
    public class TorgetItem
    {
        public int SequenceNo { get; set; }
        public string ID { get; set; }
        public string Text { get; set; }
        public string ImageURL { get; set; }
        public string URL { get; set; }
        public DateTime PublishTime { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public override bool Equals(object obj)
        {
            var torgetItem = obj as TorgetItem;
            return torgetItem != null && String.CompareOrdinal(ID, torgetItem.ID) == 0;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}