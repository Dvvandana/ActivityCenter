using System.ComponentModel.DataAnnotations;

namespace ActivityCenter.Models{

    public class Participation{
        [Key]
        public int ParticipationId{get;set;}

        public int ParticipantId{get;set;}
        public User Participant{get;set;}

        public int JoiningActivityId{get;set;}
        public Happening JoiningActivity{get;set;} 
    }
}