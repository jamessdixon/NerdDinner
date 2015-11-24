using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;

namespace NerdDinner.Models
{
    [Table("RSVPs")]
    [DataServiceKey("RsvpID")]
    public class RSVP
    {
        public int RsvpID { get; set; }
        public int DinnerID { get; set; }
        public string AttendeeName { get; set; }
        public string AttendeeNameId { get; set; }

        public virtual Dinner Dinner { get; set; }
    }
}